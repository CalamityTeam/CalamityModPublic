using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Sounds;

namespace CalamityMod.NPCs.AdultEidolonWyrm
{
    [AutoloadBossHead]
    public class AdultEidolonWyrmHead : ModNPC
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

        // Base distance from target location in order to continue turning
        private const float baseTurnDistance = 160f;

        // The distance from target location in order to initiate an attack
        private const float baseAttackTriggerDistance = 80f;

        // Max distance from the target before they are unable to hear sound telegraphs
        private const float soundDistance = 2800f;

        // The end location of each charge
        Vector2 chargeDestination = default;

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
        public static readonly SoundStyle RoarSound = new("CalamityMod/Sounds/Custom/EidolonWyrmRoarClose");

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Adult Eidolon Wyrm");
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Scale = 0.50f,
                PortraitScale = 0.6f,
                PortraitPositionXOverride = 40,
                CustomTexturePath = "CalamityMod/ExtraTextures/Bestiary/AdultEidolonWyrm_Bestiary"
            };
            value.Position.X += 55;
            value.Position.Y += 5;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.npcSlots = 50f;
            NPC.GetNPCDamage();
            NPC.width = 254;
            NPC.height = 138;
            NPC.defense = 100;
            NPC.DR_NERD(0.4f);
            NPC.LifeMaxNERB(2415000, 2898000);
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
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.netAlways = true;
            NPC.boss = true;
            Music = CalamityMod.Instance.GetMusicFromMusicMod("AdultEidolonWyrm") ?? MusicID.Boss3;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                //AAAAAAAAAAAAH Scary abyss superboss guy so he gets pitch black bg and no biome source
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement(),

				// Will move to localization whenever that is cleaned up.
				new FlavorTextBestiaryInfoElement("Traces of them appear even in records going back to before the Golden Age of Dragons… They may very well be a glimpse into the full potential of nature.")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(chargeDestination);
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
            chargeDestination = reader.ReadVector2();
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

            bool targetDownDeep = player.Calamity().ZoneAbyssLayer4;
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
                                lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<AdultEidolonWyrmBody>(), NPC.whoAmI);
                            else
                                lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<AdultEidolonWyrmBodyAlt>(), NPC.whoAmI);
                        }
                        else
                            lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<AdultEidolonWyrmTail>(), NPC.whoAmI);

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
                            if (Main.npc[a].type == NPC.type || Main.npc[a].type == ModContent.NPCType<AdultEidolonWyrmBodyAlt>() || Main.npc[a].type == ModContent.NPCType<AdultEidolonWyrmBody>() || Main.npc[a].type == ModContent.NPCType<AdultEidolonWyrmTail>())
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
            NPC.buffImmune[ModContent.BuffType<ExoFreeze>()] = immuneToSlowingDebuffs;
            NPC.buffImmune[ModContent.BuffType<GlacialState>()] = immuneToSlowingDebuffs;
            NPC.buffImmune[ModContent.BuffType<TemporalSadness>()] = immuneToSlowingDebuffs;
            NPC.buffImmune[ModContent.BuffType<KamiFlu>()] = immuneToSlowingDebuffs;
            NPC.buffImmune[ModContent.BuffType<Eutrophication>()] = immuneToSlowingDebuffs;
            NPC.buffImmune[ModContent.BuffType<TimeDistortion>()] = immuneToSlowingDebuffs;
            NPC.buffImmune[ModContent.BuffType<TeslaFreeze>()] = immuneToSlowingDebuffs;
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

            // Rotation and direction
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
            float turnDistance = baseTurnDistance;
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
            Vector2 chargeVectorFlipped = chargeVector * -1f;
            float chargePredictionAmt = death ? 50f : revenge ? 40f : expertMode ? 30f : 20f;

            // Lightning Rain variables
            Vector2 lightningRainLocation = new Vector2(0f, -baseDistance);
            float lightningRainLocationDistance = baseAttackTriggerDistance;

            // Wyrm and Eidolist variables
            Vector2 eidolonWyrmPhaseLocation = new Vector2(0f, baseDistance);
            float eidolonWyrmPhaseLocationDistance = baseAttackTriggerDistance;
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
            float ancientDoomLocationDistance = baseAttackTriggerDistance;
            int ancientDoomScale = death ? 3 : revenge ? 2 : expertMode ? 1 : 0;
            int ancientDoomLimit = (targetDownDeep ? 4 : 8) + ancientDoomScale;
            int ancientDoomDistance = death ? 520 : revenge ? 535 : expertMode ? 550 : 600;
            float maxAncientDoomRings = 3f;

            // Lightning charge variables
            Vector2 lightningChargeVector = NPC.localAI[2] == 0f ? new Vector2(baseDistance, 0f) : new Vector2(-baseDistance, 0f);
            float lightningChargeLocationDistance = baseAttackTriggerDistance;
            Vector2 lightningChargeLocation = destination + lightningChargeVector;
            Vector2 lightningChargeVectorFlipped = lightningChargeVector * -1f;
            float lightningSpawnY = 540f;
            Vector2 lightningSpawnLocation = new Vector2(NPC.Center.X, NPC.Center.Y - lightningSpawnY);
            int numLightningBolts = death ? 10 : revenge ? 8 : expertMode ? 6 : 4;
            float distanceBetweenBolts = lightningSpawnY * 2f / numLightningBolts;

            // Velocity and turn speed values
            float velocityScale = death ? 1.8f : revenge ? 1.5f : expertMode ? 1.2f : 0f;
            float baseVelocity = (targetDownDeep ? 12f : 24f) + (targetDownDeep ? velocityScale : velocityScale * 2f);
            if (Main.getGoodWorld)
                baseVelocity *= 1.15f;

            float turnSpeed = baseVelocity * 0.125f;

            float normalChargeVelocityMult = MathHelper.Lerp(1f, 2f, chargeVelocityScalar);
            float normalChargeTurnSpeedMult = MathHelper.Lerp(1f, 8f, chargeVelocityScalar);
            float invisiblePhaseVelocityMult = MathHelper.Lerp(1f, 1.5f, chargeVelocityScalar);
            float invisiblePhaseTurnSpeedMult = MathHelper.Lerp(1f, 6f, chargeVelocityScalar);
            float fastChargeVelocityMult = MathHelper.Lerp(1f, 3f, chargeVelocityScalar);
            float fastChargeTurnSpeedMult = MathHelper.Lerp(1f, 12f, chargeVelocityScalar);
            float chargeVelocityScalarIncrement = 0.025f;

            // Telekinesis while enraged
            if (!targetDownDeep)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    calamityGlobalNPC.newAI[3] += 1f;
                    if (calamityGlobalNPC.newAI[3] >= 300f)
                    {
                        calamityGlobalNPC.newAI[3] = 0f;

                        SoundEngine.PlaySound(SoundID.Item117, player.position);

                        for (int i = 0; i < 20; i++)
                        {
                            int dust = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 185, 0f, 0f, 100, default, 2f);
                            Main.dust[dust].velocity *= 0.6f;
                            if (Main.rand.NextBool(2))
                            {
                                Main.dust[dust].scale = 0.5f;
                                Main.dust[dust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                            }
                        }

                        for (int j = 0; j < 30; j++)
                        {
                            int dust = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 185, 0f, 0f, 100, default, 3f);
                            Main.dust[dust].noGravity = true;
                            dust = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 185, 0f, 0f, 100, default, 2f);
                            Main.dust[dust].velocity *= 0.2f;
                        }

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
                        turnDistance = chargeLocationDistance;

                        if ((chargeLocation - NPC.Center).Length() < chargeLocationDistance || calamityGlobalNPC.newAI[2] > chargePhaseGateValue)
                        {
                            calamityGlobalNPC.newAI[2] += 1f;
                            if (calamityGlobalNPC.newAI[2] == chargePhaseGateValue + 1f)
                            {
                                if (Main.player[Main.myPlayer].active && !Main.player[Main.myPlayer].dead && Vector2.Distance(Main.player[Main.myPlayer].Center, NPC.Center) < soundDistance)
                                    SoundEngine.PlaySound(RoarSound, Main.player[Main.myPlayer].Center);

                                chargeDestination = destination + chargeVectorFlipped + player.velocity * chargePredictionAmt;
                                NPC.velocity = Vector2.Normalize(chargeDestination - NPC.Center) * baseVelocity;
                                NPC.netUpdate = true;
                                NPC.netSpam -= 5;
                            }
                            else
                            {
                                // Charge

                                // Become totally visible
                                NPC.Opacity = 1f;

                                destination = chargeDestination;

                                if ((destination - NPC.Center).Length() < chargeLocationDistance)
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
                    turnDistance = lightningRainLocationDistance;

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

                                for (int i = 0; i < Main.maxPlayers; i++)
                                {
                                    if (!Main.player[i].active || Main.player[i].dead)
                                        continue;

                                    Vector2 playerCenter = Main.player[i].Center;
                                    float distance = Vector2.Distance(playerCenter, NPC.Center);
                                    if (distance < maxDistance)
                                    {
                                        whoAmIArray[numProjectiles] = i;
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
                        int dust = Dust.NewDust(NPC.Center - Vector2.One * 5f, 10, 10, 226, (0f - NPC.velocity.X) / 3f, (0f - NPC.velocity.Y) / 3f, 150, Color.Transparent, 0.7f);
                        Main.dust[dust].position = NPC.Center + dustPosition;
                        Main.dust[dust].velocity = Vector2.Normalize(Main.dust[dust].position - NPC.Center) * 2f;
                        Main.dust[dust].noGravity = true;

                        rotation = MathHelper.Clamp((float)Main.rand.NextDouble() * 1f - 0.5f, -0.5f, 0.5f);
                        dustPosition = new Vector2(-NPC.width * 0.6f * NPC.scale, 0f).RotatedBy(rotation * ((float)Math.PI * 2f)).RotatedBy(NPC.velocity.ToRotation());
                        dust = Dust.NewDust(NPC.Center - Vector2.One * 5f, 10, 10, 226, (0f - NPC.velocity.X) / 3f, (0f - NPC.velocity.Y) / 3f, 150, Color.Transparent, 0.7f);
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
                        turnDistance = chargeLocationDistance;

                        if ((chargeLocation - NPC.Center).Length() < chargeLocationDistance || calamityGlobalNPC.newAI[2] > chargePhaseGateValue)
                        {
                            calamityGlobalNPC.newAI[2] += 1f;
                            if (calamityGlobalNPC.newAI[2] == chargePhaseGateValue + 1f)
                            {
                                if (Main.player[Main.myPlayer].active && !Main.player[Main.myPlayer].dead && Vector2.Distance(Main.player[Main.myPlayer].Center, NPC.Center) < soundDistance)
                                    SoundEngine.PlaySound(RoarSound, Main.player[Main.myPlayer].Center);

                                chargeDestination = destination + chargeVectorFlipped + player.velocity * chargePredictionAmt;
                                NPC.velocity = Vector2.Normalize(chargeDestination - NPC.Center) * baseVelocity;
                                NPC.netUpdate = true;
                                NPC.netSpam -= 5;
                            }
                            else
                            {
                                // Charge very quickly

                                // Become totally visible
                                NPC.Opacity = 1f;

                                destination = chargeDestination;

                                if ((destination - NPC.Center).Length() < chargeLocationDistance)
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
                    turnDistance = eidolonWyrmPhaseLocationDistance;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (calamityGlobalNPC.newAI[2] == 0f)
                        {
                            if (!NPC.AnyNPCs(ModContent.NPCType<EidolonWyrmHead>()))
                            {
                                if (Main.player[Main.myPlayer].active && !Main.player[Main.myPlayer].dead && Vector2.Distance(Main.player[Main.myPlayer].Center, NPC.Center) < soundDistance)
                                    SoundEngine.PlaySound(CommonCalamitySounds.WyrmScreamSound, Main.player[Main.myPlayer].Center);

                                NPC.SpawnOnPlayer(NPC.FindClosestPlayer(), ModContent.NPCType<EidolonWyrmHead>());
                            }
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
                        turnDistance = chargeLocationDistance;

                        if ((chargeLocation - NPC.Center).Length() < chargeLocationDistance || calamityGlobalNPC.newAI[2] > chargePhaseGateValue)
                        {
                            calamityGlobalNPC.newAI[2] += 1f;
                            if (calamityGlobalNPC.newAI[2] == chargePhaseGateValue + 1f)
                            {
                                if (Main.player[Main.myPlayer].active && !Main.player[Main.myPlayer].dead && Vector2.Distance(Main.player[Main.myPlayer].Center, NPC.Center) < soundDistance)
                                    SoundEngine.PlaySound(RoarSound, Main.player[Main.myPlayer].Center);

                                chargeDestination = destination + chargeVectorFlipped + player.velocity * chargePredictionAmt;
                                NPC.velocity = Vector2.Normalize(chargeDestination - NPC.Center) * baseVelocity;
                                NPC.netUpdate = true;
                                NPC.netSpam -= 5;
                            }
                            else
                            {
                                // Charge

                                // Become totally visible
                                NPC.Opacity = 1f;

                                destination = chargeDestination;

                                if ((destination - NPC.Center).Length() < chargeLocationDistance)
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
                    turnDistance = iceMistLocationDistance;

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

                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                if (!Main.player[i].active || Main.player[i].dead)
                                    continue;

                                Vector2 playerCenter = Main.player[i].Center;
                                float distance = Vector2.Distance(playerCenter, NPC.Center);
                                if (distance < maxDistance)
                                {
                                    whoAmIArray[numProjectiles] = i;
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
                        int dust = Dust.NewDust(NPC.Center - Vector2.One * 5f, 10, 10, 197, 0f, 0f, 100, Color.Transparent);
                        Main.dust[dust].position = NPC.Center + dustPosition;
                        Main.dust[dust].velocity = Vector2.Normalize(Main.dust[dust].position - NPC.Center) * 2f;
                        Main.dust[dust].noGravity = true;

                        rotation = MathHelper.Clamp((float)Main.rand.NextDouble() * 1f - 0.5f, -0.5f, 0.5f);
                        dustPosition = new Vector2(-NPC.width * 0.6f * NPC.scale, 0f).RotatedBy(rotation * ((float)Math.PI * 2f)).RotatedBy(NPC.velocity.ToRotation());
                        dust = Dust.NewDust(NPC.Center - Vector2.One * 5f, 10, 10, 197, 0f, 0f, 100, Color.Transparent);
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
                    turnDistance = spinLocationDistance;

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
                    turnDistance = ancientDoomLocationDistance;

                    if (NPC.localAI[1] < maxAncientDoomRings)
                    {
                        calamityGlobalNPC.newAI[2] += 1f;
                        if (calamityGlobalNPC.newAI[2] >= ancientDoomPhaseGateValue)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float aiGateValue = calamityGlobalNPC.newAI[2] - ancientDoomPhaseGateValue;
                                if (aiGateValue % ancientDoomGateValue == 0f)
                                {
                                    // Spawn 3 (or more) circles of Ancient Dooms around the target
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
                                    NPC.localAI[1] += 1f;
                                    NPC.TargetClosest();
                                }
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

                        baseVelocity *= fastChargeVelocityMult;
                        turnSpeed *= fastChargeTurnSpeedMult;
                        turnDistance = lightningChargeLocationDistance;

                        if ((lightningChargeLocation - NPC.Center).Length() < lightningChargeLocationDistance || calamityGlobalNPC.newAI[2] > lightningChargePhaseGateValue)
                        {
                            calamityGlobalNPC.newAI[2] += 1f;
                            if (calamityGlobalNPC.newAI[2] == lightningChargePhaseGateValue + 1f)
                            {
                                if (Main.player[Main.myPlayer].active && !Main.player[Main.myPlayer].dead && Vector2.Distance(Main.player[Main.myPlayer].Center, NPC.Center) < soundDistance)
                                    SoundEngine.PlaySound(RoarSound, Main.player[Main.myPlayer].Center);

                                chargeDestination = destination + lightningChargeVectorFlipped + player.velocity * chargePredictionAmt;
                                NPC.velocity = Vector2.Normalize(chargeDestination - NPC.Center) * baseVelocity;
                                NPC.netUpdate = true;
                                NPC.netSpam -= 5;
                            }
                            else
                            {
                                // Charge

                                // Become totally visible
                                NPC.Opacity = 1f;

                                destination = chargeDestination;

                                // Lightning barrage
                                if (Main.netMode != NetmodeID.MultiplayerClient && NPC.localAI[3] == 0f)
                                {
                                    if (Main.player[Main.myPlayer].active && !Main.player[Main.myPlayer].dead && Vector2.Distance(Main.player[Main.myPlayer].Center, NPC.Center) < soundDistance)
                                        SoundEngine.PlaySound(CommonCalamitySounds.LightningSound, Main.player[Main.myPlayer].Center);

                                    NPC.localAI[3] = 1f;
                                    int type = ProjectileID.CultistBossLightningOrbArc;
                                    int damage = NPC.GetProjectileDamage(type);
                                    for (int i = 0; i < numLightningBolts; i++)
                                    {
                                        Vector2 projectileDestination = player.Center - lightningSpawnLocation;
                                        float ai = Main.rand.Next(100);
                                        int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), lightningSpawnLocation, NPC.velocity * 0.5f, type, damage, 0f, Main.myPlayer, projectileDestination.ToRotation(), ai);
                                        Main.projectile[proj].tileCollide = false;
                                        lightningSpawnLocation.Y += distanceBetweenBolts;
                                        if (i == numLightningBolts / 2)
                                            lightningSpawnLocation.Y += distanceBetweenBolts;
                                    }
                                }

                                if ((destination - NPC.Center).Length() < lightningChargeLocationDistance)
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
                    turnDistance = eidolonWyrmPhaseLocationDistance;

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
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float aiGateValue = calamityGlobalNPC.newAI[2] - ancientDoomPhaseGateValue;
                            if (aiGateValue % ancientDoomGateValue == 0f)
                            {
                                // Spawn 3 (or more) circles of Ancient Dooms around the target
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

                                if (calamityGlobalNPC.newAI[2] >= 240f)
                                    calamityGlobalNPC.newAI[2] = -90f;

                                NPC.TargetClosest();
                            }
                        }
                    }

                    // Swim up
                    destination += spinLocation;
                    turnDistance = spinLocationDistance;

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
                // Increase velocity if velocity is ever zero
                if (NPC.velocity == Vector2.Zero)
                    NPC.velocity = Vector2.Normalize(player.Center - NPC.Center).SafeNormalize(Vector2.Zero) * baseVelocity;

                // Acceleration
                if (!((destination - NPC.Center).Length() < turnDistance))
                {
                    float targetAngle = NPC.AngleTo(destination);
                    float f = NPC.velocity.ToRotation().AngleTowards(targetAngle, turnSpeed);
                    NPC.velocity = f.ToRotationVector2() * baseVelocity;
                }
            }

            // Velocity upper limit
            if (NPC.velocity.Length() > baseVelocity)
                NPC.velocity = NPC.velocity.SafeNormalize(Vector2.Zero) * baseVelocity;
        }

        private void ChargeDust(int dustAmt, float pie)
        {
            for (int num1474 = 0; num1474 < dustAmt; num1474++)
            {
                Vector2 vector171 = Vector2.Normalize(NPC.velocity) * new Vector2((NPC.width + 50) / 2f, NPC.height) * 0.75f;
                vector171 = vector171.RotatedBy((num1474 - (dustAmt / 2 - 1)) * (double)pie / (float)dustAmt) + NPC.Center;
                Vector2 value18 = ((float)(Main.rand.NextDouble() * pie) - MathHelper.PiOver2).ToRotationVector2() * Main.rand.Next(3, 8);
                int num1475 = Dust.NewDust(vector171 + value18, 0, 0, 172, value18.X * 2f, value18.Y * 2f, 100, default, 1.4f);
                Main.dust[num1475].noGravity = true;
                Main.dust[num1475].noLight = true;
                Main.dust[num1475].velocity /= 4f;
                Main.dust[num1475].velocity -= NPC.velocity;
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
            cooldownSlot = 1;

            Rectangle targetHitbox = target.Hitbox;

            float dist1 = Vector2.Distance(NPC.Center, targetHitbox.TopLeft());
            float dist2 = Vector2.Distance(NPC.Center, targetHitbox.TopRight());
            float dist3 = Vector2.Distance(NPC.Center, targetHitbox.BottomLeft());
            float dist4 = Vector2.Distance(NPC.Center, targetHitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist)
                minDist = dist2;
            if (dist3 < minDist)
                minDist = dist3;
            if (dist4 < minDist)
                minDist = dist4;

            return minDist <= 70f && NPC.Opacity == 1f;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.5f;
            return null;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                NPC.Opacity = 1f;
                return;
            }

            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/AdultEidolonWyrm/AdultEidolonWyrmHeadGlow").Value;
            SpriteEffects spriteEffects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 center = NPC.Center;
            Main.spriteBatch.Draw(texture, center - screenPos, NPC.frame, Color.White, NPC.rotation, texture.Size() * 0.5f, NPC.scale, spriteEffects, 0f);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<OmegaHealingPotion>();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<EidolicWail>());
            npcLoot.Add(ModContent.ItemType<SoulEdge>());
            npcLoot.Add(ModContent.ItemType<HalibutCannon>());
            npcLoot.Add(ModContent.ItemType<Voidstone>(), 1, 80, 100);

            var postClone = npcLoot.DefineConditionalDropSet(() => DownedBossSystem.downedCalamitas);
            postClone.Add(DropHelper.NormalVsExpertQuantity(ModContent.ItemType<Lumenyl>(), 1, 50, 108, 65, 135));
            postClone.Add(ItemID.Ectoplasm, 1, 21, 32);
        }

        public override void OnKill()
        {
            // Mark Adult Eidolon Wyrm as defeated
            DownedBossSystem.downedAdultEidolonWyrm = true;
            CalamityNetcode.SyncWorld();
            CalamityGlobalNPC.SetNewBossJustDowned(NPC);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 4, hitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 4, hitDirection, -1f, 0, default, 1f);

                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("WyrmAdult").Type, 1f);
                }
            }
        }

        public override bool CheckActive() => false;

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            if (NPC.Opacity == 1f)
                player.AddBuff(ModContent.BuffType<CrushDepth>(), 600, true);
        }
    }
}
