using System;
using System.Collections.Generic;
using CalamityMod.Graphics.Metaballs;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
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
        public static int Damage_HogCharge => 14;
        public static int Damage_ShockwaveProjectile => 13;
        public static int Damage_ShockwaveRubbleProjectile => 12;
        public static int Damage_VomitChunkProjectile => 13;
        public static int Damage_VomitBombProjectile => 16;
        public static int Damage_VomitEyeProjectile => 13;

        public static int ChaseTime => 180;
        public static float ChasePlayer_MaxSpeed => 5f;
        public static float ChasePlayer_MaxAcceleration => 0.3f;

        public static int HogCharge_PreChargeTime => 75;
        public static int HogCharge_ChargingTime => 240;
        public static int HogCharge_ChargeCooldownTime => CalamityWorld.revenge ? -20 : Main.expertMode ? -30 : -45;
        public static int HogCharge_PreDashTime => 45;
        public static int HogCharge_DashTime => 20;
        public static int HogCharge_MaxCharges => CalamityWorld.revenge ? 4 : Main.expertMode ? 3 : 2;
        public static float HogCharge_MaxSpeed => CalamityWorld.revenge ? 18f : Main.expertMode ? 15f : 12f;
        public static float HogCharge_MaxAcceleration => CalamityWorld.revenge ? 0.425f : Main.expertMode ? 0.4f : 0.375f;
        public static float HogCharge_MaxSlowdownDistance => CalamityWorld.revenge ? 64f : Main.expertMode ? 96f : 128f;

        public static int JumpAndDash_PreJumpTime => 75;
        public static int JumpAndDash_JumpPreDashTime => 45;
        public static int JumpAndDash_DashTime => 20;
        public static int JumpAndDash_CooldownTime => 60;
        public static int JumpAndDash_PreVomitTime => 60;
        public static int JumpAndDash_PreGroundPoundTime => 45;
        public static int JumpAndDash_MaxVomitChunks => CalamityWorld.revenge ? 5 : Main.expertMode ? 4 : 3;
        public static int JumpAndDash_VomitTimeInterval => CalamityWorld.revenge ? 10 : 15;
        public static int JumpAndDash_MaxBounces => Main.expertMode ? 3 : 2;
        public static float JumpAndDash_MaxJumpHeight => 14f;
        public static float JumpAndDash_MaxDashSpeed => 20f;
        public static float JumpAndDash_MaxBounceSpeed => CalamityWorld.revenge ? 16f : Main.expertMode ? 14f : 12f;

        public static int HorribleHoller_RoarTime => 75;
        public static int HorribleHoller_PostRoarCooldownTime => 30;
        public static int HorribleHoller_MaxZombies => 3;

        public static int VomitBarrage_PreJumpTime => 75;
        public static int VomitBarrage_VomitTime => 25;
        public static int VomitBarrage_PostVomitCooldown => 100;
        public static int VomitBarrage_MaxVomitChunks => CalamityWorld.revenge ? 10 : Main.expertMode ? 8 : 6;
        public static int VomitBarrage_MinVomitBombs => CalamityWorld.death ? 3 : CalamityWorld.revenge ? 2 : 0;
        public static int VomitBarrage_MaxVomitBombs => CalamityWorld.death ? 6 : CalamityWorld.revenge ? 5 : Main.expertMode ? 3 : 4;
        public static float VomitBarrage_MaxSpeed => 8f;
        public static float VomitBarrage_MaxAcceleration => 0.45f;
        #endregion

        public void MainBehavior_ChasePlayer(Player target)
        {
            // Whenever the player is able to be hit, don't modify velocity mid-air.
            // Otherwise, move as any grounded NPC would in order to get back towards the player.
            bool canHitPlayer = NPC.velocity.Y == 0f && Collision.CanHit(NPC, target);
            if (canHitPlayer || !Collision.CanHit(NPC, target))
            {
                NPC.direction = (target.Center.X > NPC.Center.X).ToDirectionInt();
                GroundedMovement(target.Center, ChasePlayer_MaxSpeed, ChasePlayer_MaxAcceleration);
            }

            bool jumpUpToPlayer = Main.expertMode && NPC.Center.Y - target.Center.Y >= 64f;
            bool closeToPlayer = MathHelper.Distance(NPC.Center.X, target.Center.X) < 112f;
            if (NPC.velocity.Y == 0f && jumpUpToPlayer && closeToPlayer)
            {
                NPC.velocity.Y -= 8f;
                SoundEngine.PlaySound(JumpSound, NPC.Center);
            }

            bool closeEnoughToAttack = MathHelper.Distance(NPC.Center.X, target.Center.X) <= 64f;
            if (Timer >= ChaseTime || (closeEnoughToAttack && Timer >= 120f && NPC.velocity.Y == 0f))
            {
                List<BehaviorState> possibleAttacks = [BehaviorState.JumpAndDash];
                if (NPC.Center.Y - target.Center.Y < 128)
                    possibleAttacks.Add(BehaviorState.HogCharge);
                SwitchBehavior(BehaviorState.ChasePlayer, null, [.. possibleAttacks]);
            }

            float idealRotation = (NPC.velocity.Y != 0f) ? NPC.velocity.ToRotation() + (NPC.direction > 0 ? 0f : -MathHelper.Pi) : 0f;
            NPC.rotation = idealRotation;

            int frameSpeed = (int)Utils.Remap(MathF.Abs(NPC.velocity.X), 0f, ChasePlayer_MaxSpeed, 9, 3, true);
            Animate(MinFrame_Walking, MaxFrame_Walking, frameSpeed, true, dynamicChanges: true);
        }

        public void MainBehavior_HogCharge(Player target)
        {
            if (LocalAIState == 0f)
            {
                if (Timer <= HogCharge_PreChargeTime)
                {
                    float backupSpeed = MathHelper.Lerp(0.08f, 0.16f, Timer / HogCharge_PreChargeTime);
                    if (NPC.velocity.Y == 0f)
                    {
                        if (MathF.Abs(NPC.velocity.X) > 0.4f)
                            NPC.velocity.X *= 0.9f;
                        else
                            NPC.velocity.X -= backupSpeed * NPC.direction;
                    }

                    Animate(MinFrame_Walking, MaxFrame_Walking, 7, true, dynamicChanges: true);
                    NPC.direction = (target.Center.X > NPC.Center.X).ToDirectionInt();
                }

                if (Timer >= HogCharge_PreChargeTime)
                {
                    Timer = 0f;
                    LocalAIState = 1f;
                    SoundEngine.PlaySound(SoundID.Zombie38, NPC.Center);
                    NPC.netUpdate = true;
                }

                NPC.rotation = 0f;
                if (Timer % 15f == 0f && NPC.velocity.Y == 0f)
                    SoundEngine.PlaySound(SoundID.Run with { Pitch = -0.8f, Identifier = "Horrible Hog Run" }, NPC.Center);
            }

            if (LocalAIState == 1f)
            {
                bool finishedWithCharges = Timer >= HogCharge_ChargingTime || MiscAttackCounter >= HogCharge_MaxCharges;
                if (Timer <= 0f)
                {
                    // Kill the hitbox when slowing down.
                    KillChargeHitboxProjectile();

                    if (NPC.velocity.Y == 0f)
                        NPC.velocity.X *= 0.93f;
                    NPC.direction = (target.Center.X > NPC.Center.X).ToDirectionInt();
                    NPC.rotation = 0f;

                    Vector2 dustPosition = new(NPC.Bottom.X + Main.rand.NextFloat(-NPC.width * 0.5f, NPC.width * 0.5f), NPC.Bottom.Y);
                    Dust.NewDustPerfect(dustPosition, DustID.Cloud, new Vector2(NPC.velocity.X * 0.4f, Main.rand.NextFloat(-0.38f, 0.38f)), 120, Color.LightGray, Main.rand.NextFloat(1f, 1.2f));

                    AfterimageTrailOpacity = MathHelper.Lerp(AfterimageTrailOpacity, 0f, 0.15f);
                    if (Timer == -10 && !finishedWithCharges)
                        DoEyeGlintEffect(0.4f);

                    FrameY = 20;
                }
                else
                {
                    // Spawn an artificial hitbox that can knockback and hit both hostile and friendly enemies alongside the player.
                    if (!CalamityUtils.AnyProjectiles(ModContent.ProjectileType<HorribleHogChargeHitbox>()) && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float knockback = 20f;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<HorribleHogChargeHitbox>(), Damage_HogCharge, knockback, ai0: NPC.whoAmI);
                    }

                    float maxSpeed = MathHelper.Lerp(HogCharge_MaxSpeed, HogCharge_MaxSpeed + 3.5f, MiscAttackCounter / HogCharge_MaxCharges);
                    float maxAcceleration = MathHelper.Lerp(HogCharge_MaxAcceleration, HogCharge_MaxAcceleration + 0.35f, MiscAttackCounter / HogCharge_MaxCharges);
                    if (MathF.Abs(NPC.velocity.X) < maxSpeed)
                        NPC.velocity.X += maxAcceleration * NPC.direction;

                    // Turn around if the player jumps over to the other side of it.
                    bool turnLeft = NPC.direction == 1 && (NPC.Center.X - target.Center.X) > HogCharge_MaxSlowdownDistance;
                    bool turnRight = NPC.direction == -1 && (NPC.Center.X - target.Center.X) < -HogCharge_MaxSlowdownDistance;
                    bool hitAWall = NPC.collideX && MathF.Abs(NPC.velocity.X) >= maxSpeed * 0.8f;
                    if (((turnLeft || turnRight) && Timer > 30f) || finishedWithCharges || hitAWall)
                    {
                        if (hitAWall)
                        {
                            NPC.velocity.X = NPC.oldVelocity.X * -0.64f;
                            NPC.velocity.Y -= 5f;
                        }

                        Timer = HogCharge_ChargeCooldownTime;
                        MiscAttackCounter++;
                        if (finishedWithCharges && PhaseTwo)
                            AltAttackVariant = Main.rand.Next(2);

                        NPC.netUpdate = true;
                    }

                    Vector2 dustPosition = new(NPC.Bottom.X + Main.rand.NextFloat(-NPC.width * 0.5f, NPC.width * 0.5f), NPC.Bottom.Y);
                    Dust.NewDustPerfect(dustPosition, DustID.Cloud, new Vector2(NPC.velocity.X * 0.2f, Main.rand.NextFloat(-0.3f, 0.3f)), 120, Color.LightGray, Main.rand.NextFloat(1f, 1.2f));
                    if (Timer % 7 == 0f && NPC.velocity.Y == 0f)
                        SoundEngine.PlaySound(SoundID.Run with { Pitch = -0.4f, Identifier = "Horrible Hog Run" }, NPC.Center);

                    AfterimageTrailOpacity = MathHelper.Lerp(AfterimageTrailOpacity, 1f, 0.15f);

                    if (MiscAttackCounter >= HogCharge_MaxCharges + 1)
                    {
                        KillChargeHitboxProjectile();
                        if (PhaseTwo && AltAttackVariant == 1f)
                        {
                            LocalAIState = 2f;
                            Timer = 0f;
                            NPC.netUpdate = true;
                        }
                        else
                        {
                            List<BehaviorState> possibleAttacks = [BehaviorState.HorribleHoller, BehaviorState.JumpAndDash, BehaviorState.VomitBarrage];
                            SwitchBehavior(BehaviorState.HogCharge, null, [.. possibleAttacks]);
                        }
                    }

                    int frameSpeed = (int)Utils.Remap(MathF.Abs(NPC.velocity.X), 0f, maxSpeed, 6, 2, true);
                    Animate(MinFrame_Walking, MaxFrame_Walking, frameSpeed, true, dynamicChanges: true);

                    NPC.damage = 0;
                    NPC.rotation = 0f;
                }
            }

            // Alt attack in phase two.
            // Same jump and dash but with a smaller shockwave.
            if (LocalAIState == 2f)
                AltBehavior_HogCharge_JumpAndDash(target);

            // Rebounding, land and switch attacks.
            if (LocalAIState == 3f)
            {
                if (NPC.velocity.Y == 0f && Timer > 2f)
                {
                    List<BehaviorState> possibleAttacks = [BehaviorState.HorribleHoller, BehaviorState.JumpAndDash, BehaviorState.VomitBarrage];
                    SwitchBehavior(BehaviorState.HogCharge, null, [.. possibleAttacks]);
                }

                NPC.rotation = NPC.rotation.AngleLerp(0f, 0.075f);
                SpriteRotation = SpriteRotation.AngleLerp(0f, 0.075f);
            }
        }

        public void AltBehavior_HogCharge_JumpAndDash(Player target)
        {
            if (Timer == 0f)
            {
                NPC.velocity.Y = -JumpAndDash_MaxJumpHeight * 1.2f;
                NPC.velocity.X += JumpAndDash_MaxDashSpeed * 0.4f * NPC.direction;
                DoJumpEffects();
                UseBalledSprite = true;
            }

            if (Timer <= HogCharge_PreDashTime)
            {
                NPC.velocity.Y *= 0.98f;
                NPC.velocity.X *= 0.96f;

                SetSquashVectors(new Vector2(1.06f, 0.94f));
                NPC.rotation = NPC.velocity.ToRotation() + (NPC.direction > 0 ? 0f : -MathHelper.Pi);
                SpriteRotation -= (MathHelper.TwoPi / 20f) * NPC.direction;

                if (Timer <= HogCharge_PreDashTime - 10)
                {
                    NPC.direction = (target.Center.X > NPC.Center.X).ToDirectionInt();
                    LastPlayerPosition = target.Center;
                    if (Timer == HogCharge_PreDashTime - 10)
                        DoEyeGlintEffect(0.6f);
                }
            }

            if (Timer == HogCharge_PreDashTime)
            {
                NPC.noGravity = true;
                NPC.velocity = NPC.SafeDirectionTo(LastPlayerPosition) * JumpAndDash_MaxDashSpeed;

                UseBalledSprite = false;
                FrameY = JumpFrame;
                SetSquashVectors(new Vector2(1.12f, 0.96f));
                SpriteRotation = 0f;
                NPC.rotation = NPC.velocity.ToRotation() + (NPC.direction > 0 ? 0f : -MathHelper.Pi);
                SoundEngine.PlaySound(SoundID.Zombie38, NPC.Center);
            }

            if (Timer >= HogCharge_PreDashTime && Timer <= HogCharge_PreDashTime + HogCharge_DashTime)
            {
                if (NPC.collideX || NPC.collideY || Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                {
                    // Spawn shockwave on tile impact.
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < 2; i++)
                            SpawnShockwave(2, 6, (i > 0).ToDirectionInt(), 1f, 2f);
                    }

                    CalamityUtils.AddScreenshakeAt(NPC.Center, 6f);
                    Collision.HitTiles(NPC.position, NPC.velocity, NPC.width, NPC.height);
                    SoundEngine.PlaySound(GroundImpactSound, NPC.Center);

                    UseBalledSprite = true;
                    NPC.velocity.X = NPC.DirectionFrom(target.Center).SafeNormalize(Vector2.UnitX).X * 3f;
                    NPC.velocity.Y = -10f;
                    LocalAIState = 3f;
                    Timer = 0f;
                    NPC.netUpdate = true;
                }

                NPC.noGravity = true;
                NPC.rotation = NPC.velocity.ToRotation() + (NPC.direction > 0 ? 0f : -MathHelper.Pi);
                FrameY = JumpFrame;
            }

            if (Timer >= HogCharge_PreDashTime + HogCharge_DashTime)
            {
                if (NPC.velocity.Y == 0f)
                {
                    UseBalledSprite = false;
                    FrameY = IdleFrame;
                    SetSquashVectors();
                    LocalAIState = 3f;
                    Timer = 0f;
                    NPC.netUpdate = true;
                }

                NPC.GravityMultiplier *= 3f;
                NPC.velocity.X *= 0.95f;
                SpriteRotation = 0f;
                UseBalledSprite = true;
            }

            SetSquashVectors(VelocityBasedSquashNStretch);
        }

        public void MainBehavior_JumpAndDash(Player target)
        {
            // Stop, jump and dash towards the target.
            if (LocalAIState == 0f)
            {
                if (Timer <= JumpAndDash_JumpPreDashTime + JumpAndDash_PreJumpTime)
                {
                    NPC.direction = (target.Center.X > NPC.Center.X).ToDirectionInt();
                    if (Timer < JumpAndDash_JumpPreDashTime + JumpAndDash_PreJumpTime - 15)
                        LastPlayerPosition = target.Center;
                }

                if (Timer <= JumpAndDash_PreJumpTime)
                {
                    GroundedMovement(target.Center, ChasePlayer_MaxSpeed, ChasePlayer_MaxAcceleration, slowdownDistance: 160f);
                    float targetAngle = (NPC.velocity.Y != 0f) ? NPC.velocity.X * 0.175f * (NPC.velocity.Y < 0).ToDirectionInt() : 0f;
                    NPC.rotation = NPC.rotation.AngleLerp(targetAngle, 0.075f);

                    int preJumpVisualsTime = JumpAndDash_PreJumpTime - 30;
                    if (Timer >= preJumpVisualsTime)
                    {
                        NPC.velocity.X *= 0.96f;
                        float interpolant = Utils.GetLerpValue(preJumpVisualsTime, VomitBarrage_PreJumpTime, Timer, true);
                        HorizontalShakeStrength = MathHelper.Lerp(0f, 6f, interpolant);
                        SetSquashVectors(new Vector2(1.24f, 0.84f));
                        FrameY = BalledUpFrame;
                    }

                    int frameSpeed = (int)Utils.Remap(MathF.Abs(NPC.velocity.X), 0f, ChasePlayer_MaxSpeed, 8, 2, true);
                    Animate(MinFrame_Walking, MaxFrame_Walking, frameSpeed, true, dynamicChanges: true);
                }

                if (Timer == JumpAndDash_PreJumpTime)
                {
                    NPC.velocity.Y = -JumpAndDash_MaxJumpHeight;
                    NPC.velocity.X += JumpAndDash_MaxDashSpeed * 0.2f * NPC.direction;

                    DoJumpEffects();
                    UseBalledSprite = true;
                    SetSquashVectors(new Vector2(0.84f, 1.14f));
                    HorizontalShakeStrength = 0f;
                }

                if (Timer >= JumpAndDash_PreJumpTime && Timer <= JumpAndDash_JumpPreDashTime + JumpAndDash_PreJumpTime)
                {
                    NPC.velocity.Y *= 0.98f;
                    NPC.velocity.X *= 0.96f;

                    NPC.rotation = NPC.velocity.ToRotation() + (NPC.direction > 0 ? 0f : -MathHelper.Pi);
                    SpriteRotation -= (MathHelper.TwoPi / 20f) * NPC.direction;
                    AfterimageTrailOpacity = MathHelper.Lerp(AfterimageTrailOpacity, 1f, 0.15f);
                    if (Timer == JumpAndDash_JumpPreDashTime + JumpAndDash_PreJumpTime - 10)
                        DoEyeGlintEffect(0.4f);
                }

                if (Timer == JumpAndDash_JumpPreDashTime + JumpAndDash_PreJumpTime)
                {
                    NPC.noGravity = true;
                    NPC.velocity = NPC.SafeDirectionTo(LastPlayerPosition) * JumpAndDash_MaxDashSpeed;

                    UseBalledSprite = false;
                    SpriteRotation = 0f;
                    NPC.rotation = NPC.velocity.ToRotation() + (NPC.direction > 0 ? 0f : -MathHelper.Pi);
                    SoundEngine.PlaySound(SoundID.Zombie38, NPC.Center);
                }

                if (Timer >= JumpAndDash_JumpPreDashTime + JumpAndDash_PreJumpTime && Timer <= JumpAndDash_JumpPreDashTime + JumpAndDash_PreJumpTime + JumpAndDash_DashTime)
                {
                    if (NPC.collideX || NPC.collideY || Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                    {
                        // Spawn shockwave on tile impact.
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                int shockwaveAmt = PhaseTwo ? 14 : 8;
                                SpawnShockwave(2, shockwaveAmt, (i > 0).ToDirectionInt(), 1f, 3f);
                            }
                        }

                        NPC.velocity.X = NPC.DirectionFrom(target.Center).SafeNormalize(Vector2.UnitX).X * 5f;
                        NPC.velocity.Y = PhaseTwo ? -16f : -12f;
                        LocalAIState = 1f;
                        Timer = 0f;
                        AltAttackVariant = Main.rand.Next(2);

                        UseBalledSprite = true;
                        CalamityUtils.AddScreenshakeAt(NPC.Center, 6f);
                        Collision.HitTiles(NPC.position, NPC.velocity, NPC.width, NPC.height);
                        SoundEngine.PlaySound(GroundImpactSound, NPC.Center);

                        NPC.netUpdate = true;
                    }

                    NPC.noGravity = true;
                    NPC.rotation = NPC.velocity.ToRotation() + (NPC.direction > 0 ? 0f : -MathHelper.Pi);
                    FrameY = JumpFrame;
                }

                if (Timer >= JumpAndDash_JumpPreDashTime + JumpAndDash_PreJumpTime + JumpAndDash_DashTime)
                {
                    if (NPC.velocity.Y == 0f)
                    {
                        UseBalledSprite = false;
                        FrameY = IdleFrame;
                        LocalAIState = 2f;
                        Timer = 0f;
                        NPC.netUpdate = true;
                    }

                    NPC.GravityMultiplier *= 3f;
                    NPC.velocity.X *= 0.95f;
                    AfterimageTrailOpacity = MathHelper.Lerp(AfterimageTrailOpacity, 0f, 0.15f);
                }

                if (Timer > JumpAndDash_PreJumpTime)
                    SetSquashVectors(VelocityBasedSquashNStretch);
            }

            // Rebounding after ground collision.
            if (LocalAIState == 1f)
            {
                // Randomly perform two smaller attacks right after the main dash in Phase two.
                if (PhaseTwo)
                {
                    // Alt attck #1:
                    // Shoot vomit chunks at the player.
                    // Shoot more in higher difficulties.
                    //
                    // Alt attack #2:
                    // Bounce on the ground near the player predictively.
                    // In Death Mode and above, perform a large ground pound after the last bounce.

                    if (AltAttackVariant == 0f)
                        AltBehavior_JumpAndDash_VomitChunks(target);
                    else if (AltAttackVariant == 1f)
                        AltBehavior_JumpAndDash_GroundPound(target);
                }
                else
                {
                    if (NPC.collideY && Timer > 2f)
                    {
                        LocalAIState = 2f;
                        Timer = 0f;
                        NPC.netUpdate = true;
                    }

                    SetSquashVectors();
                    SpriteRotation -= (MathHelper.TwoPi / 20f) * NPC.direction;
                    NPC.rotation = NPC.velocity.ToRotation() + (NPC.direction > 0 ? 0f : -MathHelper.Pi);
                    AfterimageTrailOpacity = MathHelper.Lerp(AfterimageTrailOpacity, 0f, 0.15f);
                }
            }

            // Landed back on the ground.
            if (LocalAIState == 2f)
            {
                NPC.velocity.X *= 0.94f;
                if (NPC.velocity.Y == 0f)
                {
                    UseBalledSprite = false;
                    NPC.rotation = 0f;
                    SpriteRotation = 0f;
                }

                AfterimageTrailOpacity = MathHelper.Lerp(AfterimageTrailOpacity, 0f, 0.15f);
                NPC.rotation = NPC.rotation.AngleLerp(0f, 0.075f);
                SpriteRotation = SpriteRotation.AngleLerp(0f, 0.075f);
                SetSquashVectors();
                FrameY = IdleFrame;

                if (Timer >= JumpAndDash_CooldownTime && NPC.velocity.Y == 0f)
                {
                    List<BehaviorState> possibleAttacks = [BehaviorState.VomitBarrage, BehaviorState.HorribleHoller];
                    if (NPC.Center.Y - target.Center.Y < 128)
                        possibleAttacks.Add(BehaviorState.HogCharge);

                    SwitchBehavior(BehaviorState.JumpAndDash, null, [.. possibleAttacks]);
                }
            }
        }

        public void AltBehavior_JumpAndDash_VomitChunks(Player target)
        {
            if (Timer <= JumpAndDash_PreVomitTime)
            {
                NPC.velocity *= 0.98f;
                NPC.GravityMultiplier *= 0.84f;
                if (Timer == JumpAndDash_PreVomitTime - 10)
                    DoEyeGlintEffect(0.6f);
            }

            if (Timer >= JumpAndDash_PreVomitTime)
            {
                if (Timer % JumpAndDash_VomitTimeInterval == 0f && MiscAttackCounter < JumpAndDash_MaxVomitChunks)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int maxVomitPerShot = 2;
                        for (int i = 0; i < maxVomitPerShot; i++)
                        {
                            Vector2 chunkVelocity = NPC.SafeDirectionTo(target.Center).RotatedBy(MathHelper.ToRadians(MathHelper.Lerp(-22f, 22f, i / (maxVomitPerShot - 1)))) * 14f;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, chunkVelocity, ModContent.ProjectileType<HorribleHogVomitChunk>(), Damage_VomitChunkProjectile, 0f);
                        }
                    }

                    Vector2 knockbackVelocity = NPC.DirectionFrom(target.Center).SafeNormalize(-Vector2.UnitY);
                    NPC.velocity = knockbackVelocity * new Vector2(3f, 5f);

                    FrameY = VomitFrame;
                    SoundEngine.PlaySound(SoundID.DD2_OgreSpit, NPC.Center);
                    MiscAttackCounter++;
                    NPC.netUpdate = true;
                }

                if (MiscAttackCounter >= JumpAndDash_MaxVomitChunks && NPC.velocity.Y == 0f)
                {
                    FrameY = JumpFrame;
                    LocalAIState = 2f;
                    Timer = 0f;
                    NPC.netUpdate = true;
                }

                FrameY = JumpFrame;
            }

            SetSquashVectors();
            NPC.direction = (target.Center.X > NPC.Center.X).ToDirectionInt();
            NPC.rotation = NPC.rotation.AngleLerp(NPC.AngleTo(target.Center) + (NPC.direction > 0 ? 0f : -MathHelper.Pi), 0.15f);
            SpriteRotation = 0f;
            AfterimageTrailOpacity = MathHelper.Lerp(AfterimageTrailOpacity, 0f, 0.15f);
        }

        public void AltBehavior_JumpAndDash_GroundPound(Player target)
        {
            // Using plus 1 here cause the first bounce is coming off of the rebound.
            if (MiscAttackCounter <= JumpAndDash_MaxBounces)
            {
                if (NPC.velocity.Y == 0f)
                {
                    if (MiscAttackCounter == JumpAndDash_MaxBounces)
                    {
                        if (!CalamityWorld.death)
                        {
                            LocalAIState = 2f;
                        }
                        else
                        {
                            // Try to bounce in front of the player for the ground pound.
                            Vector2 bounceVelocity = (target.Center.X > NPC.Center.X) ? new Vector2(18f, -20f) : new Vector2(-18f, -20f);
                            NPC.velocity = bounceVelocity;
                        }

                        Timer = 0f;
                    }
                    else
                    {
                        float speedByDistance = Utils.Remap(MathHelper.Distance(target.Center.X, NPC.Center.X), 60f, 600f, 4f, 10f, true);
                        float xVelocity = speedByDistance * (target.Center.X > NPC.Center.X).ToDirectionInt();
                        NPC.velocity = new Vector2(xVelocity, -11.25f);
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient && MiscAttackCounter > 0f)
                    {
                        for (int i = 0; i < 2; i++)
                            SpawnShockwave(2, 4, (i > 0).ToDirectionInt(), 1f, 2f);

                        for (int i = 0; i < MiscAttackCounter; i++)
                        {
                            Vector2 rubbleVelocityRight = new Vector2(2f + i * 3f, -8f - i * 1.25f);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom, rubbleVelocityRight, ModContent.ProjectileType<HorribleHogRubble>(), Damage_ShockwaveProjectile, 1f);
                            Vector2 rubbleVelocityLeft = new Vector2(-2f - i * 3f, -8f - i * 1.25f);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom, rubbleVelocityLeft, ModContent.ProjectileType<HorribleHogRubble>(), Damage_ShockwaveProjectile, 1f);
                        }
                    }

                    CalamityUtils.AddScreenshakeAt(NPC.Center, 4f);
                    SoundEngine.PlaySound(HitSound with { Pitch = -0.4f }, NPC.Center);
                    MiscAttackCounter++;
                    NPC.netUpdate = true;
                }

                if (NPC.velocity.Y > 1f && NPC.velocity.Y < 24f && MiscAttackCounter > 0f)
                {
                    NPC.velocity.Y *= 1.12f;
                    NPC.velocity.X *= 0.96f;
                    NPC.noGravity = true;
                }
            }

            // Ground pound attack in Death Mode.
            if (CalamityWorld.death && MiscAttackCounter >= JumpAndDash_MaxBounces + 1)
            {
                if (Timer < JumpAndDash_PreGroundPoundTime)
                {
                    NPC.velocity *= 0.96f;
                    NPC.GravityMultiplier *= 0.4f;
                    if (Timer == JumpAndDash_PreGroundPoundTime - 10)
                        DoEyeGlintEffect(0.6f);
                }
                else
                {
                    NPC.noGravity = true;
                }

                if (Timer == JumpAndDash_PreGroundPoundTime)
                {
                    NPC.velocity.Y = 30f;
                    NPC.velocity.X *= 0f;
                }

                if (Timer > JumpAndDash_PreGroundPoundTime && NPC.velocity.Y == 0f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        SpawnShockwave(2, 20, (target.Center.X > NPC.Center.X).ToDirectionInt(), 1f, 8f);

                    NPC.velocity.X = NPC.DirectionFrom(target.Center).SafeNormalize(Vector2.UnitX).X * 4f;
                    NPC.velocity.Y = -12f;
                    LocalAIState = 2f;
                    Timer = 0f;

                    CalamityUtils.AddScreenshakeAt(NPC.Center, 8f);
                    Collision.HitTiles(NPC.position, NPC.velocity, NPC.width, NPC.height);
                    SoundEngine.PlaySound(GroundImpactSound, NPC.Center);

                    NPC.netUpdate = true;
                }
            }

            NPC.damage = 0;
            NPC.rotation = NPC.velocity.ToRotation() + (NPC.direction > 0 ? 0f : -MathHelper.Pi);
            SpriteRotation -= (MathHelper.TwoPi / 20f) * NPC.direction;
            SetSquashVectors(VelocityBasedSquashNStretch);
            UseBalledSprite = true;
        }

        public void MainBehavior_HorribleHoller(Player target)
        {
            NPC.direction = (target.Center.X > NPC.Center.X).ToDirectionInt();
            if (NPC.velocity.Y == 0f)
                NPC.velocity.X *= 0.9f;

            if (Timer <= HorribleHoller_RoarTime)
            {
                if (Timer == HorribleHoller_RoarTime - 30)
                {
                    SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                    CalamityUtils.AddScreenshakeAt(NPC.Center, 6f);
                    NPC.velocity.Y -= 6f;
                }

                // Spawn the Zombie Spawner projectiles on top of tiles.
                if (Timer == HorribleHoller_RoarTime)
                {
                    for (int i = 0; i < HorribleHoller_MaxZombies; i++)
                    {
                        int maxTileRange = 12;
                        Vector2 zombieSpawnPosition = NPC.Center + new Vector2(Main.rand.Next(-maxTileRange * 16, (maxTileRange + 1) * 16), 0f);
                        Vector2 actualSpawnPosition = FindSuitableGround(zombieSpawnPosition.ToTileCoordinates()).ToWorldCoordinates() + Vector2.UnitY * 16f;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int maxTime = 150 + Main.rand.Next(31);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), actualSpawnPosition, Vector2.Zero, ModContent.ProjectileType<HorribleHogZombieSpawner>(), 0, 0f, Main.myPlayer, 0f, maxTime);
                        }
                    }
                }

                if (Timer >= HorribleHoller_RoarTime - 30)
                    Animate(MinFrame_Roar, MaxFrame_Roar, HorribleHoller_RoarTime / (MaxFrame_Roar + 1), true, MaxFrame_Roar - 1);
            }

            if (Timer >= HorribleHoller_RoarTime)
                Animate(MinFrame_RoarFinish, MaxFrame_RoarFinish, 4, false);

            float targetAngle = (NPC.velocity.Y != 0f) ? NPC.velocity.X * 0.125f * (NPC.velocity.Y < 0).ToDirectionInt() : 0f;
            if (Timer >= HorribleHoller_RoarTime - 30 && Timer <= HorribleHoller_RoarTime)
                targetAngle = (NPC.velocity.Y != 0f) ? (NPC.direction < 0 ? MathHelper.ToRadians(50f) : MathHelper.ToRadians(-50f)) : 0f;

            NPC.rotation = NPC.rotation.AngleLerp(targetAngle, 0.125f);

            if (Timer >= HorribleHoller_RoarTime + HorribleHoller_PostRoarCooldownTime)
            {
                List<BehaviorState> possibleAttacks = [BehaviorState.ChasePlayer, BehaviorState.VomitBarrage];
                SwitchBehavior(BehaviorState.HorribleHoller, null, [.. possibleAttacks]);
            }
        }

        public void MainBehavior_VomitBarrage(Player target)
        {
            if (LocalAIState == 0f)
            {
                if (Timer == 1f)
                {
                    SoundEngine.PlaySound(VomitChargeUpSound, NPC.Center);
                    if (PhaseTwo && CalamityWorld.revenge)
                    {
                        AltAttackVariant = Main.rand.Next(2);
                        NPC.netUpdate = true;
                    }
                }

                // Move away from the player.
                if (Timer <= VomitBarrage_PreJumpTime)
                {
                    NPC.direction = (target.Center.X > NPC.Center.X).ToDirectionInt();
                    GroundedMovement(target.Center, VomitBarrage_MaxSpeed, ChasePlayer_MaxAcceleration, slowdownDistance: 200f);

                    float targetAngle = (NPC.velocity.Y != 0f) ? NPC.velocity.X * 0.135f * (NPC.velocity.Y < 0).ToDirectionInt() : 0f;
                    NPC.rotation = NPC.rotation.AngleLerp(targetAngle, 0.125f);

                    int preJumpVisualsTime = VomitBarrage_PreJumpTime - 30;
                    if (Timer >= preJumpVisualsTime)
                    {
                        NPC.velocity.X *= 0.96f;
                        float interpolant = Utils.GetLerpValue(preJumpVisualsTime, VomitBarrage_PreJumpTime, Timer, true);
                        HorizontalShakeStrength = MathHelper.Lerp(0f, 6f, interpolant);
                        SetSquashVectors(new Vector2(1.24f, 0.84f));
                    }

                    int frameSpeed = (int)Utils.Remap(MathF.Abs(NPC.velocity.X), 0f, VomitBarrage_MaxSpeed, 9, 3, true);
                    Animate(MinFrame_Walking, MaxFrame_Walking, frameSpeed, true, dynamicChanges: true);
                }

                // Jump and shoot a bunch of vomit projectiles shortly afterwards.
                if (Timer == VomitBarrage_PreJumpTime)
                {
                    NPC.velocity.Y = -JumpAndDash_MaxJumpHeight;
                    NPC.velocity.X = -12f * NPC.direction;

                    HorizontalShakeStrength = 0f;
                    DoJumpEffects(6, 9);
                    SetSquashVectors(squashVector: new Vector2(0.84f, 1.14f));
                }

                if (Timer >= VomitBarrage_PreJumpTime && Timer <= VomitBarrage_PreJumpTime + VomitBarrage_VomitTime)
                {
                    NPC.velocity.Y *= 0.98f;
                    NPC.velocity.X *= 0.96f;

                    float targetAngle = NPC.direction < 0 ? MathHelper.ToRadians(50f) : MathHelper.ToRadians(-50f);
                    NPC.rotation = NPC.rotation.AngleLerp(targetAngle, 0.075f);
                }

                if (Timer == JumpAndDash_JumpPreDashTime + JumpAndDash_PreJumpTime)
                {
                    bool spawnEyeballs = PhaseTwo && CalamityWorld.revenge && AltAttackVariant == 1f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < VomitBarrage_MaxVomitChunks; i++)
                        {
                            int projectileType = ModContent.ProjectileType<HorribleHogVomitChunk>();
                            int damage = Damage_VomitChunkProjectile;

                            // Randomly replace chunks with bombs in Expert and above.
                            bool vomitBombFloorNotReached = MiscAttackCounter < VomitBarrage_MinVomitBombs;
                            bool vomitBombCeilingNotReached = Main.rand.NextBool(4) && MiscAttackCounter >= VomitBarrage_MinVomitBombs && MiscAttackCounter < VomitBarrage_MaxVomitBombs;
                            if (Main.expertMode && (vomitBombFloorNotReached || vomitBombCeilingNotReached))
                            {
                                projectileType = ModContent.ProjectileType<HorribleHogVomitBomb>();
                                damage = Damage_VomitBombProjectile;
                                MiscAttackCounter++;
                            }

                            // Replace chunks with homing Demon Eyes in Rev+ during phase two when the alt attack is picked.
                            if (spawnEyeballs)
                            {
                                projectileType = ModContent.ProjectileType<HorribleHogVomitEye>();
                                damage = Damage_VomitEyeProjectile;
                            }

                            Vector2 vomitVelocity = CalamityUtils.GetProjectilePhysicsFiringVelocity(NPC.Center, target.Center + new Vector2(Main.rand.NextFloat(-16f, 16f), 0f), 0.125f, Main.rand.NextFloat(5f, 8f));
                            vomitVelocity.X *= Main.rand.NextFloat(0.25f, 2.25f);
                            int vomit = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vomitVelocity, projectileType, damage, 0f);
                            if (spawnEyeballs)
                                Main.projectile[vomit].ai[0] = Main.rand.Next(45, 75);
                            if (projectileType == ModContent.ProjectileType<HorribleHogVomitBomb>())
                                Main.projectile[vomit].ai[1] = NPC.whoAmI;
                        }
                    }

                    int vomitAmt = Main.rand.Next(12, 19);
                    for (int i = 0; i < vomitAmt; i++)
                    {
                        Vector2 velocity = CalamityUtils.GetProjectilePhysicsFiringVelocity(NPC.Center, target.Center + new Vector2(Main.rand.NextFloat(-16f, 16f), 0f), 0.125f, Main.rand.NextFloat(5f, 8f));
                        velocity.X *= Main.rand.NextFloat(1f, 2f);
                        int dustType = Utils.SelectRandom(Main.rand, DustID.ToxicBubble, DustID.GreenBlood, DustID.Blood);

                        Dust.NewDust(NPC.Center, 1, 1, dustType, velocity.X, velocity.Y, Scale: Main.rand.NextFloat(1.8f, 2.4f));                    
                    }

                    FrameY = VomitFrame;
                    SoundEngine.PlaySound(VomitSound with { Volume = 1.3f }, NPC.Center);
                    NPC.velocity.X = 6f * -NPC.direction;
                    NPC.velocity.Y += 2f;
                }

                if (Timer > JumpAndDash_JumpPreDashTime + JumpAndDash_PreJumpTime && NPC.velocity.Y == 0f)
                {
                    FrameY = IdleFrame;
                    LocalAIState = 1f;
                    Timer = 0f;
                    NPC.netUpdate = true;
                }
            }

            if (LocalAIState == 1f)
            {
                if (Timer >= VomitBarrage_PostVomitCooldown)
                {
                    List<BehaviorState> possibleAttacks = [BehaviorState.HorribleHoller, BehaviorState.JumpAndDash];
                    if (NPC.Center.Y - target.Center.Y < 128)
                        possibleAttacks.Add(BehaviorState.HogCharge);
                    SwitchBehavior(BehaviorState.VomitBarrage, null, [.. possibleAttacks]);
                }

                // Do a small hiccup/burp after the attack.
                int burpTime = VomitBarrage_PostVomitCooldown / 2;
                if (Timer == burpTime)
                {
                    SoundEngine.PlaySound(HiccupSound with { Volume = 1.25f }, NPC.Center);
                    SetSquashVectors(squashVector: new Vector2(0.84f, 1.14f));
                    NPC.velocity.Y = -4.25f;

                    int burpDustAmt = Main.rand.Next(6, 10);
                    for (int i = 0; i < burpDustAmt; i++)
                    {
                        Vector2 spawnPosition = NPC.Center + new Vector2(8f * NPC.direction, -8f);
                        Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.ToRadians(20f) + NPC.rotation) * Main.rand.NextFloat(3f, 5f) * NPC.direction;
                        Dust.NewDust(spawnPosition, 1, 1, DustID.FartInAJar, velocity.X, velocity.Y, Scale: Main.rand.NextFloat(0.8f, 1.2f));
                    }
                }

                if (NPC.velocity.Y == 0f)
                    NPC.velocity.X *= 0.9f;

                float targetAngle = (NPC.velocity.Y != 0f) ? NPC.velocity.X * 0.135f * (NPC.velocity.Y < 0).ToDirectionInt() : 0f;
                if (Timer >= burpTime && Timer <= VomitBarrage_PostVomitCooldown)
                    targetAngle = (NPC.velocity.Y != 0f) ? (NPC.direction < 0 ? MathHelper.ToRadians(30f) : MathHelper.ToRadians(-30f)) : 0f;
                NPC.rotation = NPC.rotation.AngleLerp(targetAngle, 0.125f);
                NPC.direction = (target.Center.X > NPC.Center.X).ToDirectionInt();

                int frameSpeed = (int)Utils.Remap(MathF.Abs(NPC.velocity.X), 0f, VomitBarrage_MaxSpeed, 9, 3, true);
                Animate(MinFrame_Walking, MaxFrame_Walking, frameSpeed, true, dynamicChanges: true);
            }
        }

        private void SpawnShockwave(int spawnImterval, int maxShockwaves, int direction, float minHeightMultiplier, float maxHeightMultiplier)
        {
            int spawnerIndex = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom, Vector2.Zero, ModContent.ProjectileType<HorribleHogShockwaveSpawner>(), Damage_ShockwaveProjectile, 0f);
            Main.projectile[spawnerIndex].ModProjectile<HorribleHogShockwaveSpawner>().OwnerIndex = NPC.whoAmI;
            Main.projectile[spawnerIndex].ai[0] = spawnImterval;
            Main.projectile[spawnerIndex].ai[1] = maxShockwaves;
            Main.projectile[spawnerIndex].ai[2] = direction;
            Main.projectile[spawnerIndex].localAI[0] = minHeightMultiplier;
            Main.projectile[spawnerIndex].localAI[1] = maxHeightMultiplier;
            Main.projectile[spawnerIndex].netUpdate = true;
        }

        private void KillChargeHitboxProjectile()
        {
            if (CalamityUtils.AnyProjectiles(ModContent.ProjectileType<HorribleHogChargeHitbox>()))
            {
                int hitboxIndex = CalamityUtils.FindFirstProjectile(ModContent.ProjectileType<HorribleHogChargeHitbox>());
                if (Main.projectile.IndexInRange(hitboxIndex) && Main.projectile[hitboxIndex].ai[0] == NPC.whoAmI)
                {
                    Main.projectile[hitboxIndex].Kill();
                    Main.projectile[hitboxIndex].netUpdate = true;
                }
            }
        }
    }
}
