using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System;
using CalamityMod.Particles;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Projectiles.Melee.Spears
{
    public class RedtideSpearProjectile : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];
        public static int Lifetime => 28;
        public int Timer => Lifetime - Projectile.timeLeft;

        #region Thrust
        //Basic animation variables.
        public float ThrustProgress => Timer / (float)Lifetime;
        public float ThrustRotation => Projectile.velocity.ToRotation() + Owner.direction * RotationShift;
        public float BaseRotation => Projectile.velocity.ToRotation();

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
        public CurveSegment upstamp = new CurveSegment(EasingType.SineIn, loweredTime, startRotation + downwardsRotation, upwardsRotation);
        public CurveSegment stayUp = new CurveSegment(EasingType.Linear, 0.8f, startRotation + downwardsRotation + upwardsRotation, 0f);
        internal float RotationShift => PiecewiseAnimation(ThrustProgress, new CurveSegment[] { downwards, upstamp, stayUp });
        public int InitializationTime => (int)(loweredTime * Lifetime);
        #endregion

        #region Drag along the ground

        //Player can only do the run attack while moving fast horizontally, but remaining on the ground
        public static int MaxRunTime = 300;
        public bool CanRunAttack => Owner.velocity.Y == 0 && Math.Abs(Owner.velocity.X) >= Owner.maxRunSpeed * 0.7f && Owner.channel && !RunBroken;
        public ref float RunTimer => ref Projectile.ai[0];
        public bool RunBroken
        {
            get => Projectile.ai[1] > 0f; //Basically tracks if the run has been broken.
            set { Projectile.ai[1] = value ? 1f : 0f; }
        }
        #endregion

        internal static enum AttackState { ForwardThrust, RunAttack, UpwardsThrust };
        public AttackState CurrentMode => (RunBroken && RunTimer <= InitializationTime) ? AttackState.ForwardThrust : (RunBroken || RunTimer > MaxRunTime) ? AttackState.UpwardsThrust : AttackState.RunAttack;


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Redtide Spear");
        }

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
            float bladeLenght = 85 * Projectile.scale;
            float bladeWidth = 20 * Projectile.scale;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center + ThrustRotation.ToRotationVector2() * bladeLenght * -0.5f, Projectile.Center + ThrustRotation.ToRotationVector2() * bladeLenght * 0.5f, bladeWidth, ref collisionPoint);
        }

        public override void AI()
        {
            UpdateOwnerVars();
            Vector2 thrust = ThrustRotation.ToRotationVector2() * (Displacement * 40f + 5f);
            Projectile.Center = Owner.Center + thrust;
            
        }

        public void UpdateOwnerVars()
        {
            Owner.direction = Math.Sign(BaseRotation.ToRotationVector2().X);
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, ThrustRotation - MathHelper.PiOver2);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 180);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor, ThrustRotation + MathHelper.PiOver2 * 1.5f - MathHelper.ToRadians(12), texture.Size() / 2f, Projectile.scale, 0, 0);
            return false;
        }
    }
}
