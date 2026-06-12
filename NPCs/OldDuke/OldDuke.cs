using System;
using System.IO;
using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.Paintings;
using CalamityMod.Items.Placeables.Furniture.Monoliths;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Potions;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Systems;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.OldDuke
{
    [AutoloadBossHead]
    public class OldDuke : ModNPC
    {
        public static Color GlowColor = new Color(55, 255, 25, 0);

        // Old Duke doesn't have a phase 3 in normal mode
        public const float LifePercentagePhase2_Normal = 0.5f;
        public const float LifePercentagePhase2_Revenge = 0.7f;
        public const float LifePercentagePhase2_Death = 0.8f;
        public const float LifePercentagePhase3_Expert = 0.2f;
        public const float LifePercentagePhase3_Revenge = 0.35f;
        public const float LifePercentagePhase3_Death = 0.5f;

        public int Phase = 0;
        public float NuclearOverlayVisual = 0f;

        public static readonly SoundStyle HuffSound = new("CalamityMod/Sounds/Custom/OldDukeHuff");
        public static readonly SoundStyle RoarSound = new("CalamityMod/Sounds/Custom/OldDukeRoar");
        public static readonly SoundStyle VomitSound = new("CalamityMod/Sounds/Custom/OldDukeVomit");
        public static readonly SoundStyle VortexSpawnSound = new("CalamityMod/Sounds/Custom/OldDukeVortexSpawn");
        public static readonly SoundStyle DashSound = new("CalamityMod/Sounds/Custom/OldDukeDash");
        public static readonly SoundStyle DashSoundP3 = new("CalamityMod/Sounds/Custom/OldDukeDashP3");
        private static Color FireGreen = new Color(155, 255, 55);
        private SlotId RoarSoundSlot;

        public static Asset<Texture2D> GlowTexture;

        public float shake = 0f;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 7;
            NPCID.Sets.TrailingMode[Type] = 1;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                SpriteDirection = 1,
                Scale = 0.45f
            };
            value.Position.X += 14f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.AsyncLoad);
            }
        }

        public static float Phase2ContactDamageMult = 1.1f; // 308
        public static float Phase3ContactDamageMult = 1.2f; // 336
        public static int GoreDamage = 55; // 220
        public static int VortexDamage = 105; // 420

        // GFB exclusive
        public static int FartDamage = 80; // 320

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.damage = 140; // 280
            NPC.alpha = 255;
            NPC.width = 150;
            NPC.height = 100;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.defense = 90;
            NPC.DR_NERD(0.5f);
            NPC.LifeMaxNERB(400000, 600000, 400000);
            NPC.knockBackResist = 0f;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.npcSlots = 15f;
            NPC.HitSound = SoundID.NPCHit14;
            NPC.DeathSound = SoundID.NPCDeath20;
            NPC.value = Item.buyPrice(platinum: 1, gold: 50);
            NPC.boss = true;
            NPC.netAlways = true;
            NPC.timeLeft = NPC.activeTime * 30;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<SulphurousSeaBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.OldDuke")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.rotation);
            writer.Write(NPC.spriteDirection);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.rotation = reader.ReadSingle();
            NPC.spriteDirection = reader.ReadInt32();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        #region Visual Helper Methods
        static void DoChargeBurst(NPC npc, int phase)
        {
            SoundEngine.PlaySound(phase > 1 ? DashSoundP3 : DashSound, npc.Center);

            if (phase > 1)
            {
                for (int i = 0; i < 30; i++)
                {
                    float fl = Main.rand.NextFloat(-60f, 60f);
                    GeneralParticleHandler.SpawnParticle(new SparkParticle(npc.Center + (Vector2.Zero.DirectionTo(npc.velocity) * 20f) + new Vector2(0f, fl).RotatedBy(Vector2.Zero.AngleTo(npc.velocity)), (-npc.velocity * Main.rand.NextFloat(2f)).RotatedBy(MathHelper.ToRadians(fl)), false, 25, Main.rand.NextFloat(0.3f, 1.2f), FireGreen));
                }

                CalamityUtils.AddScreenshakeAt(npc.Center, 10f, 3000f);

                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, npc.Center);
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, npc.Center);
                GeneralParticleHandler.SpawnParticle(new CustomPulse(npc.Center, -npc.velocity / 3f, FireGreen, "CalamityMod/Particles/DustyCircleHardEdge", new Vector2(0.4f, 1.2f), Vector2.Zero.AngleTo(npc.velocity) + (Main.rand.NextBool() ? 0 : -MathHelper.Pi), 0.05f, 0.2f, 20));
                GeneralParticleHandler.SpawnParticle(new CustomPulse(npc.Center, -npc.velocity / 2f, FireGreen, "CalamityMod/Particles/FlameExplosion", new Vector2(0.4f, 1.2f), Vector2.Zero.AngleTo(npc.velocity) + (Main.rand.NextBool() ? 0 : -MathHelper.Pi), 0.1f, 0.4f, 20));
            }
        }
        static void DoChargeVisual(NPC npc, int phase)
        {
            float progress = npc.ai[2];
            int chargeTime = 36;

            Color col = Color.Lerp(phase > 0 ? FireGreen : new Color(100, 100, 100, 55), new Color(55, 55, 55, 55), progress / chargeTime);

            if (phase > 1)
            {
                if (Math.Floor(VisualTimerSystem.GlobalVisualTimer % 6f) < 1f)
                    GeneralParticleHandler.SpawnParticle(new CustomPulse(npc.Center, npc.velocity / 3f, col, "CalamityMod/Particles/DustyCircleHardEdge", new Vector2(0.4f, 1f), Vector2.Zero.AngleTo(npc.velocity) + (Main.rand.NextBool() ? 0 : -MathHelper.Pi), 0.04f, 0.1f, 20));

                (npc.ModNPC as OldDuke).NuclearOverlayVisual = 1f;
            }

            if (Main.rand.NextBool())
                GeneralParticleHandler.SpawnParticle(new CustomSpark(npc.Center - (npc.velocity * 2f), npc.velocity / 2f, "CalamityMod/Particles/ForwardSmear", false, 10, Main.rand.NextFloat(0.3f, 0.5f), col, new Vector2(1f, Main.rand.NextFloat(1.45f, 1.6f)), fadeIn: true, extraRotation: MathHelper.ToRadians(180f)));
        }
        #endregion

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();
            shake = MathHelper.Lerp(shake, 0f, 0.1f);

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Variables
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            float exhaustionGateValue = 360f;
            if (Main.getGoodWorld)
                exhaustionGateValue *= 0.5f;

            float numberOfAttacksBeforeExhaustion = 12f;
            float exhaustionIncreasePerAttack = exhaustionGateValue * (1f / numberOfAttacksBeforeExhaustion);
            bool exhausted = calamityGlobalNPC.newAI[1] == 1f;
            bool phase2 = lifeRatio <= (death ? LifePercentagePhase2_Death : revenge ? LifePercentagePhase2_Revenge : LifePercentagePhase2_Normal);
            bool phase3 = lifeRatio <= (death ? LifePercentagePhase3_Death : (revenge ? LifePercentagePhase3_Revenge : LifePercentagePhase3_Expert)) && expertMode;
            bool phase2AI = NPC.ai[0] > 4f;
            bool phase3AI = NPC.ai[0] > 9f;

            Phase = phase3 ? 2 : phase2 ? 1 : 0;
            NuclearOverlayVisual = (Phase == 2 && NPC.Calamity().newAI[1] != 1) ? MathHelper.Lerp(NuclearOverlayVisual, 0.2f, 0.2f) : MathHelper.Lerp(NuclearOverlayVisual, 0f, 0.1f);

            bool charging = NPC.ai[3] < 10f;
            if (calamityGlobalNPC.newAI[0] >= exhaustionGateValue)
                calamityGlobalNPC.newAI[1] = 1f;

            float alphaScale = (255 - NPC.alpha) / 255f;
            float redLight = (phase3AI ? 0.4f : phase2AI ? 0.64f : 0.88f) * alphaScale;
            float greenLight = (phase3AI ? 1.2f : phase2AI ? 0.8f : 0.4f) * alphaScale;
            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), redLight, greenLight, 0f);

            if (CalamityServerConfig.Instance.BossesStopWeather)
                CalamityWorld.StopRain();
            else if (!Main.raining && !BossRushEvent.BossRushActive)
                CalamityWorld.StartRain();

            // Adjust stats
            NPC.damage = NPC.defDamage;
            calamityGlobalNPC.DR = exhausted ? 0f : 0.5f;
            NPC.defense = exhausted ? 0 : NPC.defDefense;
            if (phase3AI)
            {
                NPC.damage = (int)Math.Round(NPC.defDamage * Phase3ContactDamageMult);
                NPC.defense = exhausted ? 0 : NPC.defDefense - 40;
            }
            else if (phase2AI)
            {
                NPC.damage = (int)Math.Round(NPC.defDamage * Phase2ContactDamageMult);
                NPC.defense = exhausted ? 0 : NPC.defDefense - 20;
            }

            int idlePhaseTimer = expertMode ? 55 : 60;
            float idlePhaseAcceleration = expertMode ? 0.75f : 0.7f;
            float idlePhaseVelocity = expertMode ? 14f : 13f;
            if (phase3AI)
            {
                idlePhaseAcceleration = expertMode ? 0.6f : 0.55f;
                idlePhaseVelocity = expertMode ? 12f : 11f;
            }
            else if (phase2AI & charging)
            {
                idlePhaseAcceleration = expertMode ? 0.8f : 0.75f;
                idlePhaseVelocity = expertMode ? 15f : 14f;
            }

            int chargeTime = expertMode ? 34 : 36;
            float chargeVelocity = expertMode ? 20f : 19f;
            if (phase3AI)
            {
                chargeTime = expertMode ? 28 : 30;
                chargeVelocity = expertMode ? 26f : 25f;
            }
            else if (charging & phase2AI)
            {
                chargeTime = expertMode ? 31 : 33;
                chargeVelocity = expertMode ? 24f : 23f;
            }

            if (death)
            {
                idlePhaseTimer = 51;
                idlePhaseAcceleration *= 1.05f;
                idlePhaseVelocity *= 1.05f;
                chargeTime -= 2;
                chargeVelocity *= 1.1f;
            }
            else if (revenge)
            {
                idlePhaseTimer = 53;
                idlePhaseAcceleration *= 1.025f;
                idlePhaseVelocity *= 1.025f;
                chargeTime -= 1;
                chargeVelocity *= 1.05f;
            }

            // The dumbest thing to ever exist
            if (Main.zenithWorld)
                chargeVelocity *= 1.25f;

            if (exhausted)
                idlePhaseVelocity *= 0.25f;

            // Variables
            int maxToothBallBelches = death ? 4 : 3;
            int toothBallBelchPhaseDivisor = death ? 30 : 40;
            int toothBallBelchPhaseTimer = toothBallBelchPhaseDivisor * maxToothBallBelches;
            float toothBallBelchPhaseAcceleration = death ? 0.6f : 0.55f;
            float toothBallBelchPhaseVelocity = death ? 10f : 9f;
            float toothBallFinalVelocity = death ? 14f : revenge ? 13f : 12f;
            float goreVelocityX = death ? 8f : revenge ? 7.5f : expertMode ? 7f : 6f;
            float goreVelocityY = death ? 10.5f : revenge ? 10f : expertMode ? 9.5f : 8f;
            float sharkronVelocity = death ? 16f : revenge ? 15f : expertMode ? 14f : 12f;
            int attackTimer = 120;
            int phaseTransitionTimer = 180;
            int teleportPauseTimer = 30;
            int toothBallSpinPhaseDivisor = death ? 32 : 45;
            int toothBallSpinTimer = maxToothBallBelches * toothBallSpinPhaseDivisor;
            float spinTime = toothBallSpinTimer / 2f;
            float toothBallSpinToothBallVelocity = death ? 9.5f : 9f;
            float spinAttackSpeed = Main.zenithWorld ? 44f : 22f;
            float spinSpeed = MathHelper.TwoPi / spinTime;

            Player player = Main.player[NPC.target];

            // Get target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || player.dead || !player.active)
            {
                NPC.TargetClosest();
                player = Main.player[NPC.target];
                NPC.netUpdate = true;
            }

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(player.Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            // Despawn
            if (player.dead || Vector2.Distance(player.Center, NPC.Center) > 8800f)
            {
                NPC.TargetClosest();

                NPC.velocity.Y -= 0.4f;

                if (NPC.timeLeft > 10)
                    NPC.timeLeft = 10;

                if (NPC.timeLeft == 1)
                {
                    AcidRainEvent.AccumulatedKillPoints = 0;
                    AcidRainEvent.HasTriedToSummonOldDuke = false;
                    AcidRainEvent.UpdateInvasion(false);
                    NPC.timeLeft = 0;
                }

                if (NPC.ai[0] > 4f)
                    NPC.ai[0] = 5f;
                else
                    NPC.ai[0] = 0f;

                NPC.ai[2] = 0f;
            }

            if (exhausted)
            {
                // Disable contact damage while tired
                NPC.damage = 0;

                // Play exhausted sound and huff fumes
                if (calamityGlobalNPC.newAI[0] % 60f == 0f && Main.LocalPlayer.active && !Main.LocalPlayer.dead && Vector2.Distance(Main.LocalPlayer.Center, NPC.Center) < 2800f)
                {
                    SoundEngine.PlaySound(HuffSound with { Volume = HuffSound.Volume * 1.25f }, Main.LocalPlayer.Center);
                    for (int i = 0; i < 40; i++)
                    {
                        float scale = Main.rand.NextFloat(1f, 3f);
                        Vector2 fumePos = NPC.Center + NPC.rotation.ToRotationVector2() * (Main.rand.NextBool() ? 100f : 80f) * NPC.direction;
                        Vector2 fumeVel = Vector2.UnitX * NPC.velocity.X + Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(12f)) * scale * -8f * Main.rand.NextFloat(1f, 1.25f);
                        MediumMistParticle fume = new(fumePos, fumeVel, GlowColor, Color.DarkSlateGray, scale, 150f);
                        GeneralParticleHandler.SpawnParticle(fume);
                    }

                    Vector2 pulsePos = NPC.Center + NPC.rotation.ToRotationVector2() * 88f * NPC.direction;
                    CustomPulse pulse = new(pulsePos, Vector2.Zero, Color.White * 0.2f, "CalamityMod/Particles/DustyCircleHardEdge", Vector2.One, Main.rand.NextFloat(MathHelper.TwoPi), 0.05f, 0.125f, 30);
                    GeneralParticleHandler.SpawnParticle(pulse);
                }

                if (Main.zenithWorld)
                {
                    float screenShakePower = 10 * Utils.GetLerpValue(800f, 0f, NPC.Distance(Main.LocalPlayer.Center), true);
                    Main.LocalPlayer.SetScreenshake(screenShakePower);

                    if (calamityGlobalNPC.newAI[0] == exhaustionGateValue)
                        SoundEngine.PlaySound(SoundID.NPCDeath64 with { Pitch = SoundID.NPCDeath64.Pitch - 0.9f, Volume = SoundID.NPCDeath64.Volume + 0.4f }, player.Center); // fart

                    if (Main.netMode != NetmodeID.MultiplayerClient && calamityGlobalNPC.newAI[0] % 5f == 0f)
                    {
                        Vector2 dist = player.Center - NPC.Center;
                        dist.Normalize();
                        dist *= 3;
                        dist.X += Main.rand.NextFloat(-0.5f, 0.5f);
                        dist.Y += Main.rand.NextFloat(-0.5f, 0.5f);
                        int type = ModContent.ProjectileType<SandPoisonCloudOldDuke>();
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, -dist, type, FartDamage, 0, Main.myPlayer);
                    }
                }

                calamityGlobalNPC.newAI[0] -= 1f;
                if (calamityGlobalNPC.newAI[0] <= 0f)
                {
                    calamityGlobalNPC.newAI[0] = 0f;
                    calamityGlobalNPC.newAI[1] = 0f;
                }
            }

            // Enrage variable
            bool enrage = !BossRushEvent.BossRushActive &&
                (player.position.Y < 300f || player.position.Y > Main.worldSurface * 16.0 ||
                (player.position.X > 8000f && player.position.X < (Main.maxTilesX * 16 - 8000)));

            // Check for the flipped Abyss
            if (Main.remixWorld)
            {
                enrage = !BossRushEvent.BossRushActive &&
                    (player.position.Y < Main.UnderworldLayer * 16 * 0.8f || player.position.Y > Main.UnderworldLayer * 16 ||
                    (player.position.X > 8000f && player.position.X < (Main.maxTilesX * 16 - 8000)));
            }

            // Enrage
            if (enrage)
            {
                if (NPC.localAI[1] > 0f)
                    NPC.localAI[1] -= 1f;
            }
            else
                NPC.localAI[1] = CalamityGlobalNPC.biomeEnrageTimerMax;

            bool biomeEnraged = NPC.localAI[1] <= 0f;

            NPC.Calamity().CurrentlyEnraged = biomeEnraged;

            // Increased DR while transitioning phases and not exhausted
            if (!exhausted)
                calamityGlobalNPC.DR = (NPC.ai[0] == -1f || NPC.ai[0] == 4f || NPC.ai[0] == 9f) ? 0.75f : 0.5f;

            calamityGlobalNPC.CurrentlyIncreasingDefenseOrDR = NPC.ai[0] == -1f || NPC.ai[0] == 4f || NPC.ai[0] == 9f;

            // Enrage
            if (biomeEnraged)
            {
                toothBallBelchPhaseTimer = 60;
                toothBallBelchPhaseDivisor = 20;
                toothBallBelchPhaseAcceleration = 1f;
                toothBallBelchPhaseVelocity = 15f;
                goreVelocityX = 12f;
                goreVelocityY = 16f;
                sharkronVelocity = 20f;
                idlePhaseTimer = 20;
                idlePhaseAcceleration = 1.2f;
                idlePhaseVelocity = 20f;
                chargeTime = 25;
                chargeVelocity += 8f;
                toothBallSpinPhaseDivisor = 24;
                toothBallSpinToothBallVelocity = 15f;
                NPC.damage *= 2;
                NPC.defense = NPC.defDefense * 3;
            }

            // The dumbest thing to ever exist
            if (Main.zenithWorld)
                chargeTime *= 2;

            // Set variables for spawn effects
            if (NPC.localAI[0] == 0f)
            {
                NPC.alpha = 255;

                NPC.velocity.Y = -12f;
                NPC.velocity.X = Main.rand.NextFloat(-2f, 2f);

                NPC.localAI[0] = 1f;
                NPC.rotation = 0f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.ai[0] = -1f;
                    NPC.netUpdate = true;
                }
            }

            // Rotation
            float rateOfRotation = 0.04f;
            if (NPC.ai[0] == 1f || NPC.ai[0] == 6f || NPC.ai[0] == 7f || NPC.ai[0] == 14f)
                rateOfRotation = 0f;
            if (NPC.ai[0] == 3f || NPC.ai[0] == 4f)
                rateOfRotation = 0.01f;
            if (NPC.ai[0] == 8f || NPC.ai[0] == 13f)
                rateOfRotation = 0.05f;

            Vector2 rotationVector = player.Center - NPC.Center;
            if (calamityGlobalNPC.newAI[1] != 1f && !player.dead)
            {
                // Rotate to show direction of predictive charge
                if (NPC.ai[0] == 0f && !phase2)
                {
                    if (NPC.ai[3] < 6f)
                    {
                        rateOfRotation = 0.1f;
                        rotationVector = Vector2.Normalize(player.Center + player.velocity * 20f - NPC.Center) * chargeVelocity;
                    }
                }
                else if (NPC.ai[0] == 5f && !phase3)
                {
                    if (NPC.ai[3] < 4f)
                    {
                        rateOfRotation = 0.1f;
                        rotationVector = Vector2.Normalize(player.Center + player.velocity * 20f - NPC.Center) * chargeVelocity;
                    }
                }
                else if (NPC.ai[0] == 10f)
                {
                    if (NPC.ai[3] < 8f && NPC.ai[3] != 1f && NPC.ai[3] != 4f)
                    {
                        rateOfRotation = 0.1f;
                        rotationVector = Vector2.Normalize(player.Center + player.velocity * 20f - NPC.Center) * chargeVelocity;
                    }
                }
            }

            float dukeRotationSpeed = (float)Math.Atan2(rotationVector.Y, rotationVector.X);
            if (NPC.spriteDirection == 1)
                dukeRotationSpeed += MathHelper.Pi;
            if (dukeRotationSpeed < 0f)
                dukeRotationSpeed += MathHelper.TwoPi;
            if (dukeRotationSpeed > MathHelper.TwoPi)
                dukeRotationSpeed -= MathHelper.TwoPi;
            if (NPC.ai[0] == -1f || NPC.ai[0] == 3f || NPC.ai[0] == 4f)
                dukeRotationSpeed = 0f;
            if (NPC.ai[0] == 8f || NPC.ai[0] == 13f)
                dukeRotationSpeed = MathHelper.Pi * 0.1666666667f * NPC.spriteDirection;

            if (NPC.rotation < dukeRotationSpeed)
            {
                if (dukeRotationSpeed - NPC.rotation > MathHelper.Pi)
                    NPC.rotation -= rateOfRotation;
                else
                    NPC.rotation += rateOfRotation;
            }
            if (NPC.rotation > dukeRotationSpeed)
            {
                if (NPC.rotation - dukeRotationSpeed > MathHelper.Pi)
                    NPC.rotation += rateOfRotation;
                else
                    NPC.rotation -= rateOfRotation;
            }

            if ((NPC.ai[0] != 8f && NPC.ai[0] != 13f) || NPC.spriteDirection == 1)
            {
                if (NPC.rotation > dukeRotationSpeed - rateOfRotation && NPC.rotation < dukeRotationSpeed + rateOfRotation)
                    NPC.rotation = dukeRotationSpeed;
                if (NPC.rotation < 0f)
                    NPC.rotation += MathHelper.TwoPi;
                if (NPC.rotation > MathHelper.TwoPi)
                    NPC.rotation -= MathHelper.TwoPi;
                if (NPC.rotation > dukeRotationSpeed - rateOfRotation && NPC.rotation < dukeRotationSpeed + rateOfRotation)
                    NPC.rotation = dukeRotationSpeed;
            }

            // Alpha adjustments
            if (NPC.ai[0] != -1f && (NPC.ai[0] < 9f || NPC.ai[0] > 12f))
            {
                if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                    NPC.alpha += 15;
                else
                    NPC.alpha -= 15;

                if (NPC.alpha < 0)
                    NPC.alpha = 0;
                if (NPC.alpha > 150)
                    NPC.alpha = 150;
            }

            // Spawn effects
            if (NPC.ai[0] == -1f)
            {
                NPC.alpha = (int)MathHelper.Lerp(NPC.alpha, 0, 0.2f);

                // Disable contact damage while spawning
                NPC.damage = 0;

                // Velocity
                if (NPC.Calamity().newAI[3] == 0f)
                    NPC.velocity *= 0.98f;

                // Direction
                int dukeFaceDirection = Math.Sign(player.Center.X - NPC.Center.X);
                if (dukeFaceDirection != 0)
                {
                    NPC.direction = dukeFaceDirection;
                    NPC.spriteDirection = -NPC.direction;
                }

                // Alpha
                if (NPC.ai[2] > 20f)
                {
                    NPC.velocity *= 0.9f;

                    NPC.alpha -= 5;
                    if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                        NPC.alpha += 15;
                    if (NPC.alpha < 0)
                        NPC.alpha = 0;
                    if (NPC.alpha > 150)
                        NPC.alpha = 150;
                }

                if (NPC.ai[2] == 75)
                {
                    RoarSoundSlot = SoundEngine.PlaySound(RoarSound, NPC.Center);

                    CalamityUtils.AddScreenshakeAt(NPC.Center, 14);
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath.WithPitchOffset(-0.5f), NPC.Center);
                    SoundEngine.PlaySound(DashSoundP3, NPC.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        CalamityUtils.BossAwakenMessage(NPC.whoAmI);
                }

                if (NPC.ai[2] >= 75f)
                {
                    float sd = player.Center.X > NPC.Center.X ? -1f : 1f;

                    shake = 2f;
                    NPC.rotation = MathHelper.Lerp(MathHelper.WrapAngle(NPC.rotation), MathHelper.ToRadians(sd == 1f ? 75f : -75f), 0.2f);

                    if (NPC.ai[2] % 5f == 1f)
                    {
                        Vector2 mouthCenter = NPC.Center + new Vector2(-80 * sd, 0).RotatedBy(NPC.rotation);

                        float factor = (NPC.ai[2] - 75f) / 50f;

                        float a = MathHelper.Lerp(1f, 0f, factor);

                        GeneralParticleHandler.SpawnParticle(new CustomPulse(mouthCenter, Vector2.Zero, new Color(55, 55, 55).MultiplyRGBA(new Color(a, a, a, a)), "CalamityMod/Particles/DustyCircleHardEdge", Vector2.One, Main.rand.NextFloat(MathHelper.TwoPi), 0.05f, 0.08f + (0.15f * factor), 10));
                    }
                }

                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= 125f)
                {
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.netUpdate = true;
                }
            }

            // Phase 1
            else if (NPC.ai[0] == 0f && !player.dead)
            {
                // Velocity
                if (NPC.ai[1] == 0f)
                    NPC.ai[1] = 500 * Math.Sign((NPC.Center - player.Center).X);

                Vector2 phase1IdleSpeed = Vector2.Normalize(player.Center + new Vector2(NPC.ai[1], -300f) - NPC.Center - NPC.velocity) * idlePhaseVelocity;
                NPC.SimpleFlyMovement(phase1IdleSpeed, idlePhaseAcceleration);

                // Rotation and direction
                int dukeLookAt = Math.Sign(player.Center.X - NPC.Center.X);
                if (dukeLookAt != 0)
                {
                    if (NPC.ai[2] == 0f && dukeLookAt != NPC.direction)
                        NPC.rotation += MathHelper.Pi;

                    NPC.direction = dukeLookAt;

                    if (NPC.spriteDirection != -NPC.direction)
                        NPC.rotation += MathHelper.Pi;

                    NPC.spriteDirection = -NPC.direction;
                }

                // Phase switch
                if (calamityGlobalNPC.newAI[1] != 1f || (phase2 && !phase2AI))
                {
                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] >= idlePhaseTimer || (phase2 && !phase2AI))
                    {
                        int oldDukeAttackPicker = 0;
                        switch ((int)NPC.ai[3])
                        {
                            case 0:
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                                oldDukeAttackPicker = 1;
                                break;
                            case 6:
                                NPC.ai[3] = 1f;
                                oldDukeAttackPicker = 2;
                                break;
                            case 7:
                                NPC.ai[3] = 0f;
                                oldDukeAttackPicker = 3;
                                break;
                        }

                        if (phase2)
                            oldDukeAttackPicker = 4;

                        // Set velocity for charge
                        if (oldDukeAttackPicker == 1)
                        {
                            NPC.ai[0] = 1f;
                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 0f;

                            // Velocity
                            Vector2 distanceVector = player.Center + player.velocity * 20f - NPC.Center;
                            NPC.velocity = Vector2.Normalize(distanceVector) * chargeVelocity;
                            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X);

                            DoChargeBurst(NPC, 0);

                            // Direction
                            if (dukeLookAt != 0)
                            {
                                NPC.direction = dukeLookAt;

                                if (NPC.spriteDirection == 1)
                                    NPC.rotation += MathHelper.Pi;

                                NPC.spriteDirection = -NPC.direction;
                            }
                        }

                        // Tooth Balls
                        else if (oldDukeAttackPicker == 2)
                        {
                            NPC.ai[0] = 2f;
                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 0f;
                        }

                        // Call sharks from the sides of the screen
                        else if (oldDukeAttackPicker == 3)
                        {
                            NPC.ai[0] = 3f;
                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 0f;
                        }

                        // Go to phase 2
                        else if (oldDukeAttackPicker == 4)
                        {
                            NPC.ai[0] = 4f;
                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 0f;
                        }

                        NPC.netUpdate = true;
                    }
                }
            }

            // Charge
            else if (NPC.ai[0] == 1f)
            {
                // The dumbest thing to ever exist
                if (Main.zenithWorld && NPC.ai[2] % 10f == 0f)
                {
                    // Rotation and direction
                    int dir = Math.Sign(player.Center.X - NPC.Center.X);
                    if (dir != 0)
                    {
                        if (NPC.ai[2] == 0f && dir != NPC.direction)
                            NPC.rotation += MathHelper.Pi;

                        NPC.direction = dir;

                        if (NPC.spriteDirection != -NPC.direction)
                            NPC.rotation += MathHelper.Pi;

                        NPC.spriteDirection = -NPC.direction;
                    }

                    Vector2 distanceVector = player.Center + player.velocity * 20f - NPC.Center;
                    NPC.velocity = Vector2.Normalize(distanceVector) * chargeVelocity;
                    NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X);

                    // Direction
                    if (dir != 0)
                    {
                        NPC.direction = dir;

                        if (NPC.spriteDirection == 1)
                            NPC.rotation += MathHelper.Pi;

                        NPC.spriteDirection = -NPC.direction;
                    }
                }
                else
                {
                    // Accelerate
                    NPC.velocity *= 1.01f;
                }

                // Spawn dust
                Vector2 vel = NPC.velocity;
                vel.Normalize();
                vel *= 55f;

                DoChargeVisual(NPC, Phase);

                GeneralParticleHandler.SpawnParticle(new SparkParticle(NPC.Center + vel + vel.RotatedBy(MathHelper.ToRadians(90)), vel / 7f, false, 20, 0.75f, new Color(155, 155, 155, 55), true));
                GeneralParticleHandler.SpawnParticle(new SparkParticle(NPC.Center + vel + vel.RotatedBy(MathHelper.ToRadians(-90)), vel / 7f, false, 20, 0.75f, new Color(155, 155, 155, 55), true));

                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= chargeTime)
                {
                    calamityGlobalNPC.newAI[0] += exhaustionIncreasePerAttack;
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] += 2f;
                    NPC.TargetClosest();
                    NPC.netUpdate = true;
                }
            }

            // Tooth Ball belch
            else if (NPC.ai[0] == 2f)
            {
                // Velocity
                if (NPC.ai[1] == 0f)
                    NPC.ai[1] = 500 * Math.Sign((NPC.Center - player.Center).X);

                Vector2 toothBallBelchPhaseSpeed = Vector2.Normalize(player.Center + new Vector2(NPC.ai[1], -300f) - NPC.Center - NPC.velocity) * toothBallBelchPhaseVelocity;
                NPC.SimpleFlyMovement(toothBallBelchPhaseSpeed, toothBallBelchPhaseAcceleration);

                // Play sounds and spawn Tooth Balls
                if (NPC.ai[2] == 0f)
                    RoarSoundSlot = SoundEngine.PlaySound(RoarSound, NPC.Center);

                if (NPC.ai[2] % toothBallBelchPhaseDivisor == 0f)
                {
                    if (NPC.ai[2] != 0f)
                        SoundEngine.PlaySound(VomitSound, NPC.Center);

                    Vector2 toothBallDirection = Vector2.Normalize(player.Center - NPC.Center) * (NPC.width + 20) / 2f + NPC.Center;
                    Vector2 toothBallVelocity = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * toothBallFinalVelocity;
                    Vector2 toothBallSpawnPos = new Vector2(toothBallDirection.X, toothBallDirection.Y + 45f);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int toothBall = NPC.NewNPC(NPC.GetSource_FromAI(), (int)toothBallSpawnPos.X, (int)toothBallSpawnPos.Y, ModContent.NPCType<OldDukeToothBall>(), 0, toothBallVelocity.X, toothBallVelocity.Y);
                        Main.npc[toothBall].velocity = Vector2.Normalize(toothBallVelocity) * NPC.velocity.Length();
                        Main.npc[toothBall].netUpdate = true;
                    }

                    for (int i = 0; i < 50; i++)
                    {
                        int dustID;
                        switch (Main.rand.Next(6))
                        {
                            case 0:
                            case 1:
                            case 2:
                            case 3:
                                dustID = (int)CalamityDusts.SulphurousSeaAcid;
                                break;
                            default:
                                dustID = DustID.Blood;
                                break;
                        }

                        // Choose a random speed and angle to belch out the vomit
                        float dustSpeed = Main.rand.NextFloat(3.0f, 12.0f);
                        float angleRandom = 0.06f;
                        Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(toothBallVelocity.ToRotation());
                        dustVel = dustVel.RotatedBy(-angleRandom);
                        dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);

                        // Pick a size for the vomit particles
                        float scale = Main.rand.NextFloat(1f, 2f);

                        // Actually spawn the vomit
                        int idx = Dust.NewDust(toothBallSpawnPos, 40, 40, dustID, dustVel.X, dustVel.Y, 0, default, scale);
                        Main.dust[idx].noGravity = true;
                    }
                }

                // Direction
                int toothBallLookAt = Math.Sign(player.Center.X - NPC.Center.X);
                if (toothBallLookAt != 0)
                {
                    NPC.direction = toothBallLookAt;
                    if (NPC.spriteDirection != -NPC.direction)
                        NPC.rotation += MathHelper.Pi;
                    NPC.spriteDirection = -NPC.direction;
                }

                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= toothBallBelchPhaseTimer)
                {
                    calamityGlobalNPC.newAI[0] += exhaustionIncreasePerAttack;
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.TargetClosest();
                    NPC.netUpdate = true;
                }
            }

            // Call sharks from the sides of the screen
            else if (NPC.ai[0] == 3f)
            {
                // Velocity
                NPC.velocity *= 0.98f;
                NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, 0f, 0.02f);

                // Play sound and spawn sharks
                if (NPC.ai[2] == attackTimer - 30)
                    SoundEngine.PlaySound(VomitSound, NPC.Center);

                if (NPC.ai[2] >= attackTimer - 90)
                {
                    if (NPC.ai[2] % 18f == 0f)
                    {
                        calamityGlobalNPC.newAI[2] += 100f;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X + 900f), (int)(NPC.Center.Y - calamityGlobalNPC.newAI[2]), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, NPC.whoAmI, 0f, 255);
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X - 900f), (int)(NPC.Center.Y - calamityGlobalNPC.newAI[2]), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, NPC.whoAmI, 0f, 255);

                            if (Main.getGoodWorld)
                            {
                                NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X + 1800f), (int)(NPC.Center.Y - calamityGlobalNPC.newAI[2]), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, NPC.whoAmI, 0f, 255);
                                NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X - 1800f), (int)(NPC.Center.Y - calamityGlobalNPC.newAI[2]), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, NPC.whoAmI, 0f, 255);
                            }
                        }
                    }
                }

                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= attackTimer)
                {
                    calamityGlobalNPC.newAI[0] += exhaustionIncreasePerAttack;
                    calamityGlobalNPC.newAI[2] = 0f;
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.TargetClosest();
                    NPC.netUpdate = true;
                }
            }

            // Transition to phase 2 and call sharks from below
            else if (NPC.ai[0] == 4f)
            {
                // Velocity
                NPC.velocity *= 0.98f;
                NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, 0f, 0.02f);

                // Sound
                if (NPC.ai[2] == phaseTransitionTimer - 60)
                    RoarSoundSlot = SoundEngine.PlaySound(RoarSound, NPC.Center);

                if (NPC.ai[2] >= phaseTransitionTimer - 60)
                {
                    if (NPC.ai[2] % 18f == 0f)
                    {
                        calamityGlobalNPC.newAI[2] += 150f;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X + 50f + calamityGlobalNPC.newAI[2]), (int)(NPC.Center.Y + 540f), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, 1f, -sharkronVelocity, 255);
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X - 50f - calamityGlobalNPC.newAI[2]), (int)(NPC.Center.Y + 540f), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, -1f, -sharkronVelocity, 255);

                            if (Main.getGoodWorld)
                            {
                                NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X + 50f + calamityGlobalNPC.newAI[2] * 0.5f), (int)(NPC.Center.Y + 270f), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, 1f, -sharkronVelocity, 255);
                                NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X - 50f - calamityGlobalNPC.newAI[2] * 0.5f), (int)(NPC.Center.Y + 270f), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, -1f, -sharkronVelocity, 255);
                            }
                        }
                    }
                }

                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= phaseTransitionTimer)
                {
                    calamityGlobalNPC.newAI[0] = 0f;
                    calamityGlobalNPC.newAI[1] = 0f;
                    calamityGlobalNPC.newAI[2] = 0f;
                    NPC.ai[0] = 5f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.TargetClosest();
                    NPC.netUpdate = true;
                }
            }

            // Phase 2
            else if (NPC.ai[0] == 5f && !player.dead)
            {
                // Velocity
                if (NPC.ai[1] == 0f)
                    NPC.ai[1] = 500 * Math.Sign((NPC.Center - player.Center).X);

                Vector2 dukePhase2IdleSpeed = Vector2.Normalize(player.Center + new Vector2(NPC.ai[1], -300f) - NPC.Center - NPC.velocity) * idlePhaseVelocity;
                NPC.SimpleFlyMovement(dukePhase2IdleSpeed, idlePhaseAcceleration);

                // Direction and rotation
                int phase2FaceDirection = Math.Sign(player.Center.X - NPC.Center.X);
                if (phase2FaceDirection != 0)
                {
                    if (NPC.ai[2] == 0f && phase2FaceDirection != NPC.direction)
                        NPC.rotation += MathHelper.Pi;

                    NPC.direction = phase2FaceDirection;

                    if (NPC.spriteDirection != -NPC.direction)
                        NPC.rotation += MathHelper.Pi;

                    NPC.spriteDirection = -NPC.direction;
                }

                // Phase switch
                if (calamityGlobalNPC.newAI[1] != 1f || (phase3 && !phase3AI))
                {
                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] >= idlePhaseTimer || (phase3 && !phase3AI))
                    {
                        int phase2AttackPicker = 0;
                        switch ((int)NPC.ai[3])
                        {
                            case 0:
                            case 1:
                            case 2:
                            case 3:
                                phase2AttackPicker = 1;
                                break;
                            case 4:
                                NPC.ai[3] = 1f;
                                phase2AttackPicker = 2;
                                break;
                            case 5:
                                NPC.ai[3] = 0f;
                                phase2AttackPicker = 3;
                                break;
                        }

                        if (phase3)
                            phase2AttackPicker = 4;

                        // Set velocity for charge
                        if (phase2AttackPicker == 1)
                        {
                            NPC.ai[0] = 6f;
                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 0f;

                            // Velocity and rotation
                            Vector2 distanceVector = player.Center + player.velocity * 20f - NPC.Center;
                            NPC.velocity = Vector2.Normalize(distanceVector) * chargeVelocity;
                            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X);

                            // Direction
                            if (phase2FaceDirection != 0)
                            {
                                NPC.direction = phase2FaceDirection;

                                if (NPC.spriteDirection == 1)
                                    NPC.rotation += MathHelper.Pi;

                                NPC.spriteDirection = -NPC.direction;
                            }

                            DoChargeBurst(NPC, 1);
                        }

                        // Set velocity for spin
                        else if (phase2AttackPicker == 2)
                        {
                            // Velocity and rotation
                            NPC.velocity = Vector2.Normalize(player.Center - NPC.Center) * spinAttackSpeed;
                            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X);

                            // Direction
                            if (phase2FaceDirection != 0)
                            {
                                NPC.direction = phase2FaceDirection;

                                if (NPC.spriteDirection == 1)
                                    NPC.rotation += MathHelper.Pi;

                                NPC.spriteDirection = -NPC.direction;
                            }

                            NPC.ai[0] = 7f;
                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 0f;
                        }

                        else if (phase2AttackPicker == 3)
                        {
                            NPC.ai[0] = 8f;
                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 0f;
                        }

                        // Go to next phase
                        else if (phase2AttackPicker == 4)
                        {
                            NPC.ai[0] = 9f;
                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 0f;
                        }

                        NPC.netUpdate = true;
                    }
                }
            }

            // Charge
            else if (NPC.ai[0] == 6f)
            {
                // The dumbest thing to ever exist
                if (Main.zenithWorld && NPC.ai[2] % 8f == 0f)
                {
                    // Rotation and direction
                    int dir = Math.Sign(player.Center.X - NPC.Center.X);
                    if (dir != 0)
                    {
                        if (NPC.ai[2] == 0f && dir != NPC.direction)
                            NPC.rotation += MathHelper.Pi;

                        NPC.direction = dir;

                        if (NPC.spriteDirection != -NPC.direction)
                            NPC.rotation += MathHelper.Pi;

                        NPC.spriteDirection = -NPC.direction;
                    }

                    Vector2 distanceVector = player.Center + player.velocity * 20f - NPC.Center;
                    NPC.velocity = Vector2.Normalize(distanceVector) * chargeVelocity;
                    NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X);

                    // Direction
                    if (dir != 0)
                    {
                        NPC.direction = dir;

                        if (NPC.spriteDirection == 1)
                            NPC.rotation += MathHelper.Pi;

                        NPC.spriteDirection = -NPC.direction;
                    }
                }
                else
                {
                    // Accelerate
                    NPC.velocity *= 1.01f;
                }

                DoChargeVisual(NPC, Phase);

                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= chargeTime)
                {
                    calamityGlobalNPC.newAI[0] += exhaustionIncreasePerAttack;
                    NPC.ai[0] = 5f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] += 2f;
                    NPC.TargetClosest();
                    NPC.netUpdate = true;
                }
            }

            // Tooth Ball and Vortex spin
            else if (NPC.ai[0] == 7f)
            {
                // Play sounds and spawn Tooth Balls and a Vortex
                if (NPC.ai[2] == 0f)
                {
                    RoarSoundSlot = SoundEngine.PlaySound(RoarSound, NPC.Center);

                    SoundEngine.PlaySound(VortexSpawnSound, NPC.Center);
                    int type = ModContent.ProjectileType<OldDukeVortex>();
                    Vector2 vortexSpawn = NPC.Center + NPC.velocity.RotatedBy(MathHelper.PiOver2 * -NPC.direction) * spinTime / MathHelper.TwoPi;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), vortexSpawn, Vector2.Zero, type, VortexDamage, 0f, Main.myPlayer, vortexSpawn.X, vortexSpawn.Y);
                }

                if (NPC.ai[2] % toothBallSpinPhaseDivisor == 0f)
                {
                    if (NPC.ai[2] != 0f)
                        SoundEngine.PlaySound(VomitSound, NPC.Center);

                    Vector2 phase2ToothBallDirection = Vector2.Normalize(NPC.velocity) * (NPC.width + 20) / 2f + NPC.Center;
                    Vector2 toothBallVelocity = Vector2.Normalize(NPC.velocity).RotatedBy(MathHelper.PiOver2 * NPC.direction) * toothBallSpinToothBallVelocity;
                    Vector2 toothBallSpawnPos = new Vector2(phase2ToothBallDirection.X, phase2ToothBallDirection.Y + 45f);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int toothBall = NPC.NewNPC(NPC.GetSource_FromAI(), (int)toothBallSpawnPos.X, (int)toothBallSpawnPos.Y, ModContent.NPCType<OldDukeToothBall>(), 0, toothBallVelocity.X, toothBallVelocity.Y);
                        Main.npc[toothBall].target = NPC.target;
                        Main.npc[toothBall].velocity = Vector2.Normalize(toothBallVelocity) * toothBallSpinToothBallVelocity * 0.5f;
                        Main.npc[toothBall].netUpdate = true;
                        Main.npc[toothBall].ai[3] = 30f;
                    }

                    for (int i = 0; i < 50; i++)
                    {
                        int dustID;
                        switch (Main.rand.Next(6))
                        {
                            case 0:
                            case 1:
                            case 2:
                            case 3:
                                dustID = (int)CalamityDusts.SulphurousSeaAcid;
                                break;
                            default:
                                dustID = DustID.Blood;
                                break;
                        }

                        // Choose a random speed and angle to belch out the vomit
                        float dustSpeed = Main.rand.NextFloat(3.0f, 12.0f);
                        float angleRandom = 0.06f;
                        Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(toothBallVelocity.ToRotation());
                        dustVel = dustVel.RotatedBy(-angleRandom);
                        dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);

                        // Pick a size for the vomit particles
                        float scale = Main.rand.NextFloat(1f, 2f);

                        // Actually spawn the vomit
                        int idx = Dust.NewDust(toothBallSpawnPos, 40, 40, dustID, dustVel.X, dustVel.Y, 0, default, scale);
                        Main.dust[idx].noGravity = true;
                    }
                }

                // Velocity and rotation
                NPC.velocity = NPC.velocity.RotatedBy(-(double)spinSpeed * (float)NPC.direction);
                NPC.rotation -= spinSpeed * NPC.direction;

                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= toothBallSpinTimer)
                {
                    calamityGlobalNPC.newAI[0] += exhaustionIncreasePerAttack;
                    NPC.ai[0] = 5f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.TargetClosest();
                    NPC.netUpdate = true;
                }
            }

            // Vomit a huge amount of gore into the sky and call sharks from the sides of the screen
            else if (NPC.ai[0] == 8f)
            {
                // Velocity
                NPC.velocity *= 0.98f;
                NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, 0f, 0.02f);

                // Play sound
                if (NPC.ai[2] == attackTimer - 30)
                {
                    SoundEngine.PlaySound(VomitSound, NPC.Center);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 phase2GoreDirection = NPC.rotation.ToRotationVector2() * (Vector2.UnitX * NPC.direction) * (NPC.width + 20) / 2f + NPC.Center;
                        int type = ModContent.ProjectileType<OldDukeGore>();
                        int totalGore = Main.getGoodWorld ? 40 : 20;
                        for (int i = 0; i < totalGore; i++)
                        {
                            float velocityX = NPC.direction * goreVelocityX * (Main.rand.NextFloat(0.2f, 0.8f) + 0.5f);
                            float velocityY = goreVelocityY * (Main.rand.NextFloat(0.2f, 0.8f) + 0.5f);

                            if (Main.getGoodWorld)
                            {
                                velocityX *= Main.rand.NextFloat() + 0.5f;
                                velocityY *= Main.rand.NextFloat() + 0.5f;
                            }

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), phase2GoreDirection.X, phase2GoreDirection.Y, velocityX, -velocityY, type, GoreDamage, 0f, Main.myPlayer, 0f, 0f);
                        }
                    }
                }

                if (NPC.ai[2] >= attackTimer - 90)
                {
                    if (NPC.ai[2] % 18f == 0f)
                    {
                        calamityGlobalNPC.newAI[2] += 100f;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float x = 900f - calamityGlobalNPC.newAI[2];
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X + x), (int)(NPC.Center.Y - calamityGlobalNPC.newAI[2]), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, NPC.whoAmI, 0f, 255);
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X - x), (int)(NPC.Center.Y - calamityGlobalNPC.newAI[2]), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, NPC.whoAmI, 0f, 255);
                        }
                    }
                }

                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= attackTimer)
                {
                    calamityGlobalNPC.newAI[0] += exhaustionIncreasePerAttack;
                    calamityGlobalNPC.newAI[2] = 0f;
                    NPC.ai[0] = 5f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.TargetClosest();
                    NPC.netUpdate = true;
                }
            }

            // Transition to phase 3 and summon sharks from above
            else if (NPC.ai[0] == 9f)
            {
                // Velocity
                NPC.velocity *= 0.98f;
                NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, 0f, 0.02f);

                // Sound
                if (NPC.ai[2] == phaseTransitionTimer - 60)
                    RoarSoundSlot = SoundEngine.PlaySound(RoarSound, NPC.Center);

                if (NPC.ai[2] >= phaseTransitionTimer - 60)
                {
                    if (NPC.ai[2] % 18f == 0f)
                    {
                        calamityGlobalNPC.newAI[2] += 200f;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X + 50f + calamityGlobalNPC.newAI[2]), (int)(NPC.Center.Y - 540f), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, 1f, sharkronVelocity, 255);
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X - 50f - calamityGlobalNPC.newAI[2]), (int)(NPC.Center.Y - 540f), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, -1f, sharkronVelocity, 255);

                            if (Main.getGoodWorld)
                            {
                                NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X + 50f + calamityGlobalNPC.newAI[2] * 0.5f), (int)(NPC.Center.Y + 270f), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, 1f, -sharkronVelocity, 255);
                                NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X - 50f - calamityGlobalNPC.newAI[2] * 0.5f), (int)(NPC.Center.Y + 270f), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, -1f, -sharkronVelocity, 255);
                            }
                        }
                    }
                }

                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= phaseTransitionTimer)
                {
                    calamityGlobalNPC.newAI[0] = 0f;
                    calamityGlobalNPC.newAI[1] = 0f;
                    calamityGlobalNPC.newAI[2] = 0f;
                    NPC.ai[0] = 10f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.TargetClosest();
                    NPC.netUpdate = true;
                }
            }

            // Phase 3
            else if (NPC.ai[0] == 10f && !player.dead)
            {
                // Alpha
                NPC.alpha -= 25;
                if (NPC.alpha < 0)
                    NPC.alpha = 0;

                // Movement location
                if (NPC.ai[1] == 0f)
                    NPC.ai[1] = 500 * Math.Sign((NPC.Center - player.Center).X);

                Vector2 desiredVelocity = Vector2.Normalize(player.Center + new Vector2(-NPC.ai[1], -300f) - NPC.Center - NPC.velocity) * idlePhaseVelocity;
                NPC.SimpleFlyMovement(desiredVelocity, idlePhaseAcceleration);

                // Rotation and direction
                int phase3FaceDirection = Math.Sign(player.Center.X - NPC.Center.X);
                if (phase3FaceDirection != 0)
                {
                    if (NPC.ai[2] == 0f && phase3FaceDirection != NPC.direction)
                    {
                        NPC.rotation += MathHelper.Pi;
                        for (int l = 0; l < NPC.oldPos.Length; l++)
                            NPC.oldPos[l] = Vector2.Zero;
                    }

                    NPC.direction = phase3FaceDirection;

                    if (NPC.spriteDirection != -NPC.direction)
                        NPC.rotation += MathHelper.Pi;

                    NPC.spriteDirection = -NPC.direction;
                }

                // Phase switch
                if (calamityGlobalNPC.newAI[1] != 1f)
                {
                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] >= idlePhaseTimer)
                    {
                        int phase3AttackPicker = 0;
                        switch ((int)NPC.ai[3])
                        {
                            case 0:
                            case 2:
                            case 3:
                            case 5:
                            case 6:
                            case 7:
                                phase3AttackPicker = 1;
                                break;
                            case 1:
                            case 8:
                                phase3AttackPicker = 2;
                                break;
                            case 4:
                                NPC.ai[3] = 1f;
                                phase3AttackPicker = 3;
                                break;
                            case 9:
                                NPC.ai[3] = 6f;
                                phase3AttackPicker = 4;
                                break;
                        }

                        // Set velocity for charge
                        if (phase3AttackPicker == 1)
                        {
                            NPC.ai[0] = 11f;
                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 0f;

                            // Velocity and rotation
                            Vector2 distanceVector = player.Center + player.velocity * 20f - NPC.Center;
                            NPC.velocity = Vector2.Normalize(distanceVector) * chargeVelocity;
                            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X);

                            // Direction
                            if (phase3FaceDirection != 0)
                            {
                                NPC.direction = phase3FaceDirection;

                                if (NPC.spriteDirection == 1)
                                    NPC.rotation += MathHelper.Pi;

                                NPC.spriteDirection = -NPC.direction;
                            }

                            DoChargeBurst(NPC, 2);
                        }

                        // Pause
                        else if (phase3AttackPicker == 2)
                        {
                            NPC.ai[0] = 12f;
                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 0f;
                        }

                        else if (phase3AttackPicker == 3)
                        {
                            NPC.ai[0] = 13f;
                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 0f;
                        }

                        // Set velocity for spin
                        else if (phase3AttackPicker == 4)
                        {
                            // Velocity and rotation
                            NPC.velocity = Vector2.Normalize(player.Center - NPC.Center) * spinAttackSpeed;
                            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X);

                            // Direction
                            if (phase3FaceDirection != 0)
                            {
                                NPC.direction = phase3FaceDirection;

                                if (NPC.spriteDirection == 1)
                                    NPC.rotation += MathHelper.Pi;

                                NPC.spriteDirection = -NPC.direction;
                            }

                            NPC.ai[0] = 14f;
                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 0f;
                        }

                        NPC.netUpdate = true;
                    }
                }
            }

            // Charge
            else if (NPC.ai[0] == 11f)
            {
                // The dumbest thing to ever exist
                if (Main.zenithWorld && NPC.ai[2] % 6f == 0f)
                {
                    // Rotation and direction
                    int dir = Math.Sign(player.Center.X - NPC.Center.X);
                    if (dir != 0)
                    {
                        if (NPC.ai[2] == 0f && dir != NPC.direction)
                            NPC.rotation += MathHelper.Pi;

                        NPC.direction = dir;

                        if (NPC.spriteDirection != -NPC.direction)
                            NPC.rotation += MathHelper.Pi;

                        NPC.spriteDirection = -NPC.direction;
                    }

                    Vector2 distanceVector = player.Center + player.velocity * 20f - NPC.Center;
                    NPC.velocity = Vector2.Normalize(distanceVector) * chargeVelocity;
                    NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X);

                    // Direction
                    if (dir != 0)
                    {
                        NPC.direction = dir;

                        if (NPC.spriteDirection == 1)
                            NPC.rotation += MathHelper.Pi;

                        NPC.spriteDirection = -NPC.direction;
                    }
                }
                else
                {
                    // Accelerate
                    NPC.velocity *= 1.01f;
                }

                DoChargeVisual(NPC, Phase);

                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= chargeTime)
                {
                    calamityGlobalNPC.newAI[0] += exhaustionIncreasePerAttack;
                    NPC.ai[0] = 10f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] += 2f;
                    NPC.TargetClosest();
                    NPC.netUpdate = true;
                }
            }

            // Pause before teleport
            else if (NPC.ai[0] == 12f)
            {
                // Disable contact damage during the teleporting phase
                NPC.damage = 0;

                // Alpha
                if (NPC.alpha < 255 && NPC.ai[2] >= teleportPauseTimer - 15f)
                {
                    NPC.alpha += 17;
                    if (NPC.alpha > 255)
                        NPC.alpha = 255;
                }

                // Velocity
                NPC.velocity *= 0.98f;
                NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, 0f, 0.02f);

                // Play sound
                if (NPC.ai[2] == teleportPauseTimer / 2)
                    RoarSoundSlot = SoundEngine.PlaySound(RoarSound, NPC.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient && NPC.ai[2] == teleportPauseTimer - 1f)
                {
                    // Teleport location
                    if (NPC.ai[1] == 0f)
                        NPC.ai[1] = 600 * Math.Sign((NPC.Center - player.Center).X);

                    // Rotation and direction
                    Vector2 center = player.Center + new Vector2(NPC.ai[1], -300f);
                    Vector2 npcCenter = NPC.Center = center;
                    int phase3TeleportFaceDirection = Math.Sign(player.Center.X - npcCenter.X);
                    if (phase3TeleportFaceDirection != 0)
                    {
                        if (NPC.ai[2] == 0f && phase3TeleportFaceDirection != NPC.direction)
                        {
                            NPC.rotation += MathHelper.Pi;
                            for (int n = 0; n < NPC.oldPos.Length; n++)
                                NPC.oldPos[n] = Vector2.Zero;
                        }

                        NPC.direction = phase3TeleportFaceDirection;

                        if (NPC.spriteDirection != -NPC.direction)
                            NPC.rotation += MathHelper.Pi;

                        NPC.spriteDirection = -NPC.direction;
                    }
                }

                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= teleportPauseTimer)
                {
                    NPC.ai[0] = 10f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;

                    NPC.ai[3] += 2f;
                    if (NPC.ai[3] >= 9f)
                        NPC.ai[3] = 0f;

                    NPC.netUpdate = true;
                }
            }

            // Vomit a huge amount of gore into the sky and call sharks from the sides of the screen
            else if (NPC.ai[0] == 13f)
            {
                // Velocity
                NPC.velocity *= 0.98f;
                NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, 0f, 0.02f);

                // Play sound
                if (NPC.ai[2] == attackTimer - 30)
                {
                    SoundEngine.PlaySound(VomitSound, NPC.Center);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 phase3GoreDirection = NPC.rotation.ToRotationVector2() * (Vector2.UnitX * NPC.direction) * (NPC.width + 20) / 2f + NPC.Center;
                        int type = ModContent.ProjectileType<OldDukeGore>();
                        int totalGore = Main.getGoodWorld ? 40 : 20;
                        for (int i = 0; i < totalGore; i++)
                        {
                            float velocityX = NPC.direction * goreVelocityX * (Main.rand.NextFloat(0.2f, 0.8f) + 0.5f);
                            float velocityY = goreVelocityY * (Main.rand.NextFloat(0.2f, 0.8f) + 0.5f);

                            if (Main.getGoodWorld)
                            {
                                velocityX *= Main.rand.NextFloat() + 0.5f;
                                velocityY *= Main.rand.NextFloat() + 0.5f;
                            }

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), phase3GoreDirection.X, phase3GoreDirection.Y, velocityX, -velocityY, type, GoreDamage, 0f, Main.myPlayer, 0f, 0f);
                        }
                    }
                }

                if (NPC.ai[2] >= attackTimer - 90)
                {
                    if (NPC.ai[2] % 18f == 0f)
                    {
                        calamityGlobalNPC.newAI[2] += 150f;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float x = 900f - calamityGlobalNPC.newAI[2];
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X + x), (int)(NPC.Center.Y - calamityGlobalNPC.newAI[2]), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, NPC.whoAmI, 0f, 255);
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X - x), (int)(NPC.Center.Y - calamityGlobalNPC.newAI[2]), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, NPC.whoAmI, 0f, 255);

                            if (Main.getGoodWorld)
                            {
                                NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X + x), (int)(NPC.Center.Y - calamityGlobalNPC.newAI[2] * 0.5f), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, NPC.whoAmI, 0f, 255);
                                NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X - x), (int)(NPC.Center.Y - calamityGlobalNPC.newAI[2] * 0.5f), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, NPC.whoAmI, 0f, 255);
                            }
                        }
                    }
                }

                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= attackTimer)
                {
                    calamityGlobalNPC.newAI[0] += exhaustionIncreasePerAttack;
                    calamityGlobalNPC.newAI[2] = 0f;
                    NPC.ai[0] = 10f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.TargetClosest();
                    NPC.netUpdate = true;
                }
            }

            // Tooth Ball and Vortex spin
            else if (NPC.ai[0] == 14f)
            {
                // Play sounds and spawn Tooth Balls and a Vortex
                if (NPC.ai[2] == 0f)
                {
                    RoarSoundSlot = SoundEngine.PlaySound(RoarSound, NPC.Center);

                    SoundEngine.PlaySound(VortexSpawnSound, NPC.Center);
                    int type = ModContent.ProjectileType<OldDukeVortex>();
                    Vector2 vortexSpawn = NPC.Center + NPC.velocity.RotatedBy(MathHelper.PiOver2 * -NPC.direction) * spinTime / MathHelper.TwoPi;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), vortexSpawn, Vector2.Zero, type, VortexDamage, 0f, Main.myPlayer, vortexSpawn.X, vortexSpawn.Y);
                }

                if (NPC.ai[2] % toothBallSpinPhaseDivisor == 0f)
                {
                    if (NPC.ai[2] != 0f)
                        SoundEngine.PlaySound(VomitSound, NPC.Center);

                    Vector2 phase3ToothBallDirection = Vector2.Normalize(NPC.velocity) * (NPC.width + 20) / 2f + NPC.Center;
                    Vector2 toothBallVelocity = Vector2.Normalize(NPC.velocity).RotatedBy(MathHelper.PiOver2 * NPC.direction) * toothBallSpinToothBallVelocity;
                    Vector2 toothBallSpawnPos = new Vector2(phase3ToothBallDirection.X, phase3ToothBallDirection.Y + 45f);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int toothBall = NPC.NewNPC(NPC.GetSource_FromAI(), (int)toothBallSpawnPos.X, (int)toothBallSpawnPos.Y, ModContent.NPCType<OldDukeToothBall>(), 0, toothBallVelocity.X, toothBallVelocity.Y);
                        Main.npc[toothBall].target = NPC.target;
                        Main.npc[toothBall].velocity = Vector2.Normalize(toothBallVelocity) * toothBallSpinToothBallVelocity * 0.5f;
                        Main.npc[toothBall].netUpdate = true;
                        Main.npc[toothBall].ai[3] = 60f;
                    }

                    for (int i = 0; i < 50; i++)
                    {
                        int dustID;
                        switch (Main.rand.Next(6))
                        {
                            case 0:
                            case 1:
                            case 2:
                            case 3:
                                dustID = (int)CalamityDusts.SulphurousSeaAcid;
                                break;
                            default:
                                dustID = DustID.Blood;
                                break;
                        }

                        // Choose a random speed and angle to belch out the vomit
                        float dustSpeed = Main.rand.NextFloat(3.0f, 12.0f);
                        float angleRandom = 0.06f;
                        Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(toothBallVelocity.ToRotation());
                        dustVel = dustVel.RotatedBy(-angleRandom);
                        dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);

                        // Pick a size for the vomit particles
                        float scale = Main.rand.NextFloat(1f, 2f);

                        // Actually spawn the vomit
                        int idx = Dust.NewDust(toothBallSpawnPos, 40, 40, dustID, dustVel.X, dustVel.Y, 0, default, scale);
                        Main.dust[idx].noGravity = true;
                    }
                }

                // Velocity and rotation
                NPC.velocity = NPC.velocity.RotatedBy(-(double)spinSpeed * NPC.direction);
                NPC.rotation -= spinSpeed * NPC.direction;

                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= toothBallSpinTimer)
                {
                    calamityGlobalNPC.newAI[0] += exhaustionIncreasePerAttack;
                    NPC.ai[0] = 10f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.TargetClosest();
                    NPC.netUpdate = true;
                }
            }

            if (SoundEngine.TryGetActiveSound(RoarSoundSlot, out var roarSound) && roarSound.IsPlaying)
                roarSound.Position = NPC.Center;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
        }

        public override void ModifyTypeName(ref string typeName)
        {
            if (Main.zenithWorld)
                typeName = CalamityUtils.GetTextValue("NPCs.BoomerDuke");
        }

        public override void FindFrame(int frameHeight)
        {
            bool tired = NPC.Calamity().newAI[1] == 1f;
            if (NPC.ai[0] == 0f || NPC.ai[0] == 5f || NPC.ai[0] == 10f || NPC.ai[0] == 12f)
            {
                int frameChangeFrequency = tired ? 14 : 7;
                if (NPC.ai[0] == 5f || NPC.ai[0] == 12f)
                    frameChangeFrequency = tired ? 12 : 6;

                NPC.frameCounter += 1D;
                if (NPC.frameCounter > frameChangeFrequency)
                {
                    NPC.frameCounter = 0D;
                    NPC.frame.Y += frameHeight;
                }
                if (NPC.frame.Y >= frameHeight * 6)
                    NPC.frame.Y = 0;
            }

            if (NPC.ai[0] == 1f || NPC.ai[0] == 6f || NPC.ai[0] == 11f)
                NPC.frame.Y = frameHeight * 2;

            if (NPC.ai[0] == 2f || NPC.ai[0] == 7f || NPC.ai[0] == 14f)
                NPC.frame.Y = frameHeight * 6;

            if (NPC.ai[0] == 3f || NPC.ai[0] == 8f || NPC.ai[0] == 13f || NPC.ai[0] == -1f)
            {
                int frameChangeGateValue = 120;
                if (NPC.ai[2] < (frameChangeGateValue - 50) || NPC.ai[2] > (frameChangeGateValue - 10))
                {
                    NPC.frameCounter += 1D;
                    if (NPC.frameCounter > 7D)
                    {
                        NPC.frameCounter = 0D;
                        NPC.frame.Y += frameHeight;
                    }
                    if (NPC.frame.Y >= frameHeight * 6)
                        NPC.frame.Y = 0;
                }
                else
                {
                    NPC.frame.Y = frameHeight * 5;
                    if (NPC.ai[2] > (frameChangeGateValue - 40) && NPC.ai[2] < (frameChangeGateValue - 15))
                        NPC.frame.Y = frameHeight * 6;
                }
            }

            if (NPC.ai[0] == 4f || NPC.ai[0] == 9f)
            {
                int secondFrameChangeGateValue = 180;
                if (NPC.ai[2] < (secondFrameChangeGateValue - 60) || NPC.ai[2] > (secondFrameChangeGateValue - 20))
                {
                    NPC.frameCounter += 1D;
                    if (NPC.frameCounter > 7D)
                    {
                        NPC.frameCounter = 0D;
                        NPC.frame.Y += frameHeight;
                    }
                    if (NPC.frame.Y >= frameHeight * 6)
                        NPC.frame.Y = 0;
                }
                else
                {
                    NPC.frame.Y = frameHeight * 5;
                    if (NPC.ai[2] > (secondFrameChangeGateValue - 50) && NPC.ai[2] < (secondFrameChangeGateValue - 25))
                        NPC.frame.Y = frameHeight * 6;
                }
            }

            if (NPC.ai[0] == -1f)
            {
                if (NPC.ai[2] >= 75f)
                    NPC.frame.Y = frameHeight * 6;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                NPC.alpha = 0;
                return true;
            }

            Color finalDrawColor = Color.Lerp(drawColor, Color.White, NuclearOverlayVisual);
            Color overlayDrawColor = Color.Lerp(Color.Transparent, Color.LimeGreen.MultiplyRGBA(new Color(255, 255, 255, 0)), NuclearOverlayVisual);
            finalDrawColor = Color.Lerp(finalDrawColor, Color.LimeGreen.MultiplyRGBA(new Color(255, 255, 255, 0)), NuclearOverlayVisual);

            finalDrawColor = Color.Lerp(finalDrawColor, Color.Transparent, (float)NPC.alpha / 255f);
            overlayDrawColor = Color.Lerp(overlayDrawColor, Color.Transparent, (float)NPC.alpha / 255f);

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Texture2D texture2D15 = TextureAssets.Npc[Type].Value;
            Vector2 halfSizeTexture = new Vector2(texture2D15.Width / 2, texture2D15.Height / Main.npcFrameCount[Type] / 2);
            Color color = drawColor;
            Color drawLerpColor = Color.White;
            float drawLerpValue = 0f;
            bool halfTiredBuffColor = NPC.ai[0] > 4f;
            bool tiredBuffColor = NPC.ai[0] > 9f && NPC.ai[0] <= 12f;
            int ai2Compare = 120;
            int buffColorDivisor = 60;
            if (tiredBuffColor)
            {
                color = CalamityGlobalNPC.buffColor(color, 0.4f, 0.8f, 0.4f, 1f);
            }
            else if (halfTiredBuffColor)
            {
                color = CalamityGlobalNPC.buffColor(color, 0.5f, 0.7f, 0.5f, 1f);
            }
            else if (NPC.ai[0] == 4f && NPC.ai[2] > ai2Compare)
            {
                float buffColorDampener = NPC.ai[2] - ai2Compare;
                buffColorDampener /= buffColorDivisor;
                color = CalamityGlobalNPC.buffColor(color, 1f - 0.5f * buffColorDampener, 1f - 0.3f * buffColorDampener, 1f - 0.5f * buffColorDampener, 1f);
            }

            int afterimageAmt = 10;
            int afterimageIncrement = 2;
            if (NPC.ai[0] == -1f)
            {
                afterimageAmt = 0;
            }
            if (NPC.ai[0] == 0f || NPC.ai[0] == 5f || NPC.ai[0] == 10f || NPC.ai[0] == 12f)
            {
                afterimageAmt = 7;
            }
            if (NPC.ai[0] == 1f || NPC.ai[0] == 6f || NPC.ai[0] > 9f)
            {
                drawLerpColor = Color.Lime;
                drawLerpValue = 0.5f;
            }
            else
            {
                color = drawColor;
            }

            if (CalamityClientConfig.Instance.Afterimages)
            {
                for (int i = 1; i < afterimageAmt; i += afterimageIncrement)
                {
                    Color afterimageColor = color;
                    afterimageColor = Color.Lerp(afterimageColor, drawLerpColor, drawLerpValue);
                    afterimageColor = Color.Lerp(afterimageColor, overlayDrawColor, NuclearOverlayVisual);
                    afterimageColor = NPC.GetAlpha(afterimageColor);
                    afterimageColor *= (afterimageAmt - i) / 15f;
                    Vector2 afterimagePos = NPC.oldPos[i] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    afterimagePos -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[Type]) * NPC.scale / 2f;
                    afterimagePos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, afterimagePos, NPC.frame, afterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                }
            }

            int secondAfterimageAmt = 0;
            float afterimageOpacity = 0f;
            float afterimageScale = 0f;

            if (NPC.ai[0] == -1f)
                secondAfterimageAmt = 0;

            if (NPC.ai[0] == 3f || NPC.ai[0] == 8f || NPC.ai[0] == 13f)
            {
                if (NPC.ai[2] > 60)
                {
                    secondAfterimageAmt = 6;
                    afterimageOpacity = 1f - (float)Math.Cos((NPC.ai[2] - 60) / 30 * MathHelper.TwoPi);
                    afterimageOpacity /= 3f;
                    afterimageScale = 40f;
                }
            }

            if ((NPC.ai[0] == 4f || NPC.ai[0] == 9f) && NPC.ai[2] > ai2Compare)
            {
                secondAfterimageAmt = 6;
                afterimageOpacity = 1f - (float)Math.Cos((NPC.ai[2] - ai2Compare) / buffColorDivisor * MathHelper.TwoPi);
                afterimageOpacity /= 3f;
                afterimageScale = 60f;
            }

            if (NPC.ai[0] == 12f)
            {
                secondAfterimageAmt = 6;
                afterimageOpacity = 1f - (float)Math.Cos(NPC.ai[2] / 30f * MathHelper.TwoPi);
                afterimageOpacity /= 3f;
                afterimageScale = 20f;
            }

            if (CalamityClientConfig.Instance.Afterimages)
            {
                for (int j = 0; j < secondAfterimageAmt; j++)
                {
                    Color secondAfterimageColor = Color.Lerp(finalDrawColor, Color.Transparent, (float)j / (float)secondAfterimageAmt);
                    Vector2 secondAfterimagePos = NPC.Center + (j / (float)secondAfterimageAmt * MathHelper.TwoPi + NPC.rotation).ToRotationVector2() * afterimageScale * afterimageOpacity - screenPos;
                    secondAfterimagePos -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[Type]) * NPC.scale / 2f;
                    secondAfterimagePos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, secondAfterimagePos, NPC.frame, secondAfterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[Type]) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            drawLocation += new Vector2(Main.rand.NextFloat(-shake, shake), Main.rand.NextFloat(-shake, shake));

            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, NPC.GetAlpha(finalDrawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, NPC.GetAlpha(overlayDrawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            float auraOutset = 6f + (float)(Math.Sin(VisualTimerSystem.GlobalVisualTimer / 10f) * 10f);
            for (float i = 0f; i < 360f; i += 90f)
                spriteBatch.Draw(texture2D15, drawLocation + new Vector2(auraOutset, 0).RotatedBy(MathHelper.ToRadians(i)), NPC.frame, NPC.GetAlpha(overlayDrawColor.MultiplyRGBA(new Color(0.4f, 0.4f, 0.4f, 0.4f))), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            if (NPC.ai[0] >= 4f && NPC.Calamity().newAI[1] != 1f)
            {
                texture2D15 = GlowTexture.Value;
                Color yellowLerpColor = Color.Lerp(Color.White, Color.Yellow, 0.5f);
                drawLerpColor = Color.Yellow;

                drawLerpValue = 1f;
                afterimageOpacity = 0.5f;
                afterimageScale = 10f;
                afterimageIncrement = 1;

                if (NPC.ai[0] == 4f || NPC.ai[0] == 9f)
                {
                    float otherAfterimageOpacity = NPC.ai[2] - ai2Compare;
                    otherAfterimageOpacity /= buffColorDivisor;
                    drawLerpColor *= otherAfterimageOpacity;
                    yellowLerpColor *= otherAfterimageOpacity;
                }

                if (NPC.ai[0] == 12f)
                {
                    float ai2Opacity = NPC.ai[2];
                    ai2Opacity /= 30f;
                    if (ai2Opacity > 0.5f)
                    {
                        ai2Opacity = 1f - ai2Opacity;
                    }
                    ai2Opacity *= 2f;
                    ai2Opacity = 1f - ai2Opacity;
                    drawLerpColor *= ai2Opacity;
                    yellowLerpColor *= ai2Opacity;
                }

                if (CalamityClientConfig.Instance.Afterimages)
                {
                    for (int k = 1; k < afterimageAmt; k += afterimageIncrement)
                    {
                        Color yellowAfterimageColor = yellowLerpColor;
                        yellowAfterimageColor = Color.Lerp(yellowAfterimageColor, drawLerpColor, drawLerpValue);
                        yellowAfterimageColor *= (afterimageAmt - k) / 15f;
                        Vector2 yellowAfterimagePos = NPC.oldPos[k] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                        yellowAfterimagePos -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[Type]) * NPC.scale / 2f;
                        yellowAfterimagePos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                        spriteBatch.Draw(texture2D15, yellowAfterimagePos, NPC.frame, yellowAfterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                    }

                    for (int l = 1; l < secondAfterimageAmt; l++)
                    {
                        Color secondYellowAfterimageColor = yellowLerpColor;
                        secondYellowAfterimageColor = Color.Lerp(secondYellowAfterimageColor, drawLerpColor, drawLerpValue);
                        secondYellowAfterimageColor = NPC.GetAlpha(secondYellowAfterimageColor);
                        secondYellowAfterimageColor *= 1f - afterimageOpacity;
                        Vector2 secondYellowAfterimagePos = NPC.Center + (l / (float)secondAfterimageAmt * MathHelper.TwoPi + NPC.rotation).ToRotationVector2() * afterimageScale * afterimageOpacity - screenPos;
                        secondYellowAfterimagePos -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[Type]) * NPC.scale / 2f;
                        secondYellowAfterimagePos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                        spriteBatch.Draw(texture2D15, secondYellowAfterimagePos, NPC.frame, secondYellowAfterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                    }
                }

                spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, yellowLerpColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
            }

            return false;
        }

        public override void BossLoot(ref int potionType)
        {
            potionType = ModContent.ItemType<SupremeHealingPotion>();
        }

        public override void OnKill()
        {
            // Don't bother running any of this in Boss Rush.
            if (BossRushEvent.BossRushActive)
                return;

            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            CalamityGlobalTownNPC.SetNewShopVariable(new int[] { ModContent.NPCType<SeaKing>() }, DownedBossSystem.downedBoomerDuke);

            // Mark Old Duke as dead
            DownedBossSystem.downedBoomerDuke = true;

            // Mark first acid rain encounter as true even if he wasn't fought in the acid rain, because it makes sense
            AcidRainEvent.OldDukeHasBeenEncountered = true;
            CalamityNetcode.SyncWorld();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<OldDukeBag>()));

            // Normal drops: Everything that would otherwise be in the bag
            var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            {
                // Weapons and such
                int[] items = new int[]
                {
                    ModContent.ItemType<InsidiousImpaler>(),
                    ModContent.ItemType<FetidEmesis>(),
                    ModContent.ItemType<SepticSkewer>(),
                    ModContent.ItemType<VitriolicViper>(),
                    ModContent.ItemType<MutatedTruffle>(),
                    ModContent.ItemType<CadaverousCarrion>(),
                    ModContent.ItemType<ToxicantTwister>()
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, items));
                normalOnly.Add(ModContent.ItemType<TheOldReaper>(), 10);

                // Equipment
                // 16NOV2025: Ozzatron: item has been chosen as the "Expert gatekept" item for this Calamity boss
                // normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<OldDukeScales>()));

                // Vanity
                normalOnly.Add(ModContent.ItemType<OldDukeMask>(), 7);
                normalOnly.Add(ModContent.ItemType<EldenDiorama>(), 10);
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
            }

            npcLoot.Add(ModContent.ItemType<OldDukeTrophy>(), 10);

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<OldDukeRelic>());

            // GFB Shattered Community drop
            npcLoot.DefineConditionalDropSet(DropHelper.GFB).Add(DropHelper.PerPlayer(ModContent.ItemType<ShatteredCommunity>()), hideLootReport: true);

            // Lore
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedBoomerDuke, ModContent.ItemType<LoreOldDuke>(), desc: DropHelper.FirstKillText);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses;
            return true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
            {
                target.AddBuff(ModContent.BuffType<Irradiated>(), Phase == 2 ? 600 : Phase == 1 ? 480 : 360);
                target.AddBuff(ModContent.BuffType<HeavyBleeding>(), Phase == 2 ? 300 : Phase == 1 ? 240 : 180);
                if (Main.zenithWorld)
                {
                    target.AddBuff(BuffID.Rabies, Main.rand.Next(180, 601));
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life > 0)
            {
            }
            else
            {
                for (int r = 0; r < 150; r++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulphurousSeaAcid, 2 * hit.HitDirection, -2f, 0, default, 1f);
                }

                if (!Main.dedServ)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center + Vector2.UnitX * 20f * NPC.direction, NPC.velocity, Mod.Find<ModGore>("OldDukeGore").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center + Vector2.UnitX * 20f * NPC.direction, NPC.velocity, Mod.Find<ModGore>("OldDukeGore2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center - Vector2.UnitX * 20f * NPC.direction, NPC.velocity, Mod.Find<ModGore>("OldDukeGore3").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center - Vector2.UnitX * 20f * NPC.direction, NPC.velocity, Mod.Find<ModGore>("OldDukeGore4").Type, NPC.scale);
                }
            }
        }
    }
}
