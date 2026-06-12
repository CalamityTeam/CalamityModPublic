using System;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs.HorribleHog
{
    public partial class HorribleHog
    {
        #region Static Behavior Properties
        public static float EngageDistance => 300f;
        public static float Idle_MaxSpeed => 2f;
        public static float Idle_MaxAcceleration => 0.125f;

        public static int DigTowardsTarget_PreJumpTime => 30;
        public static int DigTowardsTarget_FindSuitablePositionTime => 120;
        public static int DigTowardsTraget_MaxDiggingTime => 180;
        #endregion

        public void MainBehavior_PiggyTransformation()
        {
            NPC.ShowNameOnHover = false;
            NPC.dontTakeDamage = true;
            NPC.damage = 0;
            NPC.velocity.X *= 0.9f;
            NPC.rotation *= 0.9f;
            SpriteRotation *= 0.9f;

            float volumeMultiplier = Utils.GetLerpValue(0f, 45f, Timer, true) * Utils.GetLerpValue(240f, 190f, Timer, true);
            DevilsTongueVolumeMultiplier = volumeMultiplier;

            float tintStrength = Utils.GetLerpValue(0f, 45f, Timer, true);
            TintStrength = tintStrength;
            TintColorTarget = Color.White;

            float shakeStrength = Utils.Remap(Timer, 0f, 180f, 0f, 5f, true);
            HorizontalShakeStrength = shakeStrength;

            float distanceFromHog = Main.LocalPlayer.Distance(NPC.Center);
            if (distanceFromHog < 400f)
            {
                float screenshakeStrength = Utils.Remap(distanceFromHog, 400f, 100f, 0f, 3f, true) * Utils.GetLerpValue(0f, 180f, Timer, true);
                Main.LocalPlayer.SetScreenshake(screenshakeStrength);
            }

            if (Timer >= 45f)
            {
                float smokeOpacity = 1.2f * Utils.GetLerpValue(45f, 75f, Timer, true);
                int smokeAmt = Main.rand.Next(2, 5);
                for (int i = 0; i < smokeAmt; i++)
                {
                    Color color = Color.Lerp(new(30, 30, 30), Color.Crimson, Main.rand.NextBool(3) ? Main.rand.NextFloat(0.2f, 0.8f) : Main.rand.Next(2));
                    int lifetime = Main.rand.Next(30, 45);
                    float scale = Main.rand.NextFloat(1.4f, 1.6f);

                    HeavySmokeParticle circlingSmoke = new(NPC.Center, Main.rand.NextVector2Circular(16f, 16f), color, lifetime, scale, smokeOpacity, 0.01f, affectedByLight: true);
                    GeneralParticleHandler.SpawnParticle(circlingSmoke, true, Enums.GeneralDrawLayer.BeforeNPCs);
                }

                float particleStrengthInterpolant = Utils.GetLerpValue(0f, 180f, Timer, true);
                int lightSpawnRate = (int)MathHelper.Lerp(8, 1, particleStrengthInterpolant);
                if (Main.rand.NextBool(lightSpawnRate))
                {
                    int lightAmt = (int)MathHelper.Lerp(1, 6, particleStrengthInterpolant);
                    for (int i = 0; i < Main.rand.Next(1, lightAmt + 1); i++)
                    {
                        float lightSpeed = MathHelper.Lerp(2f, 4f, particleStrengthInterpolant) * tintStrength;
                        float lightScale = MathHelper.Lerp(0.5f, 0.8f, particleStrengthInterpolant) * Main.rand.NextFloat(0.5f, 1f) * tintStrength;

                        SquishyLightParticle transformLight = new(NPC.Center, Main.rand.NextVector2Circular(lightSpeed, lightSpeed), lightScale, Color.White, Main.rand.Next(30, 45));
                        GeneralParticleHandler.SpawnParticle(transformLight, true, Enums.GeneralDrawLayer.AfterNPCs);
                    }
                }
            }

            if (Timer % 30 == 0f)
            {
                float glowRingScale = Utils.Remap(Timer, 0f, 240f, 0.6f, 1.8f, true);
                Color glowRingColor = Color.Lerp(Color.Crimson, Color.White, Utils.GetLerpValue(0f, 240f, Timer, true));
                CustomPulse transformRing = new(NPC.Center, Vector2.Zero, glowRingColor, "CalamityMod/Particles/BloomRing", Vector2.One, 0f, 0f, glowRingScale, 45);
                GeneralParticleHandler.SpawnParticle(transformRing, false);
            }

            if (Timer >= 240f)
            {
                BloomParticle bloom = new(NPC.Center, Vector2.Zero, Color.White, 0f, 2.4f, 125);
                for (int i = 0; i < 3; i++)
                    GeneralParticleHandler.SpawnParticle(bloom, false, Enums.GeneralDrawLayer.AfterNPCs);

                CustomPulse glowRing = new(NPC.Center, Vector2.Zero, Color.White, "CalamityMod/Particles/BloomRing", Vector2.One, 0f, 0f, 3f, 75);
                GeneralParticleHandler.SpawnParticle(glowRing, false, Enums.GeneralDrawLayer.AfterNPCs);

                for (int i = 0; i < 25; i++)
                {
                    QuickSparkleParticle sparkle = new(NPC.Center, Main.rand.NextVector2Circular(16f, 16f), Color.White, Main.rand.NextFloat(0.6f, 0.8f), Main.rand.Next(45, 60));
                    SquishyLightParticle light = new(NPC.Center, Main.rand.NextVector2Circular(8f, 8f), Main.rand.NextFloat(0.6f, 0.8f), Color.White, Main.rand.Next(45, 60));
                    GeneralParticleHandler.SpawnParticle(Main.rand.NextBool() ? sparkle : light, true, Enums.GeneralDrawLayer.AfterNPCs);
                }

                SoundEngine.PlaySound(SoundID.Item29 with { Pitch = 0.25f, Volume = 0.25f }, NPC.Center);
                if (SoundEngine.TryGetActiveSound(DevilsTongueSlot, out var activeSound))
                    activeSound.Stop();

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.Transform(ModContent.NPCType<Piggy>());
                    NPC.netUpdate = true;
                }
            }

            Animate(IdleFrame, IdleFrame, 0, false, dynamicChanges: true);
        }

        public void MainBehavior_EngageAnimation(Player target)
        {
            if (Timer >= 45f)
            {
                if (Timer == 45f)
                {
                    FrameY = 0;
                    NPC.frameCounter = 0;
                    SoundEngine.PlaySound(SoundID.ForceRoarPitched, NPC.Center);
                }

                if (Timer % 10f == 0f)
                {
                    PulseRing hogRoar = new(NPC.Center, Vector2.Zero, Color.Red, 0f, 1.2f, 20);
                    GeneralParticleHandler.SpawnParticle(hogRoar, true);
                }

                NPC.frameCounter++;
                if (NPC.frameCounter >= 5 && FrameY < MaxFrame_RoarFinish)
                {
                    FrameY++;
                    NPC.frameCounter = 0;
                }
            }

            if (Timer >= 120f)
                SwitchBehavior(specificAttack: BehaviorState.ChasePlayer);

            NPC.velocity.X *= 0.9f;
            NPC.spriteDirection = (target.Center.X > NPC.Center.X).ToDirectionInt();
        }

        public void MainBehavior_LaughAtDeadPlayer()
        {
            if (NPC.velocity.Y == 0f)
            {
                // LMFAOOOOOOOOOOOOOOOOOOOOOOOO
                if (!SoundEngine.TryGetActiveSound(DeathLaughSoundSlot, out var _))
                {
                    DeathLaughSoundSlot = SoundEngine.PlaySound(CackleSound, NPC.Center, (activeSound) =>
                    {
                        activeSound.Position = NPC.Center;
                        return AIState == (int)BehaviorState.LaughAtDeadPlayer;
                    });
                }

                NPC.direction *= -1;
                NPC.velocity.Y -= 4f;
            }

            SearchForTargetEveryFrame = true;
            NPC.defense = 60;
            NPC.damage = 0;
            NPC.velocity.X *= 0.92f;

            float targetAngle = NPC.direction < 0 ? MathHelper.ToRadians(50f) : MathHelper.ToRadians(-50f);
            NPC.rotation = (NPC.velocity.Y != 0f) ? NPC.rotation.AngleLerp(targetAngle, 0.075f) : 0f;

            Animate(MinFrame_Laughing, MaxFrame_Laughing);

            if (Timer >= 130f)
            {
                BehaviorState nextAttack = NPC.HasValidTarget ? BehaviorState.ChasePlayer : BehaviorState.Idle;
                SwitchBehavior(specificAttack: nextAttack);
            }
        }

        public void MainBehavior_DespawnAnimation()
        {
            NPC.damage = 0;

            // Wait until Hog hits the ground before running the actual animation.
            if (LocalAIState == 0f)
            {
                if (NPC.velocity.Y == 0f && Timer > 3f)
                {
                    LocalAIState = 1f;
                    Timer = 0f;
                    NPC.netUpdate = true;
                }

                NPC.GravityMultiplier *= 2f;
            }

            if (LocalAIState == 1f)
            {
                // Jump into the ground and dig away.
                if (Timer < DigTowardsTarget_PreJumpTime)
                {
                    NPC.velocity.X *= 0.8f;
                    HorizontalShakeStrength = Utils.Remap(Timer, 0, DigTowardsTarget_PreJumpTime - 5, 0f, 6f, true);
                    SetSquashVectors(new Vector2(1.24f, 0.84f));
                }

                if (Timer == DigTowardsTarget_PreJumpTime)
                {
                    NPC.velocity.Y -= 6f;
                    NPC.velocity.X += 4f * NPC.direction;
                    HorizontalShakeStrength = 0f;
                    SetSquashVectors();
                }

                if (Timer > DigTowardsTarget_PreJumpTime)
                {
                    if (NPC.velocity.Y == 0f)
                    {
                        int dustAmt = Main.rand.Next(10, 15);
                        for (int i = 0; i < dustAmt; i++)
                        {
                            Vector2 dustPosition = NPC.Bottom + Main.rand.NextVector2Circular(NPC.width, 0f);
                            Vector2 dustVelocity = new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, -2f));
                            Dust.NewDust(dustPosition, 1, 1, DustID.Dirt, dustVelocity.X, dustVelocity.Y, Scale: Main.rand.NextFloat(0.8f, 1.2f));
                        }

                        int dustCloudAmt = Main.rand.Next(9, 13);
                        for (int i = 0; i < dustCloudAmt; i++)
                        {
                            Vector2 spawnPosition = NPC.Bottom + Main.rand.NextVector2Circular(NPC.width, 0f);
                            Vector2 velocity = new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-2f, -1f));
                            Color color = Color.Lerp(Color.SandyBrown, Color.SaddleBrown, Main.rand.NextFloat());
                            float rotationSpeed = Main.rand.NextFloat(0.01f, 0.03f) * Main.rand.NextBool().ToDirectionInt();

                            TimedSmokeParticle dustCloud = new(spawnPosition, velocity, color, color, Main.rand.NextFloat(1.2f, 1.4f), 1f, Main.rand.Next(45, 55), rotationSpeed);
                            GeneralParticleHandler.SpawnParticle(dustCloud, true);
                        }

                        CalamityUtils.AddScreenshakeAt(NPC.Center, 5f);
                        SoundEngine.PlaySound(SoundID.Item70, NPC.Center);

                        NPC.active = false;
                        NPC.netUpdate = true;
                    }
                }
            }

            float idealRotation = (NPC.velocity.Y != 0f) ? NPC.velocity.ToRotation() + (NPC.direction > 0 ? 0f : -MathHelper.Pi) : 0f;
            NPC.rotation = idealRotation;

            int frameSpeed = (int)Utils.Remap(MathF.Abs(NPC.velocity.X), 0f, VomitBarrage_MaxSpeed, 120, 8, true);
            Animate(MinFrame_Walking, MaxFrame_Walking, frameSpeed, true, dynamicChanges: true);
        }

        public void MainBehavior_DeathAnimation()
        {
            SetSquashVectors(VelocityBasedSquashNStretch);
            NPC.rotation = NPC.velocity.ToRotation() + (NPC.direction > 0 ? 0f : -MathHelper.Pi);
            SpriteRotation -= (MathHelper.TwoPi / 15f) * NPC.direction;
            TintColor = Color.Red;

            if (Timer > 2 && (NPC.collideX || NPC.collideY || Collision.SolidCollision(NPC.position, NPC.width, NPC.height)))
            {
                if (MiscAttackCounter >= 3)
                {
                    // Create smoke and throw up a big green "5000" on death just like the pigs in Angry Birds
                    for (int i = 0; i < 17; i++)
                    {
                        int goreType = Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1);
                        Gore.NewGorePerfect(NPC.position, Main.rand.NextVector2Circular(2f, 2f), goreType);
                    }

                    CombatText.NewText(NPC.Hitbox, Color.LawnGreen, 5000);
                    SoundEngine.PlaySound(NPC.DeathSound, NPC.Center);
                    Collision.HitTiles(NPC.position, NPC.velocity, NPC.width, NPC.height);

                    NPC.life = 0;
                    NPC.checkDead();
                    NPC.HitEffect();
                    NPC.netUpdate = true;
                }
                else
                {
                    SoundEngine.PlaySound(NPC.HitSound, NPC.Center);
                    TintStrength = 0.75f;

                    MiscAttackCounter++;
                    NPC.velocity.X = NPC.oldVelocity.X * 0.6f;
                    NPC.velocity.Y = -10f;

                    NPC.netUpdate = true;
                }

                CalamityUtils.AddScreenshakeAt(NPC.Center, 4f);
            }

            UseBalledSprite = true;
            NPC.damage = 0;
            NPC.GravityMultiplier *= 1.28f;
            NPC.dontTakeDamage = true;
        }

        public void MainBehavior_Idle(Player target)
        {
            bool targetIsVisible = target.Distance(NPC.Center) <= EngageDistance && Collision.CanHit(NPC, target);
            bool wasHitByNearestTarget = NPC.HasValidTarget && NPC.justHit;
            if (targetIsVisible || wasHitByNearestTarget)
            {
                SwitchBehavior(specificAttack: BehaviorState.EngageAnimation);
                DoEyeGlintEffect(0.4f);
            }

            // Standing still, occasionally switch directions.
            if (LocalAIState == 0f)
            {
                if (Timer > 0f && Timer % 45f == 0f && Main.rand.NextBool(12))
                {
                    Timer = 0f;
                    LocalAIState = 1f;
                    NPC.netUpdate = true;
                }

                if (NPC.velocity.Y == 0f)
                    NPC.velocity.X *= 0.8f;

                if (Timer % 45f == 0f && Main.rand.NextBool(4))
                    NPC.direction *= -1;
            }

            // Walking around aimlessly.
            if (LocalAIState == 1f)
            {
                if (Timer > 0f && Timer % 45f == 0f && Main.rand.NextBool(6))
                {
                    Timer = 0f;
                    LocalAIState = 0f;
                    NPC.netUpdate = true;
                }

                if (MathF.Abs(NPC.velocity.X) < Idle_MaxSpeed)
                    NPC.velocity.X += Idle_MaxAcceleration * NPC.direction;

                bool shouldJump = NPC.collideX || IsNPCApproachingHole();
                if (NPC.velocity.Y == 0f && shouldJump)
                    NPC.velocity.Y -= 6f;
            }

            // Animation.
            Animate(MinFrame_Walking, MaxFrame_Walking, 5, true, dynamicChanges: true);
            NPC.rotation = 0f;

            // Idle sounds.
            if (NPC.soundDelay == 0f && Main.rand.NextBool(250))
            {
                SetSquashVectors(squashVector: new Vector2(1.24f, 0.84f));
                SoundEngine.PlaySound(IdleSound, NPC.Center);
                NPC.soundDelay = Main.rand.Next(30, 45);
            }

            // Spawn ambient mist around itself while it idles.
            Rectangle tileWorkSpace = GetTileWorkSpaceForMist();
            int tileWorkSpaceWithWidth = tileWorkSpace.X + tileWorkSpace.Width;
            int tileWorkSpaceWithHeight = tileWorkSpace.Y + tileWorkSpace.Height;
            for (int x = tileWorkSpace.X; x < tileWorkSpaceWithWidth; x++)
            {
                for (int y = tileWorkSpace.Y; y < tileWorkSpaceWithHeight; y++)
                {
                    TrySpawningMist(x, y);
                }
            }

            // Tint slightly black.
            TintColor = Color.Black;
            TintStrength = 0.6f;

            NPC.chaseable = false;
            SearchForTargetEveryFrame = true;
            HorizontalShakeStrength = 0f;
        }

        public void MainBehavior_DigTowardsTarget(Player target)
        {
            // Initial animation. 
            if (LocalAIState == 0f)
            {
                if (Timer < DigTowardsTarget_PreJumpTime)
                {
                    NPC.velocity.X *= 0.8f;
                    HorizontalShakeStrength = Utils.Remap(Timer, 0, DigTowardsTarget_PreJumpTime - 5, 0f, 6f, true);
                    SetSquashVectors(new Vector2(1.24f, 0.84f));
                }

                if (Timer == DigTowardsTarget_PreJumpTime)
                {
                    NPC.velocity.Y -= 6f;
                    HorizontalShakeStrength = 0f;
                    SetSquashVectors(squashVector: new Vector2(0.84f, 1.24f));
                    SoundEngine.PlaySound(JumpSound, NPC.Center);
                }

                if (Timer > DigTowardsTarget_PreJumpTime)
                {
                    if (NPC.velocity.Y == 0f)
                    {
                        int dustAmt = Main.rand.Next(10, 15);
                        for (int i = 0; i < dustAmt; i++)
                        {
                            Vector2 dustPosition = NPC.Bottom + Main.rand.NextVector2Circular(NPC.width, 0f);
                            Vector2 dustVelocity = new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, -2f));
                            Dust.NewDust(dustPosition, 1, 1, DustID.Dirt, dustVelocity.X, dustVelocity.Y, Scale: Main.rand.NextFloat(0.8f, 1.2f));
                        }

                        int dustCloudAmt = Main.rand.Next(9, 13);
                        for (int i = 0; i < dustCloudAmt; i++)
                        {
                            Vector2 spawnPosition = NPC.Bottom + Main.rand.NextVector2Circular(NPC.width, 0f);
                            Vector2 velocity = new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-2f, -1f));
                            Color color = Color.Lerp(Color.SandyBrown, Color.SaddleBrown, Main.rand.NextFloat());
                            float rotationSpeed = Main.rand.NextFloat(0.01f, 0.03f) * Main.rand.NextBool().ToDirectionInt();

                            TimedSmokeParticle dustCloud = new(spawnPosition, velocity, color, color, Main.rand.NextFloat(1.2f, 1.4f), 1f, Main.rand.Next(45, 55), rotationSpeed);
                            GeneralParticleHandler.SpawnParticle(dustCloud, true);
                        }

                        NPC.rotation = 0f;
                        SoundEngine.PlaySound(SoundID.Item70, NPC.Center);
                        LocalAIState = 1f;
                        Timer = 0f;
                        SetSquashVectors();
                        NPC.netUpdate = true;
                    }

                    NPC.rotation = MathHelper.Lerp(NPC.rotation, -MathHelper.PiOver2, 0.175f);
                }
            }

            // Search for a position around the player to emerge.
            if (LocalAIState == 1f)
            {
                NPC.damage = 0;
                NPC.dontTakeDamage = true;
                NPC.Opacity = 0f;

                // Find closest spot around player.
                if (Timer <= DigTowardsTarget_FindSuitablePositionTime)
                {
                    DiggingEmergeSpot = FindSuitableGround(target.Bottom.ToTileCoordinates()).ToWorldCoordinates();
                }
                else
                {
                    float distanceInterpolant = Utils.GetLerpValue(800f, 200f, target.Distance(DiggingEmergeSpot), true);
                    float screenshake = Utils.Remap(Timer, DigTowardsTarget_FindSuitablePositionTime, DigTowardsTraget_MaxDiggingTime, 1f, 4f, true) * distanceInterpolant;
                    target.SetScreenshake(screenshake);
                }

                if (Main.rand.NextBool(3))
                {
                    int dustAmt = Main.rand.Next(1, 3);
                    for (int i = 0; i < dustAmt; i++)
                    {
                        Vector2 dustPosition = DiggingEmergeSpot + Main.rand.NextVector2Circular(NPC.width, 0f);
                        Vector2 dustVelocity = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-5f, -3f));
                        Dust.NewDust(dustPosition, 1, 1, DustID.Dirt, dustVelocity.X, dustVelocity.Y, Scale: Main.rand.NextFloat(0.8f, 1.2f));
                    }
                }

                int dustCloudAmt = Main.rand.Next(1, 3);
                for (int i = 0; i < dustCloudAmt; i++)
                {
                    Vector2 spawnPosition = DiggingEmergeSpot + Main.rand.NextVector2Circular(NPC.width, 0f);
                    Vector2 velocity = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-5f, -3f));
                    Color color = Color.Lerp(Color.SandyBrown, Color.SaddleBrown, Main.rand.NextFloat());
                    float rotationSpeed = Main.rand.NextFloat(0.01f, 0.03f) * Main.rand.NextBool().ToDirectionInt();

                    TimedSmokeParticle dustCloud = new(spawnPosition, velocity, color, color, Main.rand.NextFloat(0.6f, 0.8f), 1f, Main.rand.Next(25, 45), rotationSpeed);
                    GeneralParticleHandler.SpawnParticle(dustCloud, true, Enums.GeneralDrawLayer.BeforeSolidTiles);
                }

                // Play digging sounds.
                if (Timer <= DigTowardsTarget_FindSuitablePositionTime)
                {
                    if (!SoundEngine.TryGetActiveSound(DiggingSoundSlot, out _))
                    {
                        DiggingSoundSlot = SoundEngine.PlaySound(DiggingSlowSound, NPC.Center, (activeSound) =>
                        {
                            activeSound.Position = DiggingEmergeSpot;
                            return AIState == (int)BehaviorState.DigTowardsTarget && Timer <= 120;
                        });
                    }
                }
                else
                {
                    if (!SoundEngine.TryGetActiveSound(DiggingSoundSlot, out _))
                    {
                        DiggingSoundSlot = SoundEngine.PlaySound(DiggingFastSound, NPC.Center, (activeSound) =>
                        {
                            activeSound.Position = DiggingEmergeSpot;
                            activeSound.Volume = 1.5f;
                            return AIState == (int)BehaviorState.DigTowardsTarget && Timer >= 120 && Timer <= 180;
                        });
                    }
                }

                // Jump out.
                if (Timer >= DigTowardsTraget_MaxDiggingTime)
                {
                    CalamityUtils.AddScreenshakeAt(DiggingEmergeSpot, 4f);
                    SoundEngine.PlaySound(SoundID.Item70, DiggingEmergeSpot);

                    NPC.Center = DiggingEmergeSpot;
                    NPC.velocity = CalamityUtils.GetProjectilePhysicsFiringVelocity(NPC.Center, target.Center, NPC.gravity, 7f) * new Vector2(1f, 1.45f);
                    DoJumpEffects();

                    LocalAIState = 2f;
                    Timer = 0f;
                    NPC.netUpdate = true;
                }

                AfterimageTrailOpacity = MathHelper.Lerp(AfterimageTrailOpacity, 0f, 0.15f);
                NPC.rotation = NPC.rotation.AngleLerp(0f, 0.075f);
                SpriteRotation = SpriteRotation.AngleLerp(0f, 0.075f);
            }

            // Land after jumping and go back to attacking.
            if (LocalAIState == 2f)
            {
                NPC.Opacity = MathHelper.Lerp(NPC.Opacity, 1f, 0.085f);
                if (NPC.velocity.Y == 0f && Timer > 2f)
                {
                    DoEyeGlintEffect(0.4f);
                    SwitchBehavior(specificAttack: BehaviorState.ChasePlayer);
                }

                float idealRotation = (NPC.velocity.Y != 0f) ? NPC.velocity.ToRotation() + (NPC.direction > 0 ? 0f : -MathHelper.Pi) : 0f;
                NPC.rotation = idealRotation;
                NPC.direction = (target.Center.X > NPC.Center.X).ToDirectionInt();
            }

            int frameSpeed = (int)Utils.Remap(MathF.Abs(NPC.velocity.X), 0f, VomitBarrage_MaxSpeed, 120, 8, true);
            Animate(MinFrame_Walking, MaxFrame_Walking, frameSpeed, true, dynamicChanges: true);

            DigTimer = 0f;
            SearchForTargetEveryFrame = true;
        }
    }
}
