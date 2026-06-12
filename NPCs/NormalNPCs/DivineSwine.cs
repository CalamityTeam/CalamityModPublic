using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod.Effects;
using CalamityMod.Enums;
using CalamityMod.Graphics.Metaballs;
using CalamityMod.Items.Ammo;
using CalamityMod.Items.Critters;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Potions.Food;
using CalamityMod.Packets;
using CalamityMod.Particles;
using CalamityMod.Systems.Graphic.PixelationSystem;
using CalamityMod.Utilities.Daybreak;
using CalamityMod.Utilities.Daybreak.Buffers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class DivineSwine : ModNPC
    {
        public enum BehaviorState
        {
            PiggyTransformation = -1,
            IdleAndFly,
            OfferingAccepted,
            OfferingFailed
        }

        private static Asset<Texture2D> BloomCircle;
        private static Asset<Texture2D> BloomFlare;
        private static Asset<Texture2D> ShineFlare;
        private static Asset<Texture2D> MagicStarCircle;
        private static Asset<Texture2D> FadedStarRing;
        private static Asset<Texture2D> DistortionTexture;
        private static Asset<Texture2D> DeliciousMeatTexture;

        private static SoundStyle IdleSound = new("CalamityMod/Sounds/Custom/DivineSwine/DivineSwineIdle", 4);
        private static SoundStyle CoinFailSound = new("CalamityMod/Sounds/Custom/DivineSwine/DivineSwineCoinFail", 3);
        private static SoundStyle CoinPassSound = new("CalamityMod/Sounds/Custom/DivineSwine/DivineSwineCoinPass", 2);
        private static SoundStyle SwineSpeakLoopingSound = new("CalamityMod/Sounds/Custom/DivineSwine/DivineSwineNearbyLoop")
        {
            IsLooped = true,
            PauseBehavior = PauseBehavior.PauseWithGame,
            MaxInstances = 0,
        };

        public int FrameY;

        public bool ShouldTurnAway;

        public Vector2 SquashVector;

        public Vector2 IdleMovementVector;

        public SlotId SwineSpeakSoundSlot;

        public static float MaxSpeed => 0.38f;
        public static float MaxAcceleration => 0.06f;

        public static Color DivineBlue => new(166, 238, 247);
        public static Color DivineYellow => new(247, 242, 166);

        public ref float Timer => ref NPC.ai[0];

        public ref float AIState => ref NPC.ai[1];

        public ref float LocalAIState => ref NPC.ai[2];

        public ref float IdleMovementTimer => ref NPC.ai[3];

        public ref float AdditionalBrightness => ref NPC.localAI[0];

        public ref float GlowingVisualScale => ref NPC.localAI[1];

        public ref float DivineSwineTintStrength => ref NPC.localAI[2];

        public override void Load()
        {
            On_Main.HoverOverNPCs += DivineSwineRightClickInteraction;
            On_Main.DrawInfoAccs_AdjustInfoTextColorsForNPC += AdjustLifeformAnalyzerTextColor;

            if (!Main.dedServ)
            {
                BloomCircle = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle");
                BloomFlare = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/BloomFlare");
                ShineFlare = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/ShineFlare");
                MagicStarCircle = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/MagicStarCircle");
                FadedStarRing = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/FadedStarRing");
                DistortionTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/Smudges");
                DeliciousMeatTexture = ModContent.Request<Texture2D>("CalamityMod/Items/Potions/Food/DeliciousMeat");
            }
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 14;
            NPCID.Sets.CountsAsCritter[Type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.NormalGoldCritterBestiaryPriority.Insert(NPCID.Sets.NormalGoldCritterBestiaryPriority.IndexOf(NPCID.GoldBunny) + 3, Type);
        }

        public override void SetDefaults()
        {
            NPC.damage = 0;
            NPC.width = 32;
            NPC.height = 34;
            NPC.lifeMax = 5;
            NPC.defense = 99999999;
            NPC.rarity = 5;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0.1f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.immortal = true;
            NPC.noGravity = true;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SquashVector = Vector2.One;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.DivineSwine")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(FrameY);
            writer.Write(ShouldTurnAway);
            writer.WriteVector2(SquashVector);
            writer.WriteVector2(IdleMovementVector);

            for (int i = 0; i < 3; i++)
                writer.Write(NPC.localAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            FrameY = reader.ReadInt32();
            ShouldTurnAway = reader.ReadBoolean();
            SquashVector = reader.ReadVector2();
            IdleMovementVector = reader.ReadVector2();

            for (int i = 0; i < 3; i++)
                NPC.localAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            switch ((BehaviorState)AIState)
            {
                case BehaviorState.PiggyTransformation:
                    MainBehavior_PiggyTransformation();
                    break;

                case BehaviorState.IdleAndFly:
                    MainBehavior_IdleAndFly();
                    break;

                case BehaviorState.OfferingAccepted:
                    MainBehavior_OfferingAccepted();
                    break;

                case BehaviorState.OfferingFailed:
                    MainBehavior_OfferingFailed();
                    break;
            }

            SoundEffects();
            AmbientParticles();
            AnimateSprite();

            Lighting.AddLight(NPC.Center, DivineYellow.ToVector3() * NPC.scale * 0.825f);

            if (AIState != (int)BehaviorState.PiggyTransformation && AIState != (int)BehaviorState.OfferingAccepted && AIState != (int)BehaviorState.OfferingFailed)
            {
                AdditionalBrightness = MathHelper.Lerp(AdditionalBrightness, 0f, 0.05f);
                GlowingVisualScale = MathHelper.Lerp(GlowingVisualScale, 1f, 0.05f);
            }

            SquashVector = Vector2.Lerp(SquashVector, Vector2.One, 0.05f);
            ShouldTurnAway = NPC.ArcCollisionCheck(-0.3f, 0.3f, 0.05f, optionalCollisionCheckOverride: (arcBasePoint, arcPoint) =>
            {
                return !Collision.CanHitLine(arcBasePoint, 1, 1, arcPoint, 1, 1) || Collision.WetCollision(arcPoint, 1, 1);
            });

            NPC.rotation = NPC.velocity.X * 0.12f;
            Timer++;
        }

        public void MainBehavior_PiggyTransformation()
        {
            NPC.velocity *= 0f;
            NPC.ShowNameOnHover = false;
            NPC.dontTakeDamage = true;

            // Shrink the sigil and start tinting white.
            float dissolveStrength = CalamityUtils.SineInOutEasing(Utils.GetLerpValue(0f, 240f, Timer, true), 1);
            DivineSwineTintStrength = dissolveStrength;
            GlowingVisualScale = 1f - dissolveStrength;
            AdditionalBrightness = MathHelper.Lerp(0f, 50f, dissolveStrength);

            // Shake the screen slightly while the player is near Divine Swine.
            float distanceFromSwine = Main.LocalPlayer.Distance(NPC.Center);
            if (distanceFromSwine < 400f)
            {
                float shakeStrength = Utils.Remap(distanceFromSwine, 400f, 100f, 0f, 3f, true) * Utils.GetLerpValue(0f, 180f, Timer, true);
                Main.LocalPlayer.SetScreenshake(shakeStrength);
            }

            // Typical glowy particle VFX.
            float particleStrengthInterpolant = Utils.GetLerpValue(0f, 180f, Timer, true);
            int lightSpawnRate = (int)MathHelper.Lerp(8, 1, particleStrengthInterpolant);
            if (Main.rand.NextBool(lightSpawnRate))
            {
                int lightAmt = (int)MathHelper.Lerp(1, 6, particleStrengthInterpolant);
                for (int i = 0; i < Main.rand.Next(1, lightAmt + 1); i++)
                {
                    float lightSpeed = MathHelper.Lerp(6f, 12f, particleStrengthInterpolant);
                    float lightScale = MathHelper.Lerp(0.5f, 0.8f, particleStrengthInterpolant) * Main.rand.NextFloat(0.5f, 1f);

                    SquishyLightParticle transformLight = new(NPC.Center, Main.rand.NextVector2Circular(lightSpeed, lightSpeed), lightScale, Color.White, Main.rand.Next(30, 45));
                    GeneralParticleHandler.SpawnParticle(transformLight, true, Enums.GeneralDrawLayer.BeforeNPCs);
                }
            }

            if (Main.rand.NextBool(lightSpawnRate + 2))
            {
                int sparkleAmt = (int)MathHelper.Lerp(1, 5, particleStrengthInterpolant);
                for (int i = 0; i < Main.rand.Next(1, sparkleAmt + 1); i++)
                {
                    float sparkleSpeed = MathHelper.Lerp(8f, 18f, particleStrengthInterpolant);
                    float sparkleScale = MathHelper.Lerp(0.2f, 0.5f, particleStrengthInterpolant) * Main.rand.NextFloat(0.5f, 1f);
                    Color drawColorBlue = Color.Lerp(new Color(44, 166, 247), new Color(123, 197, 247), Main.rand.NextFloat());
                    Color drawColorYellow = Color.Lerp(new Color(249, 197, 42), new Color(249, 221, 142), Main.rand.NextFloat());
                    Color sparkleColor = Utils.SelectRandom(Main.rand, drawColorBlue, drawColorYellow);

                    QuickSparkleParticle transformSparkle = new(NPC.Center, Main.rand.NextVector2Circular(sparkleSpeed, sparkleSpeed), sparkleColor, sparkleScale, Main.rand.Next(30, 45));
                    GeneralParticleHandler.SpawnParticle(transformSparkle, true, Enums.GeneralDrawLayer.BeforeNPCs);
                }
            }

            if (Timer >= 180f && Timer % 15f == 0f)
            {
                float glowRingScale = Utils.Remap(Timer, 180f, 240f, 1.6f, 2.2f, true);
                CustomPulse transformRing = new(NPC.Center, Vector2.Zero, Color.White, "CalamityMod/Particles/BloomRing", Vector2.One, 0f, 0f, glowRingScale, 45);
                GeneralParticleHandler.SpawnParticle(transformRing, false, Enums.GeneralDrawLayer.AfterNPCs);
            }

            if (Timer >= 240f)
            {
                CustomPulse glowRing = new(NPC.Center, Vector2.Zero, Color.White, "CalamityMod/Particles/BloomRing", Vector2.One, 0f, 0f, 3f, 75);
                GeneralParticleHandler.SpawnParticle(glowRing, false, Enums.GeneralDrawLayer.AfterNPCs);

                BloomParticle bloom = new(NPC.Center, Vector2.Zero, Color.White, 0f, 2f, 125);
                for (int i = 0; i < 3; i++)
                    GeneralParticleHandler.SpawnParticle(bloom, false, Enums.GeneralDrawLayer.AfterNPCs);

                for (int i = 0; i < 25; i++)
                {
                    QuickSparkleParticle sparkle = new(NPC.Center, Main.rand.NextVector2Circular(20f, 20f), Color.White, Main.rand.NextFloat(0.6f, 0.8f), Main.rand.Next(45, 60));
                    SquishyLightParticle light = new(NPC.Center, Main.rand.NextVector2Circular(10f, 10f), Main.rand.NextFloat(0.6f, 0.8f), Color.White, Main.rand.Next(45, 60));
                    GeneralParticleHandler.SpawnParticle(Main.rand.NextBool() ? sparkle : light, true, Enums.GeneralDrawLayer.AfterNPCs);
                }

                SoundEngine.PlaySound(SoundID.Item29 with { Volume = 0.1f }, NPC.Center);
                SoundEngine.PlaySound(IdleSound, NPC.Center);
                if (SoundEngine.TryGetActiveSound(SwineSpeakSoundSlot, out var activeSound))
                    activeSound.Stop();

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.Transform(ModContent.NPCType<Piggy>());
                    NPC.netUpdate = true;
                }
            }
        }

        public void MainBehavior_IdleAndFly()
        {
            // Flying around.
            if (LocalAIState == 0f)
            {
                if (Timer > 0f && Timer % 120f == 0f && Main.rand.NextBool(6))
                {
                    Timer = 0f;
                    LocalAIState = 1f;
                    NPC.netUpdate = true;
                }

                Point tileAhead = (NPC.Center + NPC.velocity * 16f).ToTileCoordinates();
                bool leavingWorldTop = tileAhead.Y <= 40;
                bool leavingWorldBottom = tileAhead.Y >= Main.maxTilesY - 40;
                bool leavingWorldLeft = tileAhead.X <= 40;
                bool leavingWorldRight = tileAhead.X >= Main.maxTilesX - 40;
                if (ShouldTurnAway || leavingWorldTop || leavingWorldBottom || leavingWorldLeft || leavingWorldRight)
                {
                    AvoidTileCollision(MaxSpeed + 0.6f, ref leavingWorldTop, ref leavingWorldBottom, ref leavingWorldLeft, ref leavingWorldRight);
                    IdleMovementTimer = 150f;
                }
                else
                {
                    if (IdleMovementTimer == 0f)
                    {
                        IdleMovementVector = new Vector2(Main.rand.NextFloat(-100f, 101f));
                        IdleMovementTimer = Main.rand.Next(240, 360);
                        NPC.netUpdate = true;
                    }

                    float idealSpeed = MaxSpeed / IdleMovementVector.Length();
                    NPC.velocity = Vector2.Lerp(NPC.velocity, IdleMovementVector * idealSpeed, MaxAcceleration);
                    IdleMovementTimer--;

                    if (NPC.velocity.Y < 2.2f)
                        NPC.velocity.Y += 0.022f;
                }

                NPC.spriteDirection = (NPC.velocity.X > 0).ToDirectionInt();
            }

            // Stop and occasionally switch directions.
            if (LocalAIState == 1f)
            {
                if (Timer > 0f && Timer % 120f == 0f && Main.rand.NextBool(6))
                {
                    Timer = 0f;
                    LocalAIState = 0f;
                    NPC.netUpdate = true;
                }

                NPC.velocity *= 0.9f;
                if (Timer > 0f && Timer % 75f == 0f && Main.rand.NextBool(4))
                    NPC.spriteDirection *= -1;
            }
        }

        public void MainBehavior_OfferingAccepted()
        {
            // Spawn particles and do a lil animation and whateva
            if (Timer <= 210f)
            {
                float lightSpawnDistance = MathHelper.Lerp(52f, 84f, Timer / 210f);
                float glowRingScale = MathHelper.Lerp(0.3f, 0.9f, Timer / 210f);

                int lightAmt = Main.rand.Next(1, 2);
                for (int i = 0; i < lightAmt; i++)
                {
                    Vector2 lightSpawnPosition = NPC.Center + Main.rand.NextVector2Unit() * lightSpawnDistance * Main.rand.NextFloat(0.7f, 1f);
                    Vector2 lightVelocity = (NPC.Center - lightSpawnPosition).SafeNormalize(Vector2.UnitY) * Main.rand.NextFloat(3f, 4f);
                    float lightScale = Utils.Remap(Timer, 0f, 180f, 0.3f, 0.9f, true);
                    Color lightColor = Color.Lerp(DivineBlue, DivineYellow, Main.rand.NextFloat());

                    SquishyLightParticle meatLight = new(lightSpawnPosition, lightVelocity, lightScale, lightColor, Main.rand.Next(30, 45));
                    GeneralParticleHandler.SpawnParticle(meatLight, true, Enums.GeneralDrawLayer.BeforeNPCs);
                }

                int sparkleSpawnRate = (int)Utils.Remap(Timer, 0f, 180f, 45, 5, true);
                if (Main.rand.NextBool(sparkleSpawnRate))
                {
                    int sparkleAmt = Main.rand.Next(1, 3);
                    for (int i = 0; i < sparkleAmt; i++)
                    {
                        Vector2 spawnPosition = NPC.Center + Main.rand.NextVector2Unit() * lightSpawnDistance * Main.rand.NextFloat(0.8f, 1.2f);
                        Color drawColorBlue = Color.Lerp(new Color(44, 166, 247), new Color(123, 197, 247), Main.rand.NextFloat());
                        Color drawColorYellow = Color.Lerp(new Color(249, 197, 42), new Color(249, 221, 142), Main.rand.NextFloat());
                        Color sparkleColor = Utils.SelectRandom(Main.rand, drawColorBlue, drawColorYellow);
                        float sparkleScale = Utils.Remap(Timer, 0f, 180f, 0.2f, 0.8f, true);

                        QuickSparkleParticle sparkle = new(spawnPosition, Vector2.Zero, sparkleColor, sparkleScale * Main.rand.NextFloat(0.5f, 1f), Main.rand.Next(20, 30));
                        GeneralParticleHandler.SpawnParticle(sparkle, true);
                    }
                }

                if (Timer % 15f == 0f)
                {
                    CustomPulse meatLightRing = new(NPC.Center, Vector2.Zero, DivineBlue, "CalamityMod/Particles/BloomRing", Vector2.One, 0f, glowRingScale, 0f, 25);
                    GeneralParticleHandler.SpawnParticle(meatLightRing, true, Enums.GeneralDrawLayer.BeforeNPCs);
                }
            }

            // Divine meat granted
            if (Timer == 210f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int meat = Item.NewItem(NPC.GetSource_GiftOrReward(), NPC.Center, ModContent.ItemType<DeliciousMeat>());
                    Main.item[meat].velocity = Vector2.UnitY * -3f;
                }

                CustomPulse meatLightRing = new(NPC.Center, Vector2.Zero, DivineBlue, "CalamityMod/Particles/BloomRing", Vector2.One, 0f, 0f, 3f, 75);
                GeneralParticleHandler.SpawnParticle(meatLightRing, false, Enums.GeneralDrawLayer.AfterNPCs);

                BloomParticle meatBloom = new(NPC.Center, Vector2.Zero, Color.White, 0f, 3f, 125);
                for (int i = 0; i < 3; i++)
                    GeneralParticleHandler.SpawnParticle(meatBloom, false, Enums.GeneralDrawLayer.AfterNPCs);

                for (int i = 0; i < 25; i++)
                {
                    Color drawColorBlue = Color.Lerp(new Color(44, 166, 247), new Color(123, 197, 247), Main.rand.NextFloat());
                    Color drawColorYellow = Color.Lerp(new Color(249, 197, 42), new Color(249, 221, 142), Main.rand.NextFloat());
                    Color sparkleColor = Utils.SelectRandom(Main.rand, drawColorBlue, drawColorYellow);
                    QuickSparkleParticle meatSparkle = new(NPC.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(13f, 15f), sparkleColor, Main.rand.NextFloat(0.6f, 0.8f), Main.rand.Next(45, 60));
                    
                    Color lightColor = Color.Lerp(DivineBlue, DivineYellow, Main.rand.NextFloat());
                    SquishyLightParticle meatLight = new(NPC.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(3f, 5f), Main.rand.NextFloat(1.4f, 1.8f), lightColor, Main.rand.Next(45, 60));
                    
                    GeneralParticleHandler.SpawnParticle(Main.rand.NextBool() ? meatSparkle : meatLight, true, Enums.GeneralDrawLayer.AfterNPCs);
                }

                // Despawn immediately afterwards.
                NPC.checkDead();
                NPC.active = false;
            }

            GlowingVisualScale = Utils.Remap(Timer, 0f, 210f, 1f, 0f, true);
            AdditionalBrightness = Utils.Remap(Timer, 0f, 45f, 1f, 0f, true);
            NPC.scale *= 0.96f;
            NPC.velocity *= 0.9f;
            NPC.ShowNameOnHover = false;
        }

        public void MainBehavior_OfferingFailed()
        {
            if (Timer >= 75f)
            {
                CustomPulse meatLightRing = new(NPC.Center, Vector2.Zero, Color.White, "CalamityMod/Particles/BloomRing", Vector2.One, 0f, 0f, 2f, 75);
                GeneralParticleHandler.SpawnParticle(meatLightRing, false, Enums.GeneralDrawLayer.AfterNPCs);

                BloomParticle meatBloom = new(NPC.Center, Vector2.Zero, Color.White, 0f, 2f, 125);
                for (int i = 0; i < 3; i++)
                    GeneralParticleHandler.SpawnParticle(meatBloom, false, Enums.GeneralDrawLayer.AfterNPCs);

                for (int i = 0; i < 25; i++)
                {
                    QuickSparkleParticle meatSparkle = new(NPC.Center, Main.rand.NextVector2Circular(15f, 15f), Color.White, Main.rand.NextFloat(0.6f, 0.8f), Main.rand.Next(45, 60));
                    SquishyLightParticle meatLight = new(NPC.Center, Main.rand.NextVector2Circular(5f, 5f), Main.rand.NextFloat(0.6f, 0.8f), Color.White, Main.rand.Next(45, 60));

                    GeneralParticleHandler.SpawnParticle(Main.rand.NextBool() ? meatSparkle : meatLight, true, Enums.GeneralDrawLayer.AfterNPCs);
                }

                SoundEngine.PlaySound(SoundID.Item29 with { Volume = 0.1f }, NPC.Center);
                SoundEngine.PlaySound(IdleSound, NPC.Center);

                NPC.checkDead();
                NPC.active = false;
                return;
            }

            if (Timer <= 45f)
            {
                DivineSwineTintStrength = MathHelper.Lerp(0f, 1f, CalamityUtils.PolyInOutEasing(Timer / 45f, 2));
                GlowingVisualScale = MathHelper.Lerp(1f, 0f, CalamityUtils.PolyInOutEasing(Timer / 45f, 2));
            }

            int lightAmt = Main.rand.Next(2, 3);
            for (int i = 0; i < lightAmt; i++)
            {
                float lightScale = Main.rand.NextFloat(0.15f, 0.3f);
                SquishyLightParticle meatLight = new(NPC.Center, Main.rand.NextVector2Circular(2f, 2f), lightScale, Color.White, Main.rand.Next(30, 45));
                GeneralParticleHandler.SpawnParticle(meatLight, true, Enums.GeneralDrawLayer.BeforeNPCs);
            }

            NPC.velocity *= 0.9f;
            NPC.ShowNameOnHover = false;
        }

        public void TransformIntoPiggy()
        {
            AIState = (int)BehaviorState.PiggyTransformation;
            LocalAIState = 0f;
            Timer = 0f;
            NPC.netUpdate = true;
        }
        
        public bool TryTransformingIntoPiggy()
        {
            if (AIState == (int)BehaviorState.PiggyTransformation)
                return false;
            return true;
        }

        public void AvoidTileCollision(float maxSpeed, ref bool leavingWorldTop, ref bool leavingWorldBottom, ref bool leavingWorldLeft, ref bool leavingWorldRight, float turnAwayStrength = 0.125f)
        {
            float distanceToCollisionLeft = CalamityUtils.DistanceToTileCollisionHit(NPC.Center, NPC.velocity.RotatedBy(MathHelper.PiOver2), 32, ShouldAvoidTile) ?? 10000f;
            float distanceToCollisionRight = CalamityUtils.DistanceToTileCollisionHit(NPC.Center, NPC.velocity.RotatedBy(-MathHelper.PiOver2), 32, ShouldAvoidTile) ?? 10000f;
            int directionToMove = (distanceToCollisionLeft > distanceToCollisionRight).ToDirectionInt();

            Vector2 idealVelocity = NPC.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2 * directionToMove) * (maxSpeed - NPC.velocity.Length());
            if (leavingWorldTop)
                idealVelocity = Vector2.UnitY * 2f;
            if (leavingWorldBottom)
                idealVelocity = Vector2.UnitY * -2f;
            if (leavingWorldLeft)
                idealVelocity = Vector2.UnitX * 2f;
            if (leavingWorldRight)
                idealVelocity = Vector2.UnitX * -2f;

            IdleMovementVector = idealVelocity;
            NPC.velocity = Vector2.Lerp(NPC.velocity, idealVelocity, turnAwayStrength);
        }

        public void AmbientParticles()
        {
            if (AIState != (int)BehaviorState.IdleAndFly)
                return;

            // Feathers, both falling and non falling ones.
            if (Main.rand.NextBool(70))
            {
                int featherAmt = Main.rand.Next(1, 3);
                for (int i = 0; i < featherAmt; i++)
                {
                    Vector2 spawnPosition = NPC.Center + Main.rand.NextVector2Circular(NPC.width, NPC.height);
                    Vector2 velocity = Vector2.UnitY * Main.rand.NextFloat(0.25f, 0.75f);
                    Color featherColor = Color.Lerp(Color.Lerp(new Color(27, 103, 155), new Color(83, 184, 255), Main.rand.NextFloat()), new Color(221, 253, 255), Main.rand.NextFloat(0.4f));
                    float scale = Main.rand.NextFloat(0.8f, 1f);
                    FeatherParticle feather = new(spawnPosition, velocity, featherColor, scale, Main.rand.Next(360, 480), null, 0.8f, true, false, true);

                    GeneralDrawLayer featherLayer = Utils.SelectRandom(Main.rand, GeneralDrawLayer.BeforeNPCs, GeneralDrawLayer.AfterNPCs);
                    GeneralParticleHandler.SpawnParticle(feather, manualDrawLayerOverride: featherLayer);
                }
            }

            // Small sparkles.
            if (Main.rand.NextBool(45))
            {
                int sparkleAmt = Main.rand.Next(1, 3);
                for (int i = 0; i < sparkleAmt; i++)
                {
                    Vector2 spawnPosition = NPC.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(80f, 160f);
                    Color drawColorBlue = Color.Lerp(new Color(44, 166, 247), new Color(123, 197, 247), Main.rand.NextFloat());
                    Color drawColorYellow = Color.Lerp(new Color(249, 197, 42), new Color(249, 221, 142), Main.rand.NextFloat());
                    Color sparkleColor = Utils.SelectRandom(Main.rand, drawColorBlue, drawColorYellow);

                    QuickSparkleParticle sparkle = new(spawnPosition, Vector2.Zero, sparkleColor, Main.rand.NextFloat(0.2f, 0.3f), Main.rand.Next(30, 45));
                    GeneralParticleHandler.SpawnParticle(sparkle, true);
                }
            }
        }

        public void AnimateSprite()
        {
            if (Timer % 5 == 0f)
            {
                FrameY++;
                if (FrameY >= Main.npcFrameCount[Type] - 1)
                    FrameY = 0;

                if (FrameY == 5)
                    NPC.velocity.Y -= (NPC.velocity.X != 0f) ? 2.2f : 0.46f;
            }
        }

        public void SoundEffects()
        {
            if (NPC.soundDelay == 0 && Main.rand.NextBool(200) && AIState == (int)BehaviorState.IdleAndFly)
            {
                SoundEngine.PlaySound(IdleSound, NPC.Center);
                SquashVector = Utils.SelectRandom(Main.rand, new Vector2(1.4f, 0.7f), new Vector2(0.7f, 1.4f));
                AdditionalBrightness = Main.rand.NextFloat(0.3f, 0.4f);
                NPC.soundDelay = Main.rand.Next(60, 120);
                NPC.netUpdate = true;
            }

            if (!SoundEngine.TryGetActiveSound(SwineSpeakSoundSlot, out _))
                SwineSpeakSoundSlot = SoundEngine.PlaySound(SwineSpeakLoopingSound, NPC.Center, SoundCallbackMethod);

            // Fade the music depending on the distance between the player and Divine Swine.
            float musicVolumeInterpolant = Utils.Remap(NPC.Distance(Main.LocalPlayer.Center), 600f, 100f, 1f, 0.2f, true);
            Main.musicFade[Main.curMusic] = musicVolumeInterpolant;
        }

        private bool SoundCallbackMethod(ActiveSound soundInstance)
        {
            soundInstance.Position = NPC.Center;

            float idealPitch = 0f;
            bool raisePitchDuringBehavior = AIState == (int)BehaviorState.OfferingAccepted || AIState == (int)BehaviorState.PiggyTransformation;
            if (raisePitchDuringBehavior)
                idealPitch = 0.4f;

            float volumeInterpolant = Utils.Remap(NPC.Distance(Main.LocalPlayer.Center), 600f, 100f, 0.1f, 0.7f, true) * GlowingVisualScale;
            soundInstance.Volume = volumeInterpolant;
            soundInstance.Pitch = MathHelper.Lerp(soundInstance.Pitch, idealPitch, 0.075f);
            return NPC.active;
        }

        public void AcceptOffering(bool accepted)
        {
            SoundStyle offeringSound = accepted ? CoinPassSound : CoinFailSound;
            SoundEngine.PlaySound(offeringSound, NPC.Center);
            if (accepted)
                SoundEngine.PlaySound(SoundID.Coins, NPC.Center);

            AIState = accepted ? (int)BehaviorState.OfferingAccepted : (int)BehaviorState.OfferingFailed;
            Timer = 0f;
            LocalAIState = 0f;
            NPC.netUpdate = true;
            NetMessage.SendData(MessageID.WorldData);
        }

        private bool ShouldAvoidTile(Tile tile) => WorldGen.SolidTile(tile) || (tile.HasUnactuatedTile && Main.tileSolidTop[tile.TileType]) || tile.LiquidAmount >= 255;

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Sky && NPC.CountNPCS(Type) < 1)
                return 0.001f;
            return 0f;
        }

        private static void DivineSwineRightClickInteraction(On_Main.orig_HoverOverNPCs orig, Main self, Rectangle mouseRectangle)
        {
            orig(self, mouseRectangle);

            Player player = Main.LocalPlayer;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.type != ModContent.NPCType<DivineSwine>())
                    continue;

                if ((!npc.ShowNameOnHover || !(npc.active & (npc.shimmerTransparency == 0f || npc.CanApplyHunterPotionEffects()))))
                    continue;

                Main.instance.LoadNPC(npc.whoAmI);

                Rectangle npcRectangle = new((int)(npc.Bottom.X - npc.frame.Width * 0.5f), (int)(npc.Bottom.Y - npc.frame.Height), npc.frame.Width, npc.frame.Height);
                NPCLoader.ModifyHoverBoundingBox(npc, ref npcRectangle);

                bool hoveringOverHitbox = mouseRectangle.Intersects(npcRectangle);
                bool canBeInteractedWith = hoveringOverHitbox || (Main.SmartInteractShowingGenuine && Main.SmartInteractNPC == npc.whoAmI);
                if (canBeInteractedWith && npc.ai[1] != 1)
                {
                    player.cursorItemIconEnabled = true;
                    player.cursorItemIconID = ItemID.PlatinumCoin;
                    player.cursorItemIconText = "";
                    player.noThrow = 2;

                    PlayerInput.SetZoom_MouseInWorld();
                    if (Main.mouseRight && Main.npcChatRelease)
                    {
                        Main.npcChatRelease = false;
                        if (PlayerInput.UsingGamepad)
                            player.releaseInventory = false;

                        if (player.talkNPC != npc.whoAmI && !player.tileInteractionHappened)
                        {
                            bool offeringState = TryOfferingPlatinumToSwine();
                            npc.ModNPC<DivineSwine>().AcceptOffering(offeringState);
                            if (Main.netMode == NetmodeID.MultiplayerClient)
                                DivineSwineOfferingPacket.Send(npc.ModNPC<DivineSwine>(), offeringState);
                        }
                    }
                }

                // Change the mouse display to just show the name and not the HP number.
                if (hoveringOverHitbox && !player.mouseInterface)
                {
                    self.MouseTextHackZoom(npc.GivenOrTypeName);
                    Main.mouseText = true;
                }
            }
        }

        private static bool TryOfferingPlatinumToSwine()
        {
            Player player = Main.LocalPlayer;
            bool inPiggyBank = false;

            int inventoryIndexSlot = player.FindItem(ItemID.PlatinumCoin);
            int piggyBankIndexSlot = player.FindItem(ItemID.PlatinumCoin, player.bank.item);
            if (piggyBankIndexSlot != -1)
                inPiggyBank = true;

            if (inventoryIndexSlot == -1 && piggyBankIndexSlot == -1)
                return false;

            Item foundItem = inPiggyBank ? player.bank.item[piggyBankIndexSlot] : player.inventory[inventoryIndexSlot];
            if (--foundItem.stack <= 0)
                foundItem.TurnToAir();

            Recipe.FindRecipes();
            return true;
        }

        private static void AdjustLifeformAnalyzerTextColor(On_Main.orig_DrawInfoAccs_AdjustInfoTextColorsForNPC orig, Main self, NPC npc, ref Color infoTextColor, ref Color infoTextShadowColor)
        {
            orig(self, npc, ref infoTextColor, ref infoTextShadowColor);
            if (npc.type == ModContent.NPCType<DivineSwine>())
            {
                infoTextColor = Color.Lerp(DivineBlue, DivineYellow, MathF.Sin((float)Main.timeForVisualEffects / 45f) * 0.5f + 0.5f);
                infoTextShadowColor = infoTextColor * 0.1f;
                infoTextColor.A = Main.mouseTextColor;
                infoTextShadowColor.A = Main.mouseTextColor;
            }
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

        public override void FindFrame(int frameHeight)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                NPC.frameCounter += 1;
                if (NPC.frameCounter >= 5)
                {
                    if (NPC.frame.Y / frameHeight >= Main.npcFrameCount[Type] - 1)
                        NPC.frame.Y = 0;
                    else
                        NPC.frame.Y += frameHeight;

                    NPC.frameCounter = 0;
                }

                Timer++;
                if (Timer >= 3600)
                    Timer = 0f;

                return;
            }

            NPC.frame.Y = frameHeight * FrameY;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                Draw_BestiaryPortrait(spriteBatch);
                return false;
            }

            spriteBatch.End(out var snapshot);

            Texture2D baseTexture = TextureAssets.Npc[Type].Value;
            Vector2 drawPosition = NPC.Center - screenPos + Vector2.UnitY * NPC.gfxOffY;
            SpriteEffects spriteEffects = NPC.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            // Draw the cool glowing sigil behind the Divine Swine.
            Draw_DivineSigiliGlowyBullshit(spriteBatch, drawPosition);

            // Subtractive backing images.
            spriteBatch.SetBlendState(CalamityUtils.SubtractiveBlending);

            int backShadowCount = 3;
            for (int i = 0; i < backShadowCount; i++)
            {
                float rotation = (float)(Main.timeForVisualEffects / MathHelper.Pi * 0.08f) + NPC.whoAmI;
                Vector2 backglowDrawPosition = drawPosition + Vector2.UnitX.RotatedBy(i * MathHelper.TwoPi / backShadowCount + rotation) * 6f;
                spriteBatch.Draw(baseTexture, backglowDrawPosition, NPC.frame, NPC.GetAlpha(Color.White) * 0.9f, NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale * GlowingVisualScale, spriteEffects, 0f);
            }

            int backShadowCount2 = 6;
            for (int i = 0; i < backShadowCount2; i++)
            {
                float rotation = (float)(Main.timeForVisualEffects / MathHelper.Pi * 0.06f) + NPC.whoAmI;
                Vector2 backglowDrawPosition = drawPosition + Vector2.UnitX.RotatedBy(i * MathHelper.TwoPi / backShadowCount2 + rotation) * 12f;
                spriteBatch.Draw(baseTexture, backglowDrawPosition, NPC.frame, NPC.GetAlpha(Color.White) * 0.7f, NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale * GlowingVisualScale, spriteEffects, 0f);
            }

            // Actual Divine Swine.
            Effect tintShader = CalamityShaders.BasicTintShader.Value;
            tintShader.Parameters["uColor"].SetValue(Color.White.ToVector3());
            tintShader.Parameters["uOpacity"].SetValue(DivineSwineTintStrength);

            spriteBatch.EnterShaderRegion(effect: tintShader);
            spriteBatch.Draw(baseTexture, drawPosition, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() * 0.5f, SquashVector * NPC.scale, spriteEffects, 0f);

            spriteBatch.End();

            // Offering accept visual of the reward item being conjured.
            Draw_OfferingAcceptedVisual(spriteBatch, drawPosition);

            spriteBatch.Begin(snapshot);
            return false;
        }

        public void Draw_DivineSigiliGlowyBullshit(SpriteBatch spriteBatch, Vector2 drawPosition)
        {
            using var glowingBullshitLease = RenderTargetPool.Shared.Rent(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight, RenderTargetDescriptor.Default);
            using var pixelationLease = RenderTargetPool.Shared.Rent(Main.graphics.GraphicsDevice, Main.screenWidth / 2, Main.screenHeight / 2, RenderTargetDescriptor.Default);

            // Buncha yellow-colored bloom stuff to look like a glowing sun.
            using (spriteBatch.Scope())
            {
                using (glowingBullshitLease.Scope(clearColor: Color.Transparent))
                {
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Matrix.Identity);

                    float bloomFlareScale = MathHelper.Lerp(0.4f, 0.7f, MathF.Sin((float)Main.timeForVisualEffects / 60f) * 0.5f + 0.5f) * GlowingVisualScale;
                    spriteBatch.Draw(BloomFlare.Value, drawPosition, null, NPC.GetAlpha(DivineYellow with { A = 0 }) * (0.8f + AdditionalBrightness), MathHelper.PiOver4, BloomFlare.Size() * 0.5f, bloomFlareScale, 0, 0f);

                    spriteBatch.Draw(ShineFlare.Value, drawPosition, null, NPC.GetAlpha(DivineYellow with { A = 0 }) * (1f + AdditionalBrightness), 0f, MagicStarCircle.Size() * 0.5f, bloomFlareScale, 0, 0f);
                    spriteBatch.Draw(MagicStarCircle.Value, drawPosition, null, NPC.GetAlpha(DivineYellow with { A = 0 }) * (0.8f + AdditionalBrightness), 0f, MagicStarCircle.Size() * 0.5f, 0.5f * GlowingVisualScale, 0, 0f);

                    spriteBatch.End();

                    Effect chromaAbberShader = CalamityShaders.ChromaticAbberationShader.Value;
                    chromaAbberShader.Parameters["abberationStrength"].SetValue(10f);
                    chromaAbberShader.Parameters["impactPosition"].SetValue(drawPosition);

                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, chromaAbberShader, Matrix.Identity);

                    float starRingScale = MathHelper.Lerp(0.7f, 1f, MathF.Sin((float)Main.timeForVisualEffects / 180f) * 0.5f + 0.5f) * GlowingVisualScale;
                    spriteBatch.Draw(FadedStarRing.Value, drawPosition, null, NPC.GetAlpha(DivineBlue with { A = 0 }) * (0.7f + AdditionalBrightness), (float)(Main.timeForVisualEffects / 720f) + NPC.whoAmI, FadedStarRing.Size() * 0.5f, starRingScale, 0, 0f);

                    spriteBatch.End();
                }
            }

            float distortionStrength = 0.034f;
            using (spriteBatch.Scope())
            {
                using (pixelationLease.Scope(clearColor: Color.Transparent))
                {
                    Effect distortionShader = CalamityShaders.BasicTextureDistortionShader.Value;
                    distortionShader.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly);
                    distortionShader.Parameters["noiseScale"].SetValue(1.3f);
                    distortionShader.Parameters["distortionStrength"].SetValue(distortionStrength);
                    distortionShader.Parameters["timeOffset"].SetValue(new Vector2(-0.02f, 0.01f));

                    Main.graphics.GraphicsDevice.Textures[1] = DistortionTexture.Value;
                    Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.LinearWrap;

                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, distortionShader, PixelationManager.PixelationMatrix);
                    spriteBatch.Draw(glowingBullshitLease.Target, Vector2.Zero, Color.White);
                    spriteBatch.End();
                }
            }

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Vector2 targetDrawPosition = Main.ScreenSize.ToVector2() * 0.5f * distortionStrength;
            spriteBatch.Draw(pixelationLease.Target, targetDrawPosition, null, Color.White, 0f, Vector2.Zero, 2f, 0, 0f);
        }

        public void Draw_OfferingAcceptedVisual(SpriteBatch spriteBatch, Vector2 drawPosition)
        {
            if (AIState != (int)BehaviorState.OfferingAccepted)
                return;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            float bloomRotation = (float)(Main.timeForVisualEffects / MathHelper.Pi * 0.03f) + NPC.whoAmI;
            float bloomMultiplier = CalamityUtils.PolyOutEasing(Utils.Remap(Timer, 0f, 30f, 0f, 1f, true), 2);
            spriteBatch.Draw(BloomFlare.Value, drawPosition, null, NPC.GetAlpha(Color.White with { A = 0 }) * bloomMultiplier, bloomRotation, BloomFlare.Size() * 0.5f, 0.2f * bloomMultiplier, SpriteEffects.None, 0f);
            spriteBatch.Draw(BloomFlare.Value, drawPosition, null, NPC.GetAlpha(Color.White with { A = 0 }) * 0.8f * bloomMultiplier, -bloomRotation * 0.5f, BloomFlare.Size() * 0.5f, 0.3f * bloomMultiplier, SpriteEffects.FlipVertically, 0f);

            spriteBatch.SetBlendState(CalamityUtils.SubtractiveBlending);

            int backShadowCount = 6;
            float backShadowInterpolant = Utils.GetLerpValue(0f, 150f, Timer, true);
            float backShadowDistance = MathHelper.Lerp(36f, 4f, backShadowInterpolant);
            for (int i = 0; i < backShadowCount; i++)
            {
                float rotation = (float)(Main.timeForVisualEffects / MathHelper.Pi * 0.08f) + NPC.whoAmI;
                Vector2 backglowDrawPosition = drawPosition + Vector2.UnitX.RotatedBy(i * MathHelper.TwoPi / backShadowCount + rotation) * backShadowDistance;
                spriteBatch.Draw(DeliciousMeatTexture.Value, backglowDrawPosition, DeliciousMeatTexture.Value.Frame(1, 3), NPC.GetAlpha(Color.White) * backShadowInterpolant, NPC.rotation, DeliciousMeatTexture.Value.Frame(1, 3).Size() * 0.5f, 1f, 0, 0f);
            }

            spriteBatch.SetBlendState(BlendState.AlphaBlend);

            float shineInterpolant = Utils.GetLerpValue(150f, 210f, Timer, true);
            float shineScale = CalamityUtils.SineOutEasing(0.8f * shineInterpolant, 0);
            spriteBatch.Draw(BloomCircle.Value, drawPosition, null, NPC.GetAlpha(Color.White with { A = 0 }), NPC.rotation, BloomCircle.Size() * 0.5f, shineScale, 0, 0f);
            spriteBatch.Draw(ShineFlare.Value, drawPosition, null, NPC.GetAlpha(Color.White with { A = 0 }), NPC.rotation, ShineFlare.Size() * 0.5f, shineScale - 0.4f, 0, 0f);

            spriteBatch.End();
        }

        public void Draw_BestiaryPortrait(SpriteBatch spriteBatch)
        {
            Texture2D baseTexture = TextureAssets.Npc[Type].Value;
            float floatHeight = MathHelper.Lerp(-8f, 8f, MathF.Sin((float)Timer / 150f) * 0.5f + 0.5f);
            Vector2 drawPosition = NPC.Center + Vector2.UnitY * floatHeight;
            Matrix pixelationMatrix = Matrix.CreateScale(0.5f, 0.5f, 1f);

            RasterizerState previousRasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
            Rectangle previousScissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
            using (spriteBatch.Scope())
            {
                var device = Main.graphics.GraphicsDevice;
                using var glowingBullshitLease = RenderTargetPool.Shared.Rent(device, Main.screenWidth, Main.screenHeight, RenderTargetDescriptor.Default);
                using var pixelationLease = RenderTargetPool.Shared.Rent(device, Main.screenWidth / 2, Main.screenHeight / 2, RenderTargetDescriptor.Default);
                using (glowingBullshitLease.Scope(clearColor: Color.Transparent))
                {
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Matrix.Identity);

                    float bloomFlareScale = MathHelper.Lerp(0.15f, 0.3f, MathF.Sin((float)Main.timeForVisualEffects / 60f) * 0.5f + 0.5f);
                    spriteBatch.Draw(BloomFlare.Value, drawPosition, null, NPC.GetAlpha(DivineYellow) with { A = 0 } * 0.4f, MathHelper.PiOver4, BloomFlare.Size() * 0.5f, bloomFlareScale, 0, 0f);

                    spriteBatch.Draw(ShineFlare.Value, drawPosition, null, NPC.GetAlpha(DivineYellow) with { A = 0 } * 0.7f, 0f, MagicStarCircle.Size() * 0.5f, bloomFlareScale, 0, 0f);
                    spriteBatch.Draw(MagicStarCircle.Value, drawPosition, null, NPC.GetAlpha(DivineYellow) with { A = 0 } * 0.5f, 0f, MagicStarCircle.Size() * 0.5f, 0.25f, 0, 0f);

                    Effect chromaAbberShader = CalamityShaders.ChromaticAbberationShader.Value;
                    chromaAbberShader.Parameters["abberationStrength"].SetValue(10f);
                    chromaAbberShader.Parameters["impactPosition"].SetValue(drawPosition);

                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, chromaAbberShader, Matrix.Identity);

                    float starRingScale = MathHelper.Lerp(0.2f, 0.4f, MathF.Sin((float)Main.timeForVisualEffects / 180f) * 0.5f + 0.5f);
                    spriteBatch.Draw(FadedStarRing.Value, drawPosition, null, NPC.GetAlpha(DivineBlue) with { A = 0 } * 0.8f, (float)(Main.timeForVisualEffects / 720f) + NPC.whoAmI, FadedStarRing.Size() * 0.5f, starRingScale, 0, 0f);

                    spriteBatch.End();
                }

                using (pixelationLease.Scope(clearColor: Color.Transparent))
                {
                    float distortionStrength = 0.0212f;
                    Effect distortionShader = CalamityShaders.BasicTextureDistortionShader.Value;
                    distortionShader.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly);
                    distortionShader.Parameters["noiseScale"].SetValue(2.3f);
                    distortionShader.Parameters["distortionStrength"].SetValue(distortionStrength);
                    distortionShader.Parameters["timeOffset"].SetValue(new Vector2(-0.02f, 0.01f));

                    Main.graphics.GraphicsDevice.Textures[1] = DistortionTexture.Value;
                    Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.LinearWrap;

                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, distortionShader, pixelationMatrix);

                    Vector2 targetDrawPosition = Main.ScreenSize.ToVector2() * 0.5f * distortionStrength;
                    spriteBatch.Draw(glowingBullshitLease.Target, targetDrawPosition, Color.White with { A = 0 });

                    spriteBatch.End();
                }

                spriteBatch.GraphicsDevice.RasterizerState = previousRasterizerState;
                spriteBatch.GraphicsDevice.ScissorRectangle = previousScissorRectangle;
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, previousRasterizerState, null, Main.UIScaleMatrix);

                spriteBatch.Draw(pixelationLease.Target, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 2f, 0, 0f);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, CalamityUtils.SubtractiveBlending, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);

                int backShadowCount = 3;
                for (int i = 0; i < backShadowCount; i++)
                {
                    float rotation = (float)(Main.timeForVisualEffects / MathHelper.Pi * 0.08f) + NPC.whoAmI;
                    Vector2 backglowDrawPosition = drawPosition + Vector2.UnitX.RotatedBy(i * MathHelper.TwoPi / backShadowCount + rotation) * 6f;
                    spriteBatch.Draw(baseTexture, backglowDrawPosition, NPC.frame, NPC.GetAlpha(Color.White) * 0.9f, NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, 0, 0f);
                }

                int backShadowCount2 = 6;
                for (int i = 0; i < backShadowCount2; i++)
                {
                    float rotation = (float)(Main.timeForVisualEffects / MathHelper.Pi * 0.06f) + NPC.whoAmI;
                    Vector2 backglowDrawPosition = drawPosition + Vector2.UnitX.RotatedBy(i * MathHelper.TwoPi / backShadowCount2 + rotation) * 12f;
                    spriteBatch.Draw(baseTexture, backglowDrawPosition, NPC.frame, NPC.GetAlpha(Color.White) * 0.7f, NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, 0, 0f);
                }

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
                spriteBatch.Draw(baseTexture, drawPosition, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, 0, 0f);

                spriteBatch.End();
            }
        }
    }
}
