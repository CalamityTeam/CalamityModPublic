using System;
using System.Security.Policy;
using CalamityMod.Effects;
using CalamityMod.Items.Critters;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.NPCs.NormalNPCs.HorribleHog;
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
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class Piggy : ModNPC
    {
        public enum BehaviorState
        {
            DivineSwineTransformation = -2,
            HorribleHogTransformation = -1,
            IdleAndWalk,
            Running,
        }

        private static Asset<Texture2D> SpotlightTexture;
        private static Asset<Texture2D> BloomCircle;
        private static Asset<Texture2D> BloomFlare;
        private static Asset<Texture2D> ShineFlare;

        private static SoundStyle IdleSound_Grunt = new("CalamityMod/Sounds/Custom/Piggy/PiggyIdle_Grunt", 3);
        private static SoundStyle IdleSound_SnortYip = new("CalamityMod/Sounds/Custom/Piggy/PiggyIdle_SnortYip", 3);
        private static SoundStyle IdleSound_Yip = new("CalamityMod/Sounds/Custom/Piggy/PiggyIdle_Yip", 2);
        private static SoundStyle DeathSound = new("CalamityMod/Sounds/NPCKilled/PiggyDeath", 2);

        private static SoundStyle DivineSwine_CoinFailSound = new("CalamityMod/Sounds/Custom/DivineSwine/DivineSwineCoinFail", 3);
        private static SoundStyle DivineSwine_SwineSpeakLoopingSound = new("CalamityMod/Sounds/Custom/DivineSwine/DivineSwineNearbyLoop")
        {
            IsLooped = true,
            PauseBehavior = PauseBehavior.PauseWithGame,
            MaxInstances = 0,
        };

        private static SoundStyle HorribleHog_CackleSound = new("CalamityMod/Sounds/Custom/HorribleHog/HorribleHogCackle");
        private static SoundStyle HorribleHog_IdleSound = new("CalamityMod/Sounds/Custom/HorribleHog/HorribleHogIdle", 2)
        {
            PitchVariance = 0.25f,
        };
        private static SoundStyle HorribleHog_DevilsTongueLoopingSound = new("CalamityMod/Sounds/Custom/HorribleHog/HorribleHogNearbyLoop")
        {
            IsLooped = true,
            PauseBehavior = PauseBehavior.PauseWithGame,
            MaxInstances = 0
        };

        public Vector2 SquashVector;

        public SlotId NearbySoundSlot;

        public static float MaxAcceleration_Walking => 0.035f;
        public static float MaxAcceleration_Running => 0.085f;
        public static float MaxSpeed_Walking => 1.2f;
        public static float MaxSpeed_Running => 3.6f;

        public ref float Timer => ref NPC.ai[0];

        public ref float AIState => ref NPC.ai[1];

        public ref float LocalAIState => ref NPC.ai[2];

        public override void Load()
        {
            if (Main.dedServ)
                return;

            SpotlightTexture = ModContent.Request<Texture2D>("Terraria/Images/Extra_60");
            BloomCircle = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle");
            BloomFlare = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/BloomFlare");
            ShineFlare = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/ShineFlare");
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 8;
            Main.npcCatchable[Type] = true;
            NPCID.Sets.CountsAsCritter[Type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = true;
            NPCID.Sets.NormalGoldCritterBestiaryPriority.Insert(NPCID.Sets.NormalGoldCritterBestiaryPriority.IndexOf(NPCID.GoldBunny) + 1, Type);
        }

        public override void SetDefaults()
        {
            NPC.damage = 0;
            NPC.width = 42;
            NPC.height = 28;
            NPC.lifeMax = 300;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 1.15f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = DeathSound;
            NPC.catchItem = (short)ModContent.ItemType<PiggyItem>();
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<PiggyBanner>();
            NPC.chaseable = false;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;

            SquashVector = Vector2.One;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Piggy")
            });
        }

        public override void AI()
        {
            // Force bestiary cause this doesn't count as Vanilla's definition of a critter for the Bestiary.
            if (Main.netMode != NetmodeID.MultiplayerClient && Main.BestiaryTracker.Kills.GetKillCount(NPC) <= 0)
                Main.BestiaryTracker.Kills.SetKillCountDirectly(NPC.GetBestiaryCreditId(), 100);

            if (NPC.direction == 0)
                NPC.direction = Utils.SelectRandom(Main.rand, -1, 1);

            switch ((BehaviorState)AIState)
            {
                case BehaviorState.DivineSwineTransformation:
                    MainBehavior_DivineSwineTransformation();
                    break;

                case BehaviorState.HorribleHogTransformation:
                    MainBehavior_HorribleHogTransformation();
                    break;

                case BehaviorState.IdleAndWalk:
                    MainBehavior_IdleAndWalk();
                    break;

               case BehaviorState.Running:
                    MainBehavior_Running();
                    break;
            }
        
            NPC.StepUpBlocks();

            SquashVector = Vector2.Lerp(SquashVector, Vector2.One, 0.125f);
            Timer++;
        }

        public void MainBehavior_DivineSwineTransformation()
        {
            NPC.velocity.X *= 0.9f;
            NPC.rotation = 0f;
            if (!SoundEngine.TryGetActiveSound(NearbySoundSlot, out _))
                NearbySoundSlot = SoundEngine.PlaySound(DivineSwine_SwineSpeakLoopingSound, NPC.Center, NearbySoundCallbackMethod);

            float lightSpawnDistance = MathHelper.Lerp(52f, 84f, Timer / 180f);
            int lightAmt = Main.rand.Next(1, 2);
            for (int i = 0; i < lightAmt; i++)
            {
                Vector2 lightSpawnPosition = NPC.Center + Main.rand.NextVector2Unit() * lightSpawnDistance * Main.rand.NextFloat(0.7f, 1f);
                Vector2 lightVelocity = (NPC.Center - lightSpawnPosition).SafeNormalize(Vector2.UnitY) * Main.rand.NextFloat(3f, 4f);
                float lightScale = Utils.Remap(Timer, 0f, 180f, 0.3f, 0.9f, true);

                SquishyLightParticle meatLight = new(lightSpawnPosition, lightVelocity, lightScale, Color.White, Main.rand.Next(30, 45));
                GeneralParticleHandler.SpawnParticle(meatLight, true);
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

            if (Timer >= 30f && Main.rand.NextBool(10))
            {
                Vector2 spawnPosition = NPC.Center + new Vector2(Main.rand.NextFloat(-16f, 16f), Main.rand.NextFloat(-144f, -134f));
                Vector2 velocity = Vector2.UnitY * Main.rand.NextFloat(0.25f, 0.75f);
                Color featherColor = Color.Lerp(Color.Lerp(new Color(27, 103, 155), new Color(83, 184, 255), Main.rand.NextFloat()), new Color(221, 253, 255), Main.rand.NextFloat(0.4f));
                float scale = Main.rand.NextFloat(0.8f, 1f);
                
                FeatherParticle feather = new(spawnPosition, velocity, featherColor, scale, Main.rand.Next(75, 90), null, 0.9f, false, false, true);
                GeneralParticleHandler.SpawnParticle(feather, manualDrawLayerOverride: Enums.GeneralDrawLayer.BeforeNPCs);
            }

            if (Timer >= 180f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                CustomPulse lightRing = new(NPC.Center, Vector2.Zero, Color.White, "CalamityMod/Particles/BloomRing", Vector2.One, 0f, 0f, 2f, 75);
                GeneralParticleHandler.SpawnParticle(lightRing);

                BloomParticle bloom = new(NPC.Center, Vector2.Zero, Color.White, 0f, 2f, 125);
                for (int i = 0; i < 3; i++)
                    GeneralParticleHandler.SpawnParticle(bloom);

                for (int i = 0; i < 25; i++)
                {
                    QuickSparkleParticle sparkle = new(NPC.Center, Main.rand.NextVector2Circular(15f, 15f), Color.White, Main.rand.NextFloat(0.6f, 0.8f), Main.rand.Next(45, 60));
                    SquishyLightParticle light = new(NPC.Center, Main.rand.NextVector2Circular(5f, 5f), Main.rand.NextFloat(0.6f, 0.8f), Color.White, Main.rand.Next(45, 60));
                    GeneralParticleHandler.SpawnParticle(Main.rand.NextBool() ? sparkle : light, true);
                }

                SoundEngine.PlaySound(DivineSwine_CoinFailSound, NPC.Center);
                if (SoundEngine.TryGetActiveSound(NearbySoundSlot, out var activeSound))
                    activeSound.Stop();

                NPC.Transform(ModContent.NPCType<DivineSwine>());
                NPC.netUpdate = true;
            }
        }

        public void MainBehavior_HorribleHogTransformation()
        {
            NPC.velocity.X *= 0.9f;
            NPC.rotation = 0f;
            if (!SoundEngine.TryGetActiveSound(NearbySoundSlot, out _))
                NearbySoundSlot = SoundEngine.PlaySound(HorribleHog_DevilsTongueLoopingSound, NPC.Center, NearbySoundCallbackMethod);

            float smokeOpacity = Utils.GetLerpValue(0f, 45f, Timer, true);
            int circlingSmokeAmount = Main.rand.Next(2, 5);
            for (int i = 0; i < circlingSmokeAmount; i++)
            {
                Vector2 spawnDistanceVariance = Main.rand.NextVector2Circular(50f, 50f);
                Vector2 spawnPosition = NPC.Center + spawnDistanceVariance;
                Vector2 velocity = spawnDistanceVariance.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(2f, 5f);
                Color color = Color.Lerp(new(30, 30, 30), Color.Crimson, Main.rand.Next(2));
                int lifetime = Main.rand.Next(30, 45);
                float scale = Main.rand.NextFloat(1f, 1.2f);

                HeavySmokeParticle circlingSmoke = new(spawnPosition, velocity, color, lifetime, scale, smokeOpacity, 0.01f, affectedByLight: true);
                GeneralParticleHandler.SpawnParticle(circlingSmoke, true);
            }

            if (Timer >= 180f)
            {
                SoundEngine.PlaySound(HorribleHog_CackleSound, NPC.Center);
                if (SoundEngine.TryGetActiveSound(NearbySoundSlot, out var activeSound))
                    activeSound.Stop();

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.Transform(ModContent.NPCType<HorribleHog.HorribleHog>());
                    NPC.netUpdate = true;
                }
            }
        }

        public void MainBehavior_IdleAndWalk()
        {
            // Idling.
            if (LocalAIState == 0f)
            {
                if (Timer > 0f)
                {
                    if (Timer % 60f == 0f && Main.rand.NextBool(6))
                    {
                        Timer = 0f;
                        LocalAIState = 1f;
                        NPC.netUpdate = true;
                    }

                    if (Timer % 60f == 0f && Main.rand.NextBool(12))
                    {
                        AIState = (int)BehaviorState.Running;
                        Timer = 0f;
                        LocalAIState = 0f;
                        NPC.netUpdate = true;
                    }
                }

                // Stop moving and occasionally switch directions.
                if (NPC.velocity.Y == 0f)
                {
                    NPC.velocity.X *= 0.8f;
                    if (Timer % 15f == 0f && Main.rand.NextBool(12))
                        NPC.direction *= -1;
                }
            }

            // Walking.
            if (LocalAIState == 1f)
            {
                if (Timer > 120f && Timer % 60f == 0f && Main.rand.NextBool(5))
                {
                    Timer = 0f;
                    LocalAIState = 0f;
                    NPC.netUpdate = true;
                }

                if (MathF.Abs(NPC.velocity.X) < MaxSpeed_Walking)
                    NPC.velocity.X += MaxAcceleration_Walking * NPC.direction;

                if (NPC.collideX && NPC.velocity.Y == 0f)
                    NPC.velocity.Y -= 6f;
            }

            // Sound effects.
            if (NPC.soundDelay == 0 && Main.rand.NextBool(200))
            {
                // Small chance to do a cute lil hop as well.
                if (Main.rand.NextBool(5))
                {
                    NPC.velocity.Y -= Main.rand.NextFloat(2f, 4f);
                    SquashVector = new Vector2(0.7f, 1.3f);
                    NPC.netUpdate = true;
                }

                var chosenSoundStyle = Utils.SelectRandom(Main.rand, IdleSound_Grunt, IdleSound_SnortYip, IdleSound_Yip);
                SoundEngine.PlaySound(chosenSoundStyle, NPC.Center);
                NPC.soundDelay = Main.rand.Next(60, 120);
            }

            NPC.spriteDirection = NPC.direction;
            float targetAngle = (NPC.velocity.Y != 0f) ? NPC.velocity.X * 0.085f * (NPC.velocity.Y < 0).ToDirectionInt() : 0f;
            NPC.rotation = NPC.rotation.AngleLerp(targetAngle, 0.075f);
        }

        public void MainBehavior_Running()
        {
            // Run in a random direction until collision with a wall is made.
            if (LocalAIState == 0f)
            {
                if (MathF.Abs(NPC.velocity.X) < MaxSpeed_Running)
                    NPC.velocity.X += MaxAcceleration_Running * NPC.direction;
                NPC.spriteDirection = NPC.direction;

                // Spawn particles when running at max speed.
                if (NPC.velocity.Y == 0f && MathF.Abs(NPC.velocity.X) >= MaxSpeed_Running)
                {
                    int dustType = NPC.type == ModContent.NPCType<PiggyGold>() ? DustID.Enchanted_Gold : DustID.Cloud;
                    Vector2 dustPosition = new(NPC.Bottom.X + Main.rand.NextFloat(-NPC.width * 0.5f, NPC.width * 0.5f), NPC.Bottom.Y);
                    Dust.NewDustPerfect(dustPosition, dustType, new Vector2(NPC.velocity.X * 0.2f, Main.rand.NextFloat(-0.3f, 0.3f)), 0, default, Main.rand.NextFloat(1f, 1.2f));
                    if (Timer % 7 == 0f)
                        SoundEngine.PlaySound(SoundID.Run with { Pitch = 0.3f, Volume = 0.7f, Identifier = "Piggy Run" }, NPC.Center);
                }

                if (HoleBelow() && NPC.velocity.Y == 0f)
                    NPC.velocity.Y -= 6f;

                if (NPC.collideX)
                {
                    SoundEngine.PlaySound(SoundID.Dig with { Volume = 0.7f }, NPC.Center);
                    SoundEngine.PlaySound(IdleSound_Grunt with { Volume = 1.2f }, NPC.Center);
                    SquashVector = new Vector2(0.6f, 1f);

                    NPC.velocity.X = NPC.oldVelocity.X * -0.86f;
                    NPC.velocity.Y -= 3f;
                    Timer = 0f;
                    LocalAIState = 1f;
                    NPC.netUpdate = true;
                }
                // Stop running and go back to idling if 5 seconds has passed without collision.
                else if (Timer >= 300f)
                {
                    AIState = (int)BehaviorState.IdleAndWalk;
                    LocalAIState = Main.rand.Next(1);
                    Timer = 0f;
                    NPC.netUpdate = true;
                }

                float idealRotation = (NPC.velocity.Y != 0f) ? NPC.velocity.ToRotation() + (NPC.direction > 0 ? 0f : -MathHelper.Pi) : 0f;
                NPC.rotation = idealRotation;
            }

            if (LocalAIState == 1f)
            {
                if (NPC.velocity.Y == 0f)
                {
                    NPC.velocity.X *= 0.9f;
                    if (MathF.Abs(NPC.velocity.X) >= 0.08f && MathF.Abs(NPC.velocity.X) < 0.09f)
                    {
                        NPC.velocity.Y = -4f;
                        var soundStyle = Utils.SelectRandom(Main.rand, IdleSound_SnortYip, IdleSound_Yip);
                        SoundEngine.PlaySound(soundStyle.WithPitchOffset(0.35f), NPC.Center);
                    }
                }

                if (MathF.Abs(NPC.velocity.X) > 0.09f)
                    NPC.rotation += NPC.velocity.X * 0.075f;
                else
                    NPC.rotation = NPC.rotation.AngleLerp(0f, 0.125f);

                if (Timer >= 120f)
                {
                    AIState = (int)BehaviorState.IdleAndWalk;
                    LocalAIState = Main.rand.Next(1);
                    Timer = 0f;
                    NPC.netUpdate = true;
                }
            }
        }

        public void TransformIntoVariant(int type)
        {
            if (type == ModContent.NPCType<DivineSwine>())
                AIState = (int)BehaviorState.DivineSwineTransformation;
            else if (type == ModContent.NPCType<HorribleHog.HorribleHog>())
                AIState = (int)BehaviorState.HorribleHogTransformation;
            else
                AIState = (int)BehaviorState.IdleAndWalk;

            LocalAIState = 0f;
            Timer = 0f;
            NPC.netUpdate = true;
        }

        public bool TryTransformingIntoVariant()
        {
            if (AIState == (int)BehaviorState.DivineSwineTransformation || AIState == (int)BehaviorState.HorribleHogTransformation)
                return false;
            return true;
        }

        private bool HoleBelow()
        {
            int npcWidthInTiles = NPC.width / 16;
            int tileX = (int)(NPC.Center.X / 16f) - npcWidthInTiles;
            if (NPC.velocity.X > 0)
                tileX += npcWidthInTiles;

            int tileY = (int)((NPC.position.Y + NPC.height) / 16f);
            for (int y = tileY; y < tileY + 2; y++)
            {
                for (int x = tileX; x < tileX + npcWidthInTiles; x++)
                {
                    if (Main.tile[x, y].HasTile)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool NearbySoundCallbackMethod(ActiveSound soundInstance)
        {
            float interpolant = Utils.GetLerpValue(0f, 75f, Timer, true) * Utils.GetLerpValue(180f, 130f, Timer, true);
            soundInstance.Position = NPC.Center;
            soundInstance.Volume = interpolant;
            return NPC.active;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            float goldMultiplier = NPC.type == ModContent.NPCType<PiggyGold>() ? 0.0025f : 1f;
            bool inAtTown = spawnInfo.Player.townNPCs > 2f && (Main.remixWorld ? spawnInfo.Player.ZoneNormalCaverns : spawnInfo.Player.ZoneForest);
            if (inAtTown)
                return SpawnCondition.TownCritter.Chance * 0.1f * goldMultiplier;
            else if (spawnInfo.Player.ZonePurity)
                return (Main.remixWorld ? SpawnCondition.Cavern.Chance * 0.005f : SpawnCondition.OverworldDayGrassCritter.Chance * 0.005f) * goldMultiplier;

            return 0f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.Add(ItemID.Bacon, 10);

        public override void HitEffect(NPC.HitInfo hit)
        {
            int dustType = Type == ModContent.NPCType<PiggyGold>() ? DustID.GoldCritter : DustID.Blood;
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, dustType, hit.HitDirection, -1f, 0, default, 1f);
            }

            if (NPC.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, dustType, hit.HitDirection, -1f, 0, default, 1f);
                }

                if (!Main.dedServ && Type != ModContent.NPCType<PiggyGold>())
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Piggy").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Piggy2").Type, 1f);
                }
            }

            NPC.direction = hit.HitDirection;
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.velocity.Y == 0f)
            {
                if (!NPC.IsABestiaryIconDummy)
                {
                    if (NPC.velocity.X == 0f)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0.0;
                        return;
                    }
                }

                NPC.frameCounter += NPC.IsABestiaryIconDummy ? 0.6f : Math.Abs(NPC.velocity.X) * 0.25f;
                NPC.frameCounter += 1.0;
                if (NPC.frameCounter > 7.0)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }

                if (NPC.frame.Y / frameHeight >= Main.npcFrameCount[Type] - 1)
                {
                    NPC.frame.Y = frameHeight;
                }
            }
            else
            {
                NPC.frameCounter = 0.0;
                NPC.frame.Y = frameHeight * 2;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                return true;

            Texture2D baseTexture = TextureAssets.Npc[Type].Value;
            Vector2 scale = SquashVector * NPC.scale;
            SpriteEffects spriteEffects = NPC.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(baseTexture, NPC.Center - screenPos + Vector2.UnitY * NPC.gfxOffY, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() * 0.5f, scale, spriteEffects, 0f);

            spriteBatch.End(out var snapshot);

            // Draw the transformation visuals when needed.
            if (AIState == (int)BehaviorState.DivineSwineTransformation)
                Draw_DivineSwineTransformationVisuals(spriteBatch, screenPos);

            spriteBatch.Begin(snapshot);
            return false;
        }

        private void Draw_DivineSwineTransformationVisuals(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Vector2 baseDrawPosition = NPC.Center - screenPos + Vector2.UnitY * NPC.gfxOffY;
            float interpolant = CalamityUtils.SineOutEasing(Utils.GetLerpValue(0f, 120f, Timer, true), 1);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            // Spotlight from heaven.
            float spotlightOpacity = CalamityUtils.SineInOutEasing(Utils.GetLerpValue(0f, 30f, Timer, true) * Utils.GetLerpValue(180f, 150f, Timer, true), 1);
            Vector2 spotlightDrawPosition = baseDrawPosition + Vector2.UnitY * -16f;
            Vector2 spotlightScale = new Vector2(1.3f * interpolant, 1.6f);
            Vector2 spotlightOrigin = new Vector2(SpotlightTexture.Width() * 0.5f, SpotlightTexture.Height());
            spriteBatch.Draw(SpotlightTexture.Value, spotlightDrawPosition, null, Color.White with { A = 0 } * spotlightOpacity, 0f, spotlightOrigin, spotlightScale, SpriteEffects.FlipVertically, 0f);
            
            spriteBatch.End();

            var pixelationLease = RenderTargetPool.Shared.Rent(Main.graphics.GraphicsDevice, Main.screenWidth / 2, Main.screenHeight / 2, RenderTargetDescriptor.Default);
            using (pixelationLease.Scope(clearColor: Color.Transparent))
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, PixelationManager.PixelationMatrix);

                // Typical bloom effects.
                float flareRotation = (float)(Main.timeForVisualEffects / 150f) + NPC.whoAmI;
                spriteBatch.Draw(BloomFlare.Value, baseDrawPosition, null, Color.White with { A = 0 } * interpolant * 0.5f, flareRotation, BloomFlare.Size() * 0.5f, 0.2f * interpolant, 0, 0f);
                spriteBatch.Draw(BloomFlare.Value, baseDrawPosition, null, Color.White with { A = 0 } * interpolant, flareRotation * -0.6f, BloomFlare.Size() * 0.5f, 0.15f * interpolant, 0, 0f);

                spriteBatch.Draw(BloomCircle.Value, baseDrawPosition, null, Color.White with { A = 0 } * interpolant, 0f, BloomCircle.Size() * 0.5f, 0.2f * interpolant, 0, 0f);
                spriteBatch.Draw(ShineFlare.Value, baseDrawPosition, null, Color.White with { A = 0 } * interpolant, 0f, ShineFlare.Size() * 0.5f, 0.1f * interpolant, 0, 0f);

                spriteBatch.End();
            }

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            spriteBatch.Draw(pixelationLease.Target, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 2f, 0, 0f);
            spriteBatch.End();
        }
    }
}
