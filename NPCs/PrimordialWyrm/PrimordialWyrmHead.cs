using System;
using System.IO;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Furniture.DevPaintings;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.Sounds;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.PrimordialWyrm
{
    [AutoloadBossHead]
    [LongDistanceNetSync]
    public class PrimordialWyrmHead : ModNPC
    {
        public enum Phase
        {
            ChargeOne = 0,
            LightningRain = 1,
            FastCharge = 2,
            EidolonWyrmSpawn = 3,
            ChargeTwo = 4,
            IceMist = 5,
            ShadowFireballSpin = 6,
            AncientDoomSummon = 7,
            LightningCharge = 8,
            EidolistSpawn = 9,
            FinalPhase = 10
        }

        public float AIState
        {
            get => NPC.Calamity().newAI[0];
            set => NPC.Calamity().newAI[0] = value;
        }

        // Base distance from the target for most attacks
        private const float baseDistance = 1000f;

        // The distance from target location in order to initiate an attack
        private const float baseAttackTriggerDistance = 80f;

        // Max distance from the target before they are unable to hear sound telegraphs
        private const float soundDistance = 2800f;

        // Length variables
        private const int minLength = 40;
        private const int maxLength = 41;

        // Variable used to stop the segment spawning loop
        private bool TailSpawned = false;

        // The direction to spin in during spin phases
        private int rotationDirection = 0;

        // Used in the lerp to smoothly scale velocity up and down
        private float chargeVelocityScalar = 0f;

        // How much less time is needed before activating fast charge
        private const float fastChargeGateValue = 120f;

        //Sounds
        public static readonly SoundStyle SpawnSound = new("CalamityMod/Sounds/Custom/Scare");

        public static readonly SoundStyle ChargeSound = new("CalamityMod/Sounds/Custom/PrimordialWyrmCharge");
        public static readonly SoundStyle DeathSound = new("CalamityMod/Sounds/NPCKilled/PrimordialWyrmDeath");

        public static Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.50f,
                PortraitScale = 0.6f,
                PortraitPositionXOverride = 40,
                CustomTexturePath = "CalamityMod/ExtraTextures/Bestiary/PrimordialWyrm_Bestiary"
            };
            value.Position.X += 55;
            value.Position.Y += 5;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.AsyncLoad);
            }
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.npcSlots = 50f;
            NPC.GetNPCDamage();
            NPC.width = 254;
            NPC.height = 138;
            NPC.LifeMaxNERB(2500000, 3000000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.Opacity = 0f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(10, 0, 0, 0);
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = DeathSound;
            NPC.netAlways = true;
            NPC.boss = true;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;

            if (Main.zenithWorld)
            {
                NPC.defense = 999;
                NPC.DR_NERD(0.9f);
            }
            else
            {
                NPC.defense = 100;
                NPC.DR_NERD(0.4f);
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                //AAAAAAAAAAAAH Scary abyss superboss guy so he gets pitch black bg and no biome source.
                //eidolon wyrm comment jumpscare!!!!!!
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement(),
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.PrimordialWyrm")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(rotationDirection);
            writer.Write(chargeVelocityScalar);
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            rotationDirection = reader.ReadInt32();
            chargeVelocityScalar = reader.ReadSingle();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            CalamityGlobalNPC.adultEidolonWyrmHead = NPC.whoAmI;

            // Difficulty modes
            bool death = CalamityWorld.death;
            bool revenge = CalamityWorld.revenge;
            bool expertMode = Main.expertMode;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Phases
            bool phase2 = lifeRatio < 0.8f;
            bool phase3 = lifeRatio < 0.6f;
            bool phase4 = lifeRatio < 0.4f;
            bool phase5 = lifeRatio < 0.2f;
            bool phase6 = lifeRatio < 0.05f;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            // Target variable
            Player player = Main.player[NPC.target];

            bool targetDownDeep = player.Calamity().ZoneAbyssLayer4 || BossRushEvent.BossRushActive;
            bool targetOnMount = player.mount.Active;

            // Check whether enraged for the sake of the HP bar UI
            NPC.Calamity().CurrentlyEnraged = !targetDownDeep;

            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            // Play spawn sound
            if (!TailSpawned && NPC.ai[0] == 0f)
            {
                if (Main.player[Main.myPlayer].active && !Main.player[Main.myPlayer].dead && Vector2.Distance(Main.player[Main.myPlayer].Center, NPC.Center) < soundDistance)
                    SoundEngine.PlaySound(SpawnSound, Main.player[Main.myPlayer].Center);
            }

            // Spawn segments
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!TailSpawned && NPC.ai[0] == 0f)
                {
                    int Previous = NPC.whoAmI;
                    for (int i = 0; i < maxLength; i++)
                    {
                        int lol;
                        if (i >= 0 && i < minLength)
                        {
                            if (i % 2 == 0)
                                lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<PrimordialWyrmBody>(), NPC.whoAmI);
                            else
                                lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<PrimordialWyrmBodyAlt>(), NPC.whoAmI);
                        }
                        else
                            lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<PrimordialWyrmTail>(), NPC.whoAmI);

                        Main.npc[lol].realLife = NPC.whoAmI;
                        Main.npc[lol].ai[2] = NPC.whoAmI;
                        Main.npc[lol].ai[1] = Previous;
                        Main.npc[Previous].ai[0] = lol;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, lol, 0f, 0f, 0f, 0);
                        Previous = lol;
                        Main.npc[Previous].ai[3] = i / 2;
                    }
                    TailSpawned = true;
                }
            }

            // Despawn if target is dead
            bool targetDead = false;
            if (player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (player.dead)
                {
                    targetDead = true;
                    NPC.ai[3] = 0f;
                    NPC.localAI[0] = 0f;
                    NPC.localAI[1] = 0f;
                    NPC.localAI[2] = 0f;
                    NPC.localAI[3] = 0f;
                    AIState = (float)Phase.ChargeOne;
                    calamityGlobalNPC.newAI[1] = 0f;
                    calamityGlobalNPC.newAI[2] = 0f;
                    chargeVelocityScalar = 0f;
                    rotationDirection = 0;

                    NPC.velocity.Y += 3f;
                    if (NPC.position.Y > Main.worldSurface * 16.0)
                        NPC.velocity.Y += 3f;

                    if (NPC.position.Y > Main.rockLayer * 16.0)
                    {
                        for (int a = 0; a < Main.maxNPCs; a++)
                        {
                            if (Main.npc[a].type == NPC.type || Main.npc[a].type == ModContent.NPCType<PrimordialWyrmBodyAlt>() || Main.npc[a].type == ModContent.NPCType<PrimordialWyrmBody>() || Main.npc[a].type == ModContent.NPCType<PrimordialWyrmTail>())
                                Main.npc[a].active = false;
                        }
                    }
                }
            }

            // General AI pattern
            // Charge
            // Charge : Phase 2 - Spin around target and summon Shadow Fireballs
            // Charge : Phase 4 - Swim to the right and dash towards the target, summon Lightning Bolts from above during it
            // Turn invisible, swim above the target and summon predictive Lightning Bolts
            // Turn visible and charge towards the target quickly 1 time, soon after the previous attack ends
            // Spawn an Eidolon Wyrm and swim below the target for 10 seconds, or less, if the Wyrm dies
            // Charge
            // Charge : Phase 3 - Turn invisible and summon Ancient Dooms around the target
            // Charge : Phase 4 - Turn visible, swim to the left and dash towards the target, summon Lightning Bolts from above during it
            // Turn invisible, swim beneath the target and summon Ice Mist
            // Turn visible and charge towards the target quickly 1 time, soon after the previous attack ends
            // Spawn Eidolists and swim below the target for 10 seconds, or less, if the Eidolists die

            // Final phase
            // Spin around the target and summon Ancient Dooms and Shadow Fireballs

            // Attack patterns
            // Phase 1 - 0, 0, 0, 1, 2, 3, 4, 4, 4, 5, 2, 9
            // Phase 2 - 0, 6, 0, 1, 2, 3, 4, 4, 4, 5, 2, 9
            // Phase 3 - 0, 6, 0, 1, 2, 3, 4, 7, 4, 5, 2, 9
            // Phase 4 - 0, 6, 8, 1, 2, 3, 4, 7, 8, 5, 2, 9
            // Phase 5 - 0, 6, 8, 1, 2, 2, 4, 7, 8, 5, 2, 2
            // Phase 6 - 10

            // Phase gate values
            float chargePhaseGateValue = death ? 180f : revenge ? 210f : expertMode ? 240f : 300f;
            float lightningRainDuration = 180f;
            float eidolonWyrmPhaseDuration = death ? 120f : revenge ? 135f : expertMode ? 150f : 180f;
            float iceMistDuration = 180f;
            float spinPhaseDuration = death ? 240f : revenge ? 255f : expertMode ? 270f : 300f;
            float ancientDoomPhaseGateValue = 30f;
            float ancientDoomGateValue = death ? 95f : revenge ? 100f : expertMode ? 105f : 120f;
            float lightningChargePhaseGateValue = death ? 120f : revenge ? 135f : expertMode ? 150f : 180f;

            if (Main.getGoodWorld)
            {
                lightningRainDuration *= 0.5f;
                eidolonWyrmPhaseDuration *= 0.25f;
                iceMistDuration *= 0.5f;
            }

            // Adjust slowing debuff immunity
            bool immuneToSlowingDebuffs = AIState == (float)Phase.FinalPhase || AIState == (float)Phase.ShadowFireballSpin;
            NPC.buffImmune[ModContent.BuffType<GlacialState>()] = immuneToSlowingDebuffs;
            NPC.buffImmune[ModContent.BuffType<TemporalSadness>()] = immuneToSlowingDebuffs;
            NPC.buffImmune[ModContent.BuffType<KamiFlu>()] = immuneToSlowingDebuffs;
            NPC.buffImmune[ModContent.BuffType<Eutrophication>()] = immuneToSlowingDebuffs;
            NPC.buffImmune[ModContent.BuffType<TimeDistortion>()] = immuneToSlowingDebuffs;
            NPC.buffImmune[ModContent.BuffType<GalvanicCorrosion>()] = immuneToSlowingDebuffs;
            NPC.buffImmune[ModContent.BuffType<Vaporfied>()] = immuneToSlowingDebuffs;
            NPC.buffImmune[BuffID.Slow] = immuneToSlowingDebuffs;
            NPC.buffImmune[BuffID.Webbed] = immuneToSlowingDebuffs;

            // Adjust opacity
            bool invisiblePartOfChargePhase = calamityGlobalNPC.newAI[2] >= chargePhaseGateValue && calamityGlobalNPC.newAI[2] <= chargePhaseGateValue + 1f && (AIState == (float)Phase.ChargeOne || AIState == (float)Phase.ChargeTwo || AIState == (float)Phase.FastCharge);
            bool invisiblePartOfLightningChargePhase = calamityGlobalNPC.newAI[2] >= lightningChargePhaseGateValue && calamityGlobalNPC.newAI[2] <= lightningChargePhaseGateValue + 1f && AIState == (float)Phase.LightningCharge;
            bool invisiblePhase = AIState == (float)Phase.LightningRain || AIState == (float)Phase.IceMist || AIState == (float)Phase.AncientDoomSummon;
            if (!invisiblePartOfChargePhase && !invisiblePartOfLightningChargePhase && !invisiblePhase)
            {
                NPC.Opacity += 0.2f;
                if (NPC.Opacity > 1f)
                    NPC.Opacity = 1f;
            }
            else
            {
                NPC.Opacity -= 0.05f;
                if (NPC.Opacity < 0f)
                    NPC.Opacity = 0f;
            }

            // Direction and rotation
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            int direction = NPC.direction;
            NPC.direction = NPC.spriteDirection = (NPC.velocity.X > 0f) ? 1 : (-1);
            if (direction != NPC.direction)
                NPC.netUpdate = true;

            // Default vector to swim to
            Vector2 destination = player.Center;

            // Charge variables
            Vector2 chargeVector = Vector2.Zero;
            float chargeDistance = baseDistance;
            float chargeLocationDistance = baseAttackTriggerDistance;
            switch ((int)calamityGlobalNPC.newAI[1])
            {
                case 0:
                    chargeVector.X -= chargeDistance;
                    break;
                case 1:
                    chargeVector.X += chargeDistance;
                    break;
                case 2:
                    chargeVector.Y -= chargeDistance;
                    break;
                case 3:
                    chargeVector.Y += chargeDistance;
                    break;
                case 4:
                    chargeVector.X -= chargeDistance;
                    chargeVector.Y -= chargeDistance;
                    break;
                case 5:
                    chargeVector.X += chargeDistance;
                    chargeVector.Y += chargeDistance;
                    break;
                case 6:
                    chargeVector.X -= chargeDistance;
                    chargeVector.Y += chargeDistance;
                    break;
                case 7:
                    chargeVector.X += chargeDistance;
                    chargeVector.Y -= chargeDistance;
                    break;
            }
            Vector2 chargeLocation = destination + chargeVector;

            // Lightning Rain variables
            Vector2 lightningRainLocation = new Vector2(0f, -baseDistance);
            float lightningRainLocationDistance = baseAttackTriggerDistance;

            // Wyrm and Eidolist variables
            Vector2 eidolonWyrmPhaseLocation = new Vector2(0f, baseDistance);
            int eidolistScale = death ? 3 : revenge ? 2 : expertMode ? 1 : 0;
            int maxEidolists = (targetDownDeep ? 3 : 6) + eidolistScale;

            // Ice Mist variables
            Vector2 iceMistLocation = new Vector2(0f, baseDistance);
            float iceMistLocationDistance = baseAttackTriggerDistance;

            // Spin variables
            float spinRadius = baseDistance;
            Vector2 spinLocation = new Vector2(0f, -spinRadius);
            float spinLocationDistance = baseAttackTriggerDistance;

            // Ancient Doom variables
            Vector2 ancientDoomLocation = new Vector2(0f, -baseDistance);
            int ancientDoomScale = death ? 3 : revenge ? 2 : expertMode ? 1 : 0;
            int ancientDoomLimit = (targetDownDeep ? 4 : 8) + ancientDoomScale;
            int ancientDoomDistance = death ? 520 : revenge ? 535 : expertMode ? 550 : 600;
            float maxAncientDoomRings = 3f;

            // Lightning charge variables
            Vector2 lightningChargeVector = NPC.localAI[2] == 0f ? new Vector2(baseDistance, 0f) : new Vector2(-baseDistance, 0f);
            float lightningChargeLocationDistance = baseAttackTriggerDistance;
            Vector2 lightningChargeLocation = destination + lightningChargeVector;
            float lightningSpawnY = 540f;
            Vector2 lightningSpawnLocation = new Vector2(NPC.Center.X, NPC.Center.Y - lightningSpawnY);
            int numLightningBolts = death ? 10 : revenge ? 8 : expertMode ? 6 : 4;
            float distanceBetweenBolts = lightningSpawnY * 2f / numLightningBolts;

            // Velocity and turn speed values
            float velocityScale = death ? 1.8f : revenge ? 1.5f : expertMode ? 1.2f : 0f;
            float baseVelocity = (targetDownDeep ? 10f : 15f) + (targetDownDeep ? velocityScale : velocityScale * 1.5f);
            if (Main.getGoodWorld)
                baseVelocity *= 1.15f;

            float turnSpeed = baseVelocity * 0.015f;
            float normalChargeVelocityMult = MathHelper.Lerp(1f, 2f, chargeVelocityScalar);
            float normalChargeTurnSpeedMult = MathHelper.Lerp(1f, 4f, chargeVelocityScalar);
            float invisiblePhaseVelocityMult = MathHelper.Lerp(1f, 1.5f, chargeVelocityScalar);
            float invisiblePhaseTurnSpeedMult = MathHelper.Lerp(1f, 3f, chargeVelocityScalar);
            float fastChargeVelocityMult = MathHelper.Lerp(1f, 3f, chargeVelocityScalar);
            float fastChargeTurnSpeedMult = MathHelper.Lerp(1f, 8f, chargeVelocityScalar);
            float chargeVelocityScalarIncrement = 0.005f;
            float totalChargeDistance = 3000f;

            bool lookingAtTarget = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY).ToRotation().AngleTowards(NPC.velocity.ToRotation(), MathHelper.PiOver4) == NPC.velocity.ToRotation();

            // Telekinesis while enraged
            if (!targetDownDeep)
            {
                calamityGlobalNPC.newAI[3] += 1f;
                if (calamityGlobalNPC.newAI[3] >= 300f)
                {
                    calamityGlobalNPC.newAI[3] = 0f;

                    SoundEngine.PlaySound(SoundID.Item117, player.Center);

                    for (int i = 0; i < 20; i++)
                    {
                        int dust = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, DustID.FrostHydra, 0f, 0f, 100, default, 2f);
                        Main.dust[dust].velocity *= 0.6f;
                        if (Main.rand.NextBool())
                        {
                            Main.dust[dust].scale = 0.5f;
                            Main.dust[dust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                        }
                    }

                    for (int j = 0; j < 30; j++)
                    {
                        int dust = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, DustID.FrostHydra, 0f, 0f, 100, default, 3f);
                        Main.dust[dust].noGravity = true;
                        dust = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, DustID.FrostHydra, 0f, 0f, 100, default, 2f);
                        Main.dust[dust].velocity *= 0.2f;
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (player.velocity.Length() > 0f)
                            player.velocity *= -3f;
                    }
                }
            }

            // Phase switch
            switch ((int)AIState)
            {
                // First charge combo
                case (int)Phase.ChargeOne:

                    if (calamityGlobalNPC.newAI[2] >= chargePhaseGateValue)
                    {
                        ChargeDust(7, (float)Math.PI);

                        // Use a lerp to smoothly scale up velocity and turn speed
                        chargeVelocityScalar += chargeVelocityScalarIncrement;
                        if (chargeVelocityScalar > 1f)
                            chargeVelocityScalar = 1f;

                        baseVelocity *= normalChargeVelocityMult;
                        turnSpeed *= normalChargeTurnSpeedMult;

                        if ((chargeLocation - NPC.Center).Length() < chargeLocationDistance || calamityGlobalNPC.newAI[2] > chargePhaseGateValue)
                        {
                            // Set the scalar to max
                            if (chargeVelocityScalar < 1f)
                                chargeVelocityScalar = 1f;

                            // Lock into looking at the target
                            if (calamityGlobalNPC.newAI[2] < chargePhaseGateValue + 1f)
                                calamityGlobalNPC.newAI[2] += 1f;

                            // Turn towards the target
                            if (!lookingAtTarget && calamityGlobalNPC.newAI[2] < chargePhaseGateValue + 2f)
                            {
                                baseVelocity /= normalChargeVelocityMult;
                                turnSpeed /= normalChargeTurnSpeedMult;
                            }

                            // Charge at the target
                            else
                            {
                                if (calamityGlobalNPC.newAI[2] == chargePhaseGateValue + 1f)
                                {
                                    if (Main.player[Main.myPlayer].active && !Main.player[Main.myPlayer].dead && Vector2.Distance(Main.player[Main.myPlayer].Center, NPC.Center) < soundDistance)
                                        SoundEngine.PlaySound(ChargeSound, Main.player[Main.myPlayer].Center);
                                }

                                // Lock into the charge phase and use this for a charge time check
                                calamityGlobalNPC.newAI[2] += 1f;

                                // Become totally visible
                                NPC.Opacity = 1f;

                                float totalChargeTime = (totalChargeDistance / baseVelocity) + chargePhaseGateValue + 1f;
                                if (calamityGlobalNPC.newAI[2] > totalChargeTime)
                                {
                                    NPC.ai[3] += 1f;
                                    float maxCharges = phase4 ? 1 : phase2 ? 2 : 3;
                                    if (NPC.ai[3] >= maxCharges)
                                    {
                                        NPC.ai[3] = 0f;
                                        AIState = phase4 ? (float)Phase.ShadowFireballSpin : (float)Phase.LightningRain;
                                    }
                                    else if (phase2)
                                        AIState = (float)Phase.ShadowFireballSpin;

                                    calamityGlobalNPC.newAI[1] += 1f;
                                    if (calamityGlobalNPC.newAI[1] > 7f)
                                        calamityGlobalNPC.newAI[1] = 0f;

                                    calamityGlobalNPC.newAI[2] = 0f;
                                    chargeVelocityScalar = 0f;
                                    FinalPhaseCheck();
                                    NPC.TargetClosest();
                                }
                            }
                        }
                        else
                            destination += chargeVector;
                    }
                    else
                        calamityGlobalNPC.newAI[2] += 1f;

                    break;

                // Turn invisible, swim above and summon predictive lightning bolts
                case (int)Phase.LightningRain:

                    // Swim up
                    destination += lightningRainLocation;

                    // Use a lerp to smoothly scale up velocity and turn speed
                    chargeVelocityScalar += chargeVelocityScalarIncrement;
                    if (chargeVelocityScalar > 1f)
                        chargeVelocityScalar = 1f;

                    baseVelocity *= invisiblePhaseVelocityMult;
                    turnSpeed *= invisiblePhaseTurnSpeedMult;

                    if ((destination - NPC.Center).Length() < lightningRainLocationDistance || calamityGlobalNPC.newAI[2] > 0f)
                    {
                        if (calamityGlobalNPC.newAI[2] % 30f == 0f && calamityGlobalNPC.newAI[2] < lightningRainDuration)
                        {
                            if (Main.player[Main.myPlayer].active && !Main.player[Main.myPlayer].dead && Vector2.Distance(Main.player[Main.myPlayer].Center, NPC.Center) < soundDistance)
                                SoundEngine.PlaySound(CommonCalamitySounds.LightningSound, Main.player[Main.myPlayer].Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int maxTargets = 2;
                                int[] whoAmIArray = new int[maxTargets];
                                Vector2[] targetCenterArray = new Vector2[maxTargets];
                                int numProjectiles = 0;
                                float maxDistance = 2400f;

                                foreach (Player plr in Main.ActivePlayers)
                                {
                                    if (plr.dead)
                                        continue;

                                    Vector2 playerCenter = plr.Center;
                                    float distance = Vector2.Distance(playerCenter, NPC.Center);
                                    if (distance < maxDistance)
                                    {
                                        whoAmIArray[numProjectiles] = plr.whoAmI;
                                        targetCenterArray[numProjectiles] = playerCenter;
                                        int projectileLimit = numProjectiles + 1;
                                        numProjectiles = projectileLimit;
                                        if (projectileLimit >= targetCenterArray.Length)
                                            break;
                                    }
                                }

                                float predictionAmt = targetDownDeep ? 45f : 60f;
                                float lightningVelocityScale = death ? 0.9f : revenge ? 0.75f : expertMode ? 0.6f : 0f;
                                float lightningVelocity = ((targetDownDeep && !targetOnMount) ? 6f : 9f) + ((targetDownDeep && !targetOnMount) ? lightningVelocityScale : lightningVelocityScale * 1.5f);
                                for (int i = 0; i < numProjectiles; i++)
                                {
                                    // Predictive bolt
                                    Vector2 projectileDestination = targetCenterArray[i] + Main.player[whoAmIArray[i]].velocity * predictionAmt - NPC.Center;
                                    float ai = Main.rand.Next(100);
                                    Vector2 projectileVelocity = Vector2.Normalize(projectileDestination.RotatedByRandom(MathHelper.PiOver4)) * lightningVelocity;
                                    int type = ProjectileID.CultistBossLightningOrbArc;
                                    int damage = NPC.GetProjectileDamage(type);
                                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projectileVelocity, type, damage, 0f, Main.myPlayer, projectileDestination.ToRotation(), ai);
                                    Main.projectile[proj].tileCollide = false;

                                    // Opposite bolt
                                    projectileDestination = targetCenterArray[i] - Main.player[whoAmIArray[i]].velocity * predictionAmt - NPC.Center;
                                    ai = Main.rand.Next(100);
                                    projectileVelocity = Vector2.Normalize(projectileDestination.RotatedByRandom(MathHelper.PiOver4)) * lightningVelocity;
                                    proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projectileVelocity, type, damage, 0f, Main.myPlayer, projectileDestination.ToRotation(), ai);
                                    Main.projectile[proj].tileCollide = false;

                                    // Normal bolt
                                    projectileDestination = targetCenterArray[i] - NPC.Center;
                                    ai = Main.rand.Next(100);
                                    projectileVelocity = Vector2.Normalize(projectileDestination.RotatedByRandom(MathHelper.PiOver4)) * lightningVelocity;
                                    proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projectileVelocity, type, damage, 0f, Main.myPlayer, projectileDestination.ToRotation(), ai);
                                    Main.projectile[proj].tileCollide = false;
                                }
                            }
                        }

                        calamityGlobalNPC.newAI[2] += 1f;

                        Lighting.AddLight(NPC.Center, 0.4f, 0.85f, 0.9f);

                        float rotation = MathHelper.Clamp((float)Main.rand.NextDouble() * 1f - 0.5f, -0.5f, 0.5f);
                        Vector2 dustPosition = new Vector2(-NPC.width * 0.2f * NPC.scale, 0f).RotatedBy(rotation * ((float)Math.PI * 2f)).RotatedBy(NPC.velocity.ToRotation());
                        int dust = Dust.NewDust(NPC.Center - Vector2.One * 5f, 10, 10, DustID.Electric, (0f - NPC.velocity.X) / 3f, (0f - NPC.velocity.Y) / 3f, 150, Color.Transparent, 0.7f);
                        Main.dust[dust].position = NPC.Center + dustPosition;
                        Main.dust[dust].velocity = Vector2.Normalize(Main.dust[dust].position - NPC.Center) * 2f;
                        Main.dust[dust].noGravity = true;

                        rotation = MathHelper.Clamp((float)Main.rand.NextDouble() * 1f - 0.5f, -0.5f, 0.5f);
                        dustPosition = new Vector2(-NPC.width * 0.6f * NPC.scale, 0f).RotatedBy(rotation * ((float)Math.PI * 2f)).RotatedBy(NPC.velocity.ToRotation());
                        dust = Dust.NewDust(NPC.Center - Vector2.One * 5f, 10, 10, DustID.Electric, (0f - NPC.velocity.X) / 3f, (0f - NPC.velocity.Y) / 3f, 150, Color.Transparent, 0.7f);
                        Main.dust[dust].velocity = Vector2.Zero;
                        Main.dust[dust].position = NPC.Center + dustPosition;
                        Main.dust[dust].noGravity = true;

                        if (calamityGlobalNPC.newAI[2] >= lightningRainDuration)
                        {
                            NPC.localAI[0] = 0f;
                            AIState = (float)Phase.FastCharge;
                            calamityGlobalNPC.newAI[2] = fastChargeGateValue;
                            chargeVelocityScalar = 0f;
                            FinalPhaseCheck();
                            NPC.TargetClosest();
                        }
                    }

                    break;

                // Turn visible and charge quickly
                case (int)Phase.FastCharge:

                    if (calamityGlobalNPC.newAI[2] >= chargePhaseGateValue)
                    {
                        ChargeDust(7, (float)Math.PI);

                        // Use a lerp to smoothly scale up velocity and turn speed
                        chargeVelocityScalar += chargeVelocityScalarIncrement;
                        if (chargeVelocityScalar > 1f)
                            chargeVelocityScalar = 1f;

                        baseVelocity *= fastChargeVelocityMult;
                        turnSpeed *= fastChargeTurnSpeedMult;

                        if ((chargeLocation - NPC.Center).Length() < chargeLocationDistance || calamityGlobalNPC.newAI[2] > chargePhaseGateValue)
                        {
                            // Set the scalar to max
                            if (chargeVelocityScalar < 1f)
                                chargeVelocityScalar = 1f;

                            // Lock into looking at the target
                            if (calamityGlobalNPC.newAI[2] < chargePhaseGateValue + 1f)
                                calamityGlobalNPC.newAI[2] += 1f;

                            // Turn towards the target
                            if (!lookingAtTarget && calamityGlobalNPC.newAI[2] < chargePhaseGateValue + 2f)
                            {
                                baseVelocity /= fastChargeVelocityMult;
                                turnSpeed /= fastChargeTurnSpeedMult;
                            }

                            // Charge very quickly
                            else
                            {
                                if (calamityGlobalNPC.newAI[2] == chargePhaseGateValue + 1f)
                                {
                                    if (Main.player[Main.myPlayer].active && !Main.player[Main.myPlayer].dead && Vector2.Distance(Main.player[Main.myPlayer].Center, NPC.Center) < soundDistance)
                                        SoundEngine.PlaySound(ChargeSound, Main.player[Main.myPlayer].Center);
                                }

                                // Lock into the charge phase and use this for a charge time check
                                calamityGlobalNPC.newAI[2] += 1f;

                                // Become totally visible
                                NPC.Opacity = 1f;

                                float totalChargeTime = (totalChargeDistance / baseVelocity) + chargePhaseGateValue + 1f;
                                if (calamityGlobalNPC.newAI[2] > totalChargeTime)
                                {
                                    if (!phase5)
                                    {
                                        AIState = NPC.localAI[0] == 0f ? (float)Phase.EidolonWyrmSpawn : (float)Phase.EidolistSpawn;
                                        calamityGlobalNPC.newAI[2] = 0f;
                                    }
                                    else
                                    {
                                        NPC.ai[3] += 1f;
                                        if (NPC.ai[3] >= 2f)
                                        {
                                            NPC.ai[3] = 0f;
                                            AIState = NPC.localAI[0] == 0f ? (float)Phase.ChargeTwo : (float)Phase.ChargeOne;
                                            calamityGlobalNPC.newAI[2] = 0f;
                                        }
                                        else
                                            calamityGlobalNPC.newAI[2] = fastChargeGateValue;
                                    }

                                    calamityGlobalNPC.newAI[1] += 1f;
                                    if (calamityGlobalNPC.newAI[1] > 7f)
                                        calamityGlobalNPC.newAI[1] = 0f;

                                    chargeVelocityScalar = 0f;
                                    FinalPhaseCheck();
                                    NPC.TargetClosest();
                                }
                            }
                        }
                        else
                            destination += chargeVector;
                    }
                    else
                        calamityGlobalNPC.newAI[2] += 1f;

                    break;

                // Summon an Eidolon Wyrm and swim below the target for 3 seconds, or less, if the Wyrm dies
                case (int)Phase.EidolonWyrmSpawn:

                    destination += eidolonWyrmPhaseLocation;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (calamityGlobalNPC.newAI[2] == 0f)
                        {
                            if (!NPC.AnyNPCs(ModContent.NPCType<EidolonWyrmHead>()))
                                NPC.SpawnOnPlayer(NPC.FindClosestPlayer(), ModContent.NPCType<EidolonWyrmHead>());
                        }
                    }

                    calamityGlobalNPC.newAI[2] += 1f;

                    if (calamityGlobalNPC.newAI[2] >= eidolonWyrmPhaseDuration)
                    {
                        AIState = (float)Phase.ChargeTwo;
                        calamityGlobalNPC.newAI[2] = 0f;
                        FinalPhaseCheck();
                        NPC.TargetClosest();
                    }

                    break;

                // Second charge combo
                case (int)Phase.ChargeTwo:

                    if (calamityGlobalNPC.newAI[2] >= chargePhaseGateValue)
                    {
                        ChargeDust(7, (float)Math.PI);

                        // Use a lerp to smoothly scale up velocity and turn speed
                        chargeVelocityScalar += chargeVelocityScalarIncrement;
                        if (chargeVelocityScalar > 1f)
                            chargeVelocityScalar = 1f;

                        baseVelocity *= normalChargeVelocityMult;
                        turnSpeed *= normalChargeTurnSpeedMult;

                        if ((chargeLocation - NPC.Center).Length() < chargeLocationDistance || calamityGlobalNPC.newAI[2] > chargePhaseGateValue)
                        {
                            // Set the scalar to max
                            if (chargeVelocityScalar < 1f)
                                chargeVelocityScalar = 1f;

                            // Lock into looking at the target
                            if (calamityGlobalNPC.newAI[2] < chargePhaseGateValue + 1f)
                                calamityGlobalNPC.newAI[2] += 1f;

                            // Turn towards the target
                            if (!lookingAtTarget && calamityGlobalNPC.newAI[2] < chargePhaseGateValue + 2f)
                            {
                                baseVelocity /= normalChargeVelocityMult;
                                turnSpeed /= normalChargeTurnSpeedMult;
                            }

                            // Charge
                            else
                            {
                                if (calamityGlobalNPC.newAI[2] == chargePhaseGateValue + 1f)
                                {
                                    if (Main.player[Main.myPlayer].active && !Main.player[Main.myPlayer].dead && Vector2.Distance(Main.player[Main.myPlayer].Center, NPC.Center) < soundDistance)
                                        SoundEngine.PlaySound(ChargeSound, Main.player[Main.myPlayer].Center);
                                }

                                // Lock into the charge phase and use this for a charge time check
                                calamityGlobalNPC.newAI[2] += 1f;

                                // Become totally visible
                                NPC.Opacity = 1f;

                                float totalChargeTime = (totalChargeDistance / baseVelocity) + chargePhaseGateValue + 1f;
                                if (calamityGlobalNPC.newAI[2] > totalChargeTime)
                                {
                                    NPC.ai[3] += 1f;
                                    float maxCharges = phase4 ? 1 : phase3 ? 2 : 3;
                                    if (NPC.ai[3] >= maxCharges)
                                    {
                                        NPC.ai[3] = 0f;
                                        AIState = phase4 ? (float)Phase.AncientDoomSummon : (float)Phase.IceMist;
                                    }
                                    else if (phase3)
                                        AIState = (float)Phase.AncientDoomSummon;

                                    calamityGlobalNPC.newAI[1] += 1f;
                                    if (calamityGlobalNPC.newAI[1] > 7f)
                                        calamityGlobalNPC.newAI[1] = 0f;

                                    calamityGlobalNPC.newAI[2] = 0f;
                                    chargeVelocityScalar = 0f;
                                    FinalPhaseCheck();
                                    NPC.TargetClosest();
                                }
                            }
                        }
                        else
                            destination += chargeVector;
                    }
                    else
                        calamityGlobalNPC.newAI[2] += 1f;

                    break;

                // Turn invisible, swim beneath the target and summon ice mist
                case (int)Phase.IceMist:

                    // Swim down
                    destination += iceMistLocation;

                    // Use a lerp to smoothly scale up velocity and turn speed
                    chargeVelocityScalar += chargeVelocityScalarIncrement;
                    if (chargeVelocityScalar > 1f)
                        chargeVelocityScalar = 1f;

                    baseVelocity *= invisiblePhaseVelocityMult;
                    turnSpeed *= invisiblePhaseTurnSpeedMult;

                    if ((destination - NPC.Center).Length() < iceMistLocationDistance || calamityGlobalNPC.newAI[2] > 0f)
                    {
                        if (calamityGlobalNPC.newAI[2] % 60f == 0f && calamityGlobalNPC.newAI[2] < iceMistDuration && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int maxTargets = 2;
                            int[] whoAmIArray = new int[maxTargets];
                            Vector2[] targetCenterArray = new Vector2[maxTargets];
                            int numProjectiles = 0;
                            float maxDistance = 2400f;

                            foreach (Player plr in Main.ActivePlayers)
                            {
                                if (plr.dead)
                                    continue;

                                Vector2 playerCenter = plr.Center;
                                float distance = Vector2.Distance(playerCenter, NPC.Center);
                                if (distance < maxDistance)
                                {
                                    whoAmIArray[numProjectiles] = plr.whoAmI;
                                    targetCenterArray[numProjectiles] = playerCenter;
                                    int projectileLimit = numProjectiles + 1;
                                    numProjectiles = projectileLimit;
                                    if (projectileLimit >= targetCenterArray.Length)
                                        break;
                                }
                            }

                            float predictionAmt = targetDownDeep ? 90f : 120f;
                            float iceMistVelocityScale = death ? 1.8f : revenge ? 1.5f : expertMode ? 1.2f : 0f;
                            float iceMistVelocity = (targetDownDeep ? 12f : 18f) + (targetDownDeep ? iceMistVelocityScale : iceMistVelocityScale * 1.5f);
                            for (int i = 0; i < numProjectiles; i++)
                            {
                                // Predictive mist
                                Vector2 projectileDestination = targetCenterArray[i] + Main.player[whoAmIArray[i]].velocity * predictionAmt - NPC.Center;
                                Vector2 projectileVelocity = Vector2.Normalize(projectileDestination) * iceMistVelocity;
                                int type = ProjectileID.CultistBossIceMist;
                                int damage = NPC.GetProjectileDamage(type);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projectileVelocity, type, damage, 0f, Main.myPlayer, 0f, 1f);

                                // Opposite mist
                                projectileDestination = targetCenterArray[i] - Main.player[whoAmIArray[i]].velocity * predictionAmt - NPC.Center;
                                projectileVelocity = Vector2.Normalize(projectileDestination) * iceMistVelocity;
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projectileVelocity, type, damage, 0f, Main.myPlayer, 0f, 1f);

                                // Normal bolt
                                projectileDestination = targetCenterArray[i] - NPC.Center;
                                projectileVelocity = Vector2.Normalize(projectileDestination) * iceMistVelocity;
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projectileVelocity, type, damage, 0f, Main.myPlayer, 0f, 1f);
                            }
                        }

                        calamityGlobalNPC.newAI[2] += 1f;

                        Lighting.AddLight(NPC.Center, 0.3f, 0.75f, 0.9f);

                        float rotation = MathHelper.Clamp((float)Main.rand.NextDouble() * 1f - 0.5f, -0.5f, 0.5f);
                        Vector2 dustPosition = new Vector2(-NPC.width * 0.2f * NPC.scale, 0f).RotatedBy(rotation * ((float)Math.PI * 2f)).RotatedBy(NPC.velocity.ToRotation());
                        int dust = Dust.NewDust(NPC.Center - Vector2.One * 5f, 10, 10, DustID.NorthPole, 0f, 0f, 100, Color.Transparent);
                        Main.dust[dust].position = NPC.Center + dustPosition;
                        Main.dust[dust].velocity = Vector2.Normalize(Main.dust[dust].position - NPC.Center) * 2f;
                        Main.dust[dust].noGravity = true;

                        rotation = MathHelper.Clamp((float)Main.rand.NextDouble() * 1f - 0.5f, -0.5f, 0.5f);
                        dustPosition = new Vector2(-NPC.width * 0.6f * NPC.scale, 0f).RotatedBy(rotation * ((float)Math.PI * 2f)).RotatedBy(NPC.velocity.ToRotation());
                        dust = Dust.NewDust(NPC.Center - Vector2.One * 5f, 10, 10, DustID.NorthPole, 0f, 0f, 100, Color.Transparent);
                        Main.dust[dust].velocity = Vector2.Zero;
                        Main.dust[dust].position = NPC.Center + dustPosition;
                        Main.dust[dust].noGravity = true;

                        if (calamityGlobalNPC.newAI[2] >= iceMistDuration)
                        {
                            NPC.localAI[0] = 1f;
                            AIState = (float)Phase.FastCharge;
                            calamityGlobalNPC.newAI[2] = fastChargeGateValue;
                            chargeVelocityScalar = 0f;
                            FinalPhaseCheck();
                            NPC.TargetClosest();
                        }
                    }

                    break;

                // Phase 2 attack - Get in position for spin, spin around target and summon shadow fireballs
                case (int)Phase.ShadowFireballSpin:

                    // Swim up
                    destination += spinLocation;

                    // Use a lerp to smoothly scale up velocity and turn speed
                    chargeVelocityScalar += chargeVelocityScalarIncrement;
                    if (chargeVelocityScalar > 1f)
                        chargeVelocityScalar = 1f;

                    baseVelocity *= invisiblePhaseVelocityMult;
                    turnSpeed *= invisiblePhaseTurnSpeedMult;

                    // Spin around target
                    if ((destination - NPC.Center).Length() < spinLocationDistance || calamityGlobalNPC.newAI[2] > 0f)
                    {
                        calamityGlobalNPC.newAI[2] += 1f;

                        float spinVelocityDivisor = targetDownDeep ? 120f : 90f;
                        if (rotationDirection == 0)
                        {
                            // Set spin direction
                            if (Main.player[NPC.target].velocity.X > 0f)
                                rotationDirection = 1;
                            else if (Main.player[NPC.target].velocity.X < 0f)
                                rotationDirection = -1;
                            else
                                rotationDirection = player.direction;

                            // Set spin velocity
                            NPC.velocity.X = MathHelper.Pi * spinRadius / spinVelocityDivisor;
                            NPC.velocity *= -rotationDirection;
                            NPC.netUpdate = true;
                        }
                        else
                        {
                            NPC.velocity = NPC.velocity.RotatedBy(MathHelper.Pi / spinVelocityDivisor * -rotationDirection);
                            if (calamityGlobalNPC.newAI[2] >= spinPhaseDuration)
                            {
                                rotationDirection = 0;
                                AIState = phase4 ? (float)Phase.LightningCharge : (float)Phase.ChargeOne;
                                calamityGlobalNPC.newAI[2] = 0f;
                                chargeVelocityScalar = 0f;
                                FinalPhaseCheck();
                                NPC.TargetClosest();
                            }
                        }

                        // Return to prevent other velocity code from being called
                        return;
                    }

                    break;

                // Phase 3 attack - Swim above, turn invisible and summon ancient dooms around the target
                case (int)Phase.AncientDoomSummon:

                    // Swim up
                    destination += ancientDoomLocation;

                    if (NPC.localAI[1] < maxAncientDoomRings)
                    {
                        calamityGlobalNPC.newAI[2] += 1f;
                        if (calamityGlobalNPC.newAI[2] >= ancientDoomPhaseGateValue)
                        {
                            float aiGateValue = calamityGlobalNPC.newAI[2] - ancientDoomPhaseGateValue;
                            if (aiGateValue % ancientDoomGateValue == 0f)
                            {
                                // Spawn 3 (or more) circles of Ancient Dooms around the target
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int ancientDoomScale2 = (int)(aiGateValue / ancientDoomGateValue);
                                    ancientDoomLimit += ancientDoomScale2;
                                    ancientDoomDistance += ancientDoomScale2 * 45;
                                    int ancientDoomDegrees = 360 / ancientDoomLimit;
                                    for (int i = 0; i < ancientDoomLimit; i++)
                                    {
                                        float ai2 = i * ancientDoomDegrees;
                                        NPC.NewNPC(NPC.GetSource_FromAI(), (int)(player.Center.X + (float)(Math.Sin(i * ancientDoomDegrees) * ancientDoomDistance)), (int)(player.Center.Y + (float)(Math.Cos(i * ancientDoomDegrees) * ancientDoomDistance)),
                                            NPCID.AncientDoom, 0, NPC.whoAmI, 0f, ai2, 0f, Main.maxPlayers);
                                    }
                                }
                                NPC.localAI[1] += 1f;
                                NPC.TargetClosest();
                            }
                        }
                    }

                    if (NPC.localAI[1] >= maxAncientDoomRings)
                    {
                        NPC.localAI[1] += 1f;
                        if (NPC.localAI[1] >= ancientDoomGateValue + maxAncientDoomRings)
                        {
                            NPC.localAI[1] = 0f;
                            AIState = phase4 ? (float)Phase.LightningCharge : (float)Phase.ChargeTwo;
                            calamityGlobalNPC.newAI[2] = 0f;
                            FinalPhaseCheck();
                            NPC.TargetClosest();
                        }
                    }

                    break;

                // Phase 4 attack - Swim to the right or left and charge towards the target, summon a wall of lightning bolts in the direction of the charge
                case (int)Phase.LightningCharge:

                    if (calamityGlobalNPC.newAI[2] >= lightningChargePhaseGateValue)
                    {
                        ChargeDust(7, (float)Math.PI);

                        // Use a lerp to smoothly scale up velocity and turn speed
                        chargeVelocityScalar += chargeVelocityScalarIncrement;
                        if (chargeVelocityScalar > 1f)
                            chargeVelocityScalar = 1f;

                        baseVelocity *= normalChargeVelocityMult;
                        turnSpeed *= normalChargeTurnSpeedMult;

                        if ((lightningChargeLocation - NPC.Center).Length() < lightningChargeLocationDistance || calamityGlobalNPC.newAI[2] > lightningChargePhaseGateValue)
                        {
                            // Set the scalar to max
                            if (chargeVelocityScalar < 1f)
                                chargeVelocityScalar = 1f;

                            // Lock into looking at the target
                            if (calamityGlobalNPC.newAI[2] < lightningChargePhaseGateValue + 1f)
                                calamityGlobalNPC.newAI[2] += 1f;

                            // Turn towards the target
                            if (!lookingAtTarget && calamityGlobalNPC.newAI[2] < lightningChargePhaseGateValue + 2f)
                            {
                                baseVelocity /= normalChargeVelocityMult;
                                turnSpeed /= normalChargeTurnSpeedMult;
                            }

                            // Charge
                            else
                            {
                                if (calamityGlobalNPC.newAI[2] == lightningChargePhaseGateValue + 1f)
                                {
                                    if (Main.player[Main.myPlayer].active && !Main.player[Main.myPlayer].dead && Vector2.Distance(Main.player[Main.myPlayer].Center, NPC.Center) < soundDistance)
                                        SoundEngine.PlaySound(ChargeSound, Main.player[Main.myPlayer].Center);
                                }

                                // Lock into the charge phase and use this for a charge time check
                                calamityGlobalNPC.newAI[2] += 1f;

                                // Become totally visible
                                NPC.Opacity = 1f;

                                // Lightning barrage
                                if (NPC.localAI[3] == 0f)
                                {
                                    if (Main.player[Main.myPlayer].active && !Main.player[Main.myPlayer].dead && Vector2.Distance(Main.player[Main.myPlayer].Center, NPC.Center) < soundDistance)
                                        SoundEngine.PlaySound(CommonCalamitySounds.LightningSound, Main.player[Main.myPlayer].Center);

                                    NPC.localAI[3] = 1f;
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        int type = ProjectileID.CultistBossLightningOrbArc;
                                        int damage = NPC.GetProjectileDamage(type);
                                        for (int i = 0; i < numLightningBolts; i++)
                                        {
                                            Vector2 projectileDestination = player.Center - lightningSpawnLocation;
                                            Vector2 projectileVelocity = Vector2.Normalize(projectileDestination.RotatedByRandom(MathHelper.PiOver4)) * baseVelocity * 0.5f;
                                            float ai = Main.rand.Next(100);
                                            int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), lightningSpawnLocation, projectileVelocity, type, damage, 0f, Main.myPlayer, projectileDestination.ToRotation(), ai);
                                            Main.projectile[proj].tileCollide = false;
                                            lightningSpawnLocation.Y += distanceBetweenBolts;
                                            if (i == numLightningBolts / 2)
                                                lightningSpawnLocation.Y += distanceBetweenBolts;
                                        }
                                    }
                                }

                                float totalChargeTime = (totalChargeDistance / baseVelocity) + lightningChargePhaseGateValue + 1f;
                                if (calamityGlobalNPC.newAI[2] > totalChargeTime)
                                {
                                    AIState = NPC.localAI[2] == 0f ? (float)Phase.LightningRain : (float)Phase.IceMist;
                                    calamityGlobalNPC.newAI[2] = 0f;

                                    NPC.localAI[2] += 1f;
                                    if (NPC.localAI[2] > 1f)
                                        NPC.localAI[2] = 0f;

                                    NPC.localAI[3] = 0f;
                                    chargeVelocityScalar = 0f;
                                    FinalPhaseCheck();
                                    NPC.TargetClosest();
                                }
                            }
                        }
                        else
                            destination += lightningChargeVector;
                    }
                    else
                        calamityGlobalNPC.newAI[2] += 1f;

                    break;

                // Summon Eidolists and swim below the target for 3 seconds, or less, if the Eidolists die
                case (int)Phase.EidolistSpawn:

                    destination += eidolonWyrmPhaseLocation;

                    if (calamityGlobalNPC.newAI[2] == 0f)
                    {
                        if (!NPC.AnyNPCs(ModContent.NPCType<Eidolist>()))
                        {
                            if (Main.player[Main.myPlayer].active && !Main.player[Main.myPlayer].dead && Vector2.Distance(Main.player[Main.myPlayer].Center, NPC.Center) < soundDistance)
                                SoundEngine.PlaySound(Eidolist.DeathSound, Main.player[Main.myPlayer].Center);

                            // Spawn Eidolists randomly around the target
                            for (int i = 0; i < maxEidolists; i++)
                            {
                                Point npcCenter = NPC.Center.ToTileCoordinates();
                                Point playerCenter = player.Center.ToTileCoordinates();
                                Vector2 distance = player.Center - NPC.Center;

                                int baseSpawnDistance = 60;
                                int minDistance = 3;
                                int maxDistance = 7;
                                int collisionRange = 2;
                                int iterations = 0;
                                bool tooFar = distance.Length() > 2800f;
                                while (!tooFar && iterations < 100)
                                {
                                    iterations++;
                                    int randomX = Main.rand.Next(playerCenter.X - baseSpawnDistance, playerCenter.X + baseSpawnDistance + 1);
                                    int randomY = Main.rand.Next(playerCenter.Y - baseSpawnDistance, playerCenter.Y + baseSpawnDistance + 1);
                                    if ((randomY < playerCenter.Y - maxDistance || randomY > playerCenter.Y + maxDistance || randomX < playerCenter.X - maxDistance || randomX > playerCenter.X + maxDistance) && (randomY < npcCenter.Y - minDistance || randomY > npcCenter.Y + minDistance || randomX < npcCenter.X - minDistance || randomX > npcCenter.X + minDistance) && !Main.tile[randomX, randomY].HasUnactuatedTile)
                                    {
                                        bool canSpawn = true;
                                        if (canSpawn && Collision.SolidTiles(randomX - collisionRange, randomX + collisionRange, randomY - collisionRange, randomY + collisionRange))
                                            canSpawn = false;

                                        if (canSpawn)
                                        {
                                            NPC.NewNPC(NPC.GetSource_FromAI(), randomX * 16 + 8, randomY * 16 + 8, ModContent.NPCType<Eidolist>());
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    calamityGlobalNPC.newAI[2] += 1f;
                    if (calamityGlobalNPC.newAI[2] >= eidolonWyrmPhaseDuration)
                    {
                        AIState = (float)Phase.ChargeOne;
                        calamityGlobalNPC.newAI[2] = 0f;
                        FinalPhaseCheck();
                        NPC.TargetClosest();
                    }

                    break;

                // Spin around target, summon Ancient Dooms and shoot Shadow Fireballs from body segments
                case (int)Phase.FinalPhase:

                    // Ancient Dooms
                    calamityGlobalNPC.newAI[2] += 1f;
                    if (calamityGlobalNPC.newAI[2] >= ancientDoomPhaseGateValue)
                    {
                        float aiGateValue = calamityGlobalNPC.newAI[2] - ancientDoomPhaseGateValue;
                        if (aiGateValue % ancientDoomGateValue == 0f)
                        {
                            // Spawn 3 (or more) circles of Ancient Dooms around the target
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int ancientDoomScale2 = (int)(aiGateValue / ancientDoomGateValue);
                                ancientDoomLimit += ancientDoomScale2;
                                ancientDoomDistance += ancientDoomScale2 * 45;
                                int ancientDoomDegrees = 360 / ancientDoomLimit;
                                for (int i = 0; i < ancientDoomLimit; i++)
                                {
                                    float ai2 = i * ancientDoomDegrees;
                                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)(player.Center.X + (float)(Math.Sin(i * ancientDoomDegrees) * ancientDoomDistance)), (int)(player.Center.Y + (float)(Math.Cos(i * ancientDoomDegrees) * ancientDoomDistance)),
                                        NPCID.AncientDoom, 0, NPC.whoAmI, 0f, ai2, 0f, Main.maxPlayers);
                                }
                            }

                            if (calamityGlobalNPC.newAI[2] >= 240f)
                                calamityGlobalNPC.newAI[2] = -90f;

                            NPC.TargetClosest();
                        }
                    }

                    // Swim up
                    destination += spinLocation;

                    // Use a lerp to smoothly scale up velocity and turn speed
                    chargeVelocityScalar += chargeVelocityScalarIncrement;
                    if (chargeVelocityScalar > 1f)
                        chargeVelocityScalar = 1f;

                    baseVelocity *= invisiblePhaseVelocityMult;
                    turnSpeed *= invisiblePhaseTurnSpeedMult;

                    // Spin around target
                    if ((destination - NPC.Center).Length() < spinLocationDistance || calamityGlobalNPC.newAI[1] > 0f)
                    {
                        calamityGlobalNPC.newAI[1] += 1f;
                        float spinVelocityDivisor = targetDownDeep ? 90f : 60f;
                        if (rotationDirection == 0)
                        {
                            // Set spin direction
                            if (Main.player[NPC.target].velocity.X > 0f)
                                rotationDirection = 1;
                            else if (Main.player[NPC.target].velocity.X < 0f)
                                rotationDirection = -1;
                            else
                                rotationDirection = player.direction;

                            // Set spin velocity
                            NPC.velocity.X = MathHelper.Pi * spinRadius / spinVelocityDivisor;
                            NPC.velocity *= -rotationDirection;
                            NPC.netUpdate = true;
                        }
                        else
                        {
                            NPC.velocity = NPC.velocity.RotatedBy(MathHelper.Pi / spinVelocityDivisor * -rotationDirection);

                            // Swim above target again if enough time has passed
                            if (calamityGlobalNPC.newAI[1] >= spinPhaseDuration)
                            {
                                calamityGlobalNPC.newAI[1] = 0f;

                                chargeVelocityScalar = 0f;
                                rotationDirection = 0;

                                NPC.TargetClosest();
                            }
                        }

                        // Return to prevent other velocity code from being called
                        return;
                    }

                    break;
            }

            void FinalPhaseCheck()
            {
                if (phase6)
                {
                    NPC.localAI[1] = 0f;
                    AIState = (float)Phase.FinalPhase;
                    calamityGlobalNPC.newAI[1] = 0f;
                    calamityGlobalNPC.newAI[2] = 0f;
                    rotationDirection = 0;
                }
            }

            if (!targetDead)
            {
                // Ensure that speed stays within a specific range.
                NPC.velocity = NPC.velocity.ClampMagnitude(baseVelocity * 0.2f, baseVelocity * 1.3f);
                Vector2 idealVelocity = NPC.SafeDirectionTo(destination) * baseVelocity;

                if ((NPC.velocity.X > 0f && idealVelocity.X > 0f) || (NPC.velocity.X < 0f && idealVelocity.X < 0f) || (NPC.velocity.Y > 0f && idealVelocity.Y > 0f) || (NPC.velocity.Y < 0f && idealVelocity.Y < 0f))
                {
                    // Accelerate towards the ideal velocity.
                    NPC.velocity.X += (NPC.velocity.X < idealVelocity.X).ToDirectionInt() * turnSpeed;
                    NPC.velocity.Y += (NPC.velocity.Y < idealVelocity.Y).ToDirectionInt() * turnSpeed;

                    // Swim more quickly towards the ideal velocity if there isn't much speed currently or if the velocity goes against the ideal velocity.
                    if (Math.Abs(idealVelocity.Y) < baseVelocity * 0.2 && ((NPC.velocity.X > 0f && idealVelocity.X < 0f) || (NPC.velocity.X < 0f && idealVelocity.X > 0f)))
                        NPC.velocity.Y += NPC.velocity.Y.DirectionalSign() * turnSpeed * 2f;

                    if (Math.Abs(idealVelocity.X) < baseVelocity * 0.2 && ((NPC.velocity.Y > 0f && idealVelocity.Y < 0f) || (NPC.velocity.Y < 0f && idealVelocity.Y > 0f)))
                        NPC.velocity.X += NPC.velocity.X.DirectionalSign() * turnSpeed * 2f;
                }

                // Choose whichever axis the Wyrm is closest to it's destination on and emphasize moving in that direction.
                else if (MathHelper.Distance(destination.X, NPC.Center.X) > MathHelper.Distance(destination.Y, NPC.Center.Y))
                {
                    NPC.velocity.X += (NPC.velocity.X < idealVelocity.X).ToDirectionInt() * turnSpeed * 1.1f;
                    if (NPC.velocity.ManhattanDistance(Vector2.Zero) < baseVelocity * 0.5)
                        NPC.velocity.Y += NPC.velocity.Y.DirectionalSign() * turnSpeed;
                }
                else
                {
                    NPC.velocity.Y += (NPC.velocity.Y < idealVelocity.Y).ToDirectionInt() * turnSpeed * 1.1f;
                    if (NPC.velocity.ManhattanDistance(Vector2.Zero) < baseVelocity * 0.5)
                        NPC.velocity.X += NPC.velocity.X.DirectionalSign() * turnSpeed;
                }
            }
        }

        private void ChargeDust(int dustAmt, float pie)
        {
            for (int num1474 = 0; num1474 < dustAmt; num1474++)
            {
                Vector2 dustRotation = Vector2.Normalize(NPC.velocity) * new Vector2((NPC.width + 50) / 2f, NPC.height) * 0.75f;
                dustRotation = dustRotation.RotatedBy((num1474 - (dustAmt / 2 - 1)) * (double)pie / (float)dustAmt) + NPC.Center;
                Vector2 dustVelocity = ((float)(Main.rand.NextDouble() * pie) - MathHelper.PiOver2).ToRotationVector2() * Main.rand.Next(3, 8);
                int chargeDust = Dust.NewDust(dustRotation + dustVelocity, 0, 0, DustID.DungeonWater, dustVelocity.X * 2f, dustVelocity.Y * 2f, 100, default, 1.4f);
                Main.dust[chargeDust].noGravity = true;
                Main.dust[chargeDust].noLight = true;
                Main.dust[chargeDust].velocity /= 4f;
                Main.dust[chargeDust].velocity -= NPC.velocity;
            }
        }

        public static bool CanMinionsDropThings()
        {
            bool adultWyrmAlive = false;
            if (CalamityGlobalNPC.adultEidolonWyrmHead != -1)
            {
                if (Main.npc[CalamityGlobalNPC.adultEidolonWyrmHead].active)
                    adultWyrmAlive = true;
            }
            return !adultWyrmAlive;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses;

            Rectangle targetHitbox = target.Hitbox;

            float hitboxTopLeft = Vector2.Distance(NPC.Center, targetHitbox.TopLeft());
            float hitboxTopRight = Vector2.Distance(NPC.Center, targetHitbox.TopRight());
            float hitboxBotLeft = Vector2.Distance(NPC.Center, targetHitbox.BottomLeft());
            float hitboxBotRight = Vector2.Distance(NPC.Center, targetHitbox.BottomRight());

            float minDist = hitboxTopLeft;
            if (hitboxTopRight < minDist)
                minDist = hitboxTopRight;
            if (hitboxBotLeft < minDist)
                minDist = hitboxBotLeft;
            if (hitboxBotRight < minDist)
                minDist = hitboxBotRight;

            return minDist <= 70f && NPC.Opacity == 1f;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.5f;
            return null;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                NPC.Opacity = 1f;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Vector2 vector = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2);

            Vector2 center = NPC.Center - screenPos;
            center -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
            center += vector * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture, center, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, vector, NPC.scale, spriteEffects, 0f);

            texture = GlowTexture.Value;
            spriteBatch.Draw(texture, center, NPC.frame, Color.White * NPC.Opacity, NPC.rotation, vector, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<OmegaHealingPotion>();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<EidolicWail>());
            npcLoot.Add(ModContent.ItemType<VoidEdge>());
            npcLoot.Add(ModContent.ItemType<HalibutCannon>());
            npcLoot.Add(ModContent.ItemType<AbyssShellFossil>());
            npcLoot.Add(ModContent.ItemType<Voidstone>(), 1, 80, 100);
            npcLoot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

            var postLevi = npcLoot.DefineConditionalDropSet(() => DownedBossSystem.downedLeviathan);
            postLevi.Add(DropHelper.NormalVsExpertQuantity(ModContent.ItemType<Lumenyl>(), 1, 50, 108, 65, 135));
            postLevi.Add(ItemID.Ectoplasm, 1, 21, 32);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            // Create gore and dust hit effects.
            if (Main.netMode == NetmodeID.Server)
                return;

            for (int k = 0; k < 15; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TintableDust, hit.HitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TintableDust, hit.HitDirection, -1f, 0, default, 1f);

                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("PrimordialWyrm").Type, 1f);
            }
        }

        public override bool CheckActive() => false;

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }

        public override void ModifyTypeName(ref string typeName)
        {
            if (Main.zenithWorld)
            {
                typeName = CalamityUtils.GetTextValue("NPCs.Jared");
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (NPC.Opacity == 1f && hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<CrushDepth>(), 1200, true);
        }
    }
}
