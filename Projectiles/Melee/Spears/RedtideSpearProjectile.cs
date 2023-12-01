using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Projectiles.Melee.Spears
{
    public class RedtideSpearProjectile : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<RedtideSpear>();
        public Player Owner => Main.player[Projectile.owner];
        public static int Lifetime = 28;
        public int Timer => Lifetime - Projectile.timeLeft;

        private SlotId ChargeWindSoundSlot;


        public int ChargeDirection => Math.Sign(Projectile.velocity.X);

        #region Thrust
        //Basic animation variables.
        public float ThrustProgress => Timer / (float)Lifetime;
        public float BaseRotation => Projectile.velocity.ToRotation();
        public float ThrustRotation => BaseRotation + Owner.direction * RotationShift;

        //Thrust animation keys
        public CurveSegment thrust = new CurveSegment(EasingType.PolyIn, 0f, 0.15f, 0.85f, 3);
        public CurveSegment hold = new CurveSegment(EasingType.SineBump, 0.3f, 1f, 0.2f);
        public CurveSegment retract = new CurveSegment(EasingType.PolyOut, 0.7f, 1f, -1f, 2);
        public CurveSegment correct = new CurveSegment(EasingType.PolyIn, 0.85f, 0f, 0.17f);
        internal float Displacement => PiecewiseAnimation(ThrustProgress, new CurveSegment[] { thrust, hold, retract, correct });

        internal const float startRotation = MathHelper.PiOver4 * 0.1f;
        internal const float downwardsRotation = MathHelper.PiOver4 * 0.15f;
        internal const float upwardsRotation = MathHelper.PiOver4 * -0.44f;
        internal const float loweredTime = 0.25f;

        public CurveSegment downwards = new CurveSegment(EasingType.SineOut, 0f, startRotation, downwardsRotation, 2);
        public CurveSegment raiseback = new CurveSegment(EasingType.SineIn, loweredTime, startRotation + downwardsRotation, upwardsRotation);
        public CurveSegment stayUp = new CurveSegment(EasingType.Linear, 0.8f, startRotation + downwardsRotation + upwardsRotation, 0f);
        internal float RotationShift => PiecewiseAnimation(ThrustProgress, new CurveSegment[] { downwards, raiseback, stayUp });
        public int InitializationTime => (int)(loweredTime * Lifetime);
        #endregion

        #region Drag along the ground


        public static int MaxRunTime = 300;
        //Player can only do the run attack while moving fast horizontally in the direction of the thrust, but remaining on the ground
        public bool CanRunAttack => Owner.velocity.Y >= -0.8f && Owner.velocity.Y <= 7f && Math.Abs(Owner.velocity.X) >= Owner.maxRunSpeed * 0.7f && Math.Sign(Owner.velocity.X) == ChargeDirection && Owner.channel && !RunBroken;
        //You can only start a run attack if you meet the other conditions, but ALSO you can't do it if you are aiming too much upwards or downwards
        public bool CanStartRunAttack => CanRunAttack && Math.Abs(Owner.Calamity().mouseWorld.Y - Owner.MountedCenter.Y) <= 300;
        public ref float RunTimer => ref Projectile.ai[0];
        //The window of time during which the run hasn't fully started, and is indentical to the regular thrust's start. This tells us when we can no longer do this switch.
        public bool FullyRunning => RunTimer > InitializationTime;

        //Basically tracks if the run has been broken, either because the player did not try to run at all, stopped running, or ran for too long.
        public bool RunBroken
        {
            get => Projectile.ai[1] > 0f;
            set { Projectile.ai[1] = value ? 1f : 0f; }
        }

        public static int UpThrustLifetime = 20;
        public int UpThrustTimer => UpThrustLifetime - Projectile.timeLeft;
        public float UpThrustProgress => UpThrustTimer / (float)UpThrustLifetime;

        //Gets the ideal rotation for the spear while running. Basically, slightly angled downwards towards the ground, to the left or right of the player depending on the direction
        public float IdealSpearRotationForRun
        {
            get
            {
                float rotation = MathHelper.PiOver4 * 0.12f;

                if (ChargeDirection > 0)
                    return rotation;

                //Mirror the angle if facing left.
                return MathHelper.Pi - rotation - MathHelper.TwoPi;
            } 
        }

        //Properly wrap the angle. Indeed, the angles on the left suddenly go from Pi to -Pi
        public float ProjectileRotationButWrappedForTransition
        {
            get 
            {
                if (ChargeDirection < 0 && Projectile.rotation > 0)
                    return Projectile.rotation - MathHelper.TwoPi;

                return Projectile.rotation;
            }
        }

        public CurveSegment transitionSegment => new CurveSegment(EasingType.PolyInOut, 0f, ProjectileRotationButWrappedForTransition, IdealSpearRotationForRun - ProjectileRotationButWrappedForTransition, 2);
        internal float RunAttackRotation => PiecewiseAnimation(Math.Clamp(RunTimer / 30f, 0f, 1f), new CurveSegment[] { transitionSegment });


        public CurveSegment anticipationDown => new CurveSegment(EasingType.SineIn, 0f, IdealSpearRotationForRun, ChargeDirection * MathHelper.PiOver4 * 0.1f, 2);
        public CurveSegment upthrust => new CurveSegment(EasingType.SineOut, 0.4f, IdealSpearRotationForRun + ChargeDirection * MathHelper.PiOver4 * 0.1f, ChargeDirection * MathHelper.PiOver2 * -0.75f);
        public CurveSegment holdup => new CurveSegment(EasingType.Linear, 0.8f, IdealSpearRotationForRun + ChargeDirection * MathHelper.PiOver2 * -0.7f, -0.14f);
        internal float UpThrustRotation => PiecewiseAnimation(UpThrustProgress, new CurveSegment[] { anticipationDown, upthrust, holdup });


        public CurveSegment goback = new CurveSegment(EasingType.SineOut, 0f, 0.68f, -0.2f);
        public CurveSegment goforwardfast = new CurveSegment(EasingType.PolyIn, 0.33f, 0.58f, 0.7f, 3);
        public CurveSegment gobounce = new CurveSegment(EasingType.Linear, 0.6f, 1.28f, 0f);
        public CurveSegment gobackagain = new CurveSegment(EasingType.SineOut, 0.86f, 1.28f, -0.2f);
        internal float UpThrustDisplacement => PiecewiseAnimation(UpThrustProgress, new CurveSegment[] { goback, goforwardfast, gobounce, gobackagain });

        #endregion

        #region General calculations
        public enum AttackState { ForwardThrust, RunAttack, UpwardsThrust };
        public AttackState CurrentAttackState => (RunBroken && !FullyRunning) ? AttackState.ForwardThrust : (RunBroken || RunTimer > MaxRunTime) ? AttackState.UpwardsThrust : AttackState.RunAttack;

        //Gets the proper rotation based on the current state of the spear
        public float AppropriateRotation
        {
            get
            {
                //If the player isn't fully running yet, we just don't care about the actual attack, and have them have the default thrust rotation
                if (!FullyRunning)
                    return ThrustRotation;

                if (CurrentAttackState == AttackState.RunAttack)
                    return RunAttackRotation;

                if (CurrentAttackState == AttackState.UpwardsThrust)
                    return UpThrustRotation;

                return ThrustRotation;
            }
        }

        //Gets the proper displacement based on the state of the spear
        public float AppropriateDisplacement
        {
            get
            {
                //If the player isn't fully running yet, we just don't care about the actual attack, and have them have the default thrust rotation
                if (!FullyRunning)
                    return Displacement;

                //If the player is running, make them keep the lance a bit behind as they run
                if (CurrentAttackState == AttackState.RunAttack)
                    return MathHelper.Lerp(0.93f, 0.68f, Math.Clamp(RunTimer / 30f, 0f, 1f));

                if (CurrentAttackState == AttackState.UpwardsThrust)
                    return UpThrustDisplacement;

                return Displacement;
            }
        }
        #endregion

        public override void SetDefaults()
        {
            Projectile.width = 56;
            Projectile.DamageType = DamageClass.Melee;  //Dictates whether this is a melee-class weapon.
            Projectile.timeLeft = Lifetime;
            Projectile.height = 56;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeLength = 85 * Projectile.scale;
            float bladeWidth = 20 * Projectile.scale;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center + AppropriateRotation.ToRotationVector2() * bladeLength * -0.5f, Projectile.Center + ThrustRotation.ToRotationVector2() * bladeLength * 0.5f, bladeWidth, ref collisionPoint);
        }

        public override void AI()
        {
            UpThrustLifetime = 20;


            //If the player starts the attack without being able to do a run attack, mark the run as "broken" from the start.
            if (Projectile.timeLeft == Lifetime && RunTimer == 0 && !CanStartRunAttack)
            {
                RunBroken = true;
            }

            if (CurrentAttackState == AttackState.RunAttack)
            {
                Owner.GetModPlayer<SpearChargePlayer>().ChargingKnockbackResist = true;

                //Keep the projectile alive during the run attack. 
                //The fullyRunning check is important, because the animation timer relies on the projectiles time left for the startup animation.
                if (FullyRunning)
                    Projectile.timeLeft++;

                else
                    //We never actually use Projectile.rotation to draw or do any collision calculations.
                    //Instead, we use it to keep track of the final rotation of the spear BEFORE it was fully running
                    //We proceed to use this value to smooth the transition between the start of the thrust animation with the run animation.
                    Projectile.rotation = AppropriateRotation;

                //If at any point of the run attack, something happens to interrupt it, break the run and transition back into a forward or upwards thrust, depending on how long the player ran.
                if (!CanRunAttack)
                {
                    RunBroken = true;
                    //If the player was running for long enough, allow them to do a thrust attack. For this, we change the animation timer.
                    if (FullyRunning)
                    {
                        Projectile.timeLeft = UpThrustLifetime;
                    }
                }

                ActiveSound soundOut;
                if (!SoundEngine.TryGetActiveSound(ChargeWindSoundSlot, out soundOut) || !soundOut.IsPlaying)
                    ChargeWindSoundSlot = SoundEngine.PlaySound(SoundID.DD2_BookStaffTwisterLoop with { IsLooped = true, Volume = SoundID.DD2_BookStaffTwisterLoop.Volume * 0.7f }, Projectile.Center);

                else
                {
                    soundOut.Volume = MathHelper.Lerp(0f, SoundID.DD2_BookStaffTwisterLoop.Volume * 0.7f, Math.Clamp(RunTimer / 120f, 0f, 1f));
                    soundOut.Position = Projectile.Center;
                }

                RunTimer++;

                if (Main.rand.NextBool())
                {
                    int dustOpacity = Main.rand.Next(80);
                    float dustScale = Main.rand.NextFloat(1f, 1.4f);
                    Vector2 dustVelocity = (AppropriateRotation + MathHelper.Pi).ToRotationVector2().RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(0.5f, 5f);
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + AppropriateRotation.ToRotationVector2() * 40f, 16, dustVelocity, Alpha: dustOpacity, Scale: dustScale);
                }
            }

            //Basic spear attack
            else if (CurrentAttackState == AttackState.ForwardThrust)
            {
                if (ThrustProgress == 0.5f)
                    SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack, Projectile.Center);
            }

            else if (CurrentAttackState == AttackState.UpwardsThrust)
            {
                ActiveSound soundOut;


                if (SoundEngine.TryGetActiveSound(ChargeWindSoundSlot, out soundOut))
                {
                    soundOut.Volume *= 0.7f;
                    soundOut.Position = Projectile.Center;
                    if (soundOut.Volume < 0.2f)
                        soundOut.Stop();
                }

                if (ThrustProgress == 0.5f)
                    SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing, Projectile.Center);
            }

            UpdateOwnerVars();
            Vector2 thrust = AppropriateRotation.ToRotationVector2() * (AppropriateDisplacement * 40f + 5f);

            //During the run anim, make the spear bob up and down.
            if (CurrentAttackState == AttackState.RunAttack)
                thrust -= Vector2.UnitY * 2f * MathHelper.Clamp(0.5f + 0.8f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 20f), 0f, 1f);

            Projectile.Center = Owner.MountedCenter + thrust;
        }

        public void UpdateOwnerVars()
        {
            Owner.ChangeDir(Math.Sign(BaseRotation.ToRotationVector2().X));
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, AppropriateRotation - MathHelper.PiOver2);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;

            //Add more latency after an upwards thrust, for impact purposes
            if (CurrentAttackState == AttackState.UpwardsThrust)
            {
                Owner.itemTime = 20;
                Owner.itemAnimation = 20;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //Send the enemy flying up if hit by the upwards thrust.
            if (CurrentAttackState == AttackState.UpwardsThrust)
            {
                target.velocity.Y -= 12 * (float)Math.Sqrt(target.knockBackResist);
                target.FallingNPC().ApplyFallDamage(target, 50, 5f);
            }

            target.AddBuff(BuffID.Poisoned, 180);

            //Give a sliver of iframes to the player so its safer to ram into hordes (which is fun and should be encouraged)
            if (CurrentAttackState == AttackState.RunAttack)
                Owner.GiveIFrames(5);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            //Boost the knockback during the run attack
            if (CurrentAttackState == AttackState.RunAttack)
                modifiers.Knockback *= 1.35f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor, AppropriateRotation + MathHelper.PiOver2 * 1.5f - MathHelper.ToRadians(12), texture.Size() / 2f, Projectile.scale, 0, 0);
            return false;
        }
    }
}
