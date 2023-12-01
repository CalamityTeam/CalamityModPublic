using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Projectiles.BaseProjectiles
{
    public abstract class BaseShortswordProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public enum ShortswordType
        {
            TypicalShortsword
        }

        #region Virtual Values
        // Typical shortsword virtual values
        public virtual float FadeInDuration => 7f;
        public virtual float FadeOutDuration => 4f;
        public virtual float TotalDuration => 16f;
        public virtual Action<Projectile> EffectBeforePullback => null;
        public virtual ShortswordType ShortswordAIType => ShortswordType.TypicalShortsword;
        #endregion

    public float CollisionWidth => 4f * Projectile.scale;
    public float FullUse => Timer / 14f;
    public Player Owner => Main.player[Projectile.owner];


        public int Timer
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public virtual void Behavior()
        {
            Player player = Main.player[Projectile.owner];

            if (ShortswordAIType == ShortswordType.TypicalShortsword && !player.frozen)
            {
                player.ChangeDir(Projectile.direction);
                player.heldProj = Projectile.whoAmI;
                player.itemTime = player.itemAnimation;


                Timer += 1;
                if (Timer >= TotalDuration)
                {
                    Projectile.Kill();
                    return;
                }
                else
                {
                    player.heldProj = Projectile.whoAmI;
                }

                // Fade in and out
                Projectile.Opacity = Utils.GetLerpValue(0f, FadeInDuration, Timer, clamped: true) * Utils.GetLerpValue(TotalDuration, TotalDuration - FadeOutDuration, Timer, clamped: true);

                // If we haven't done the special effect yet (assuming there is one), do it.
                // Note : Null Coalescing does not work in this case because we are invoking a method, not setting a value.
                if (Projectile.localAI[0] == 0f && EffectBeforePullback != null && Main.myPlayer == Projectile.owner)
                {
                    Projectile.localAI[0] = 1f;
                    EffectBeforePullback.Invoke(Projectile);
                }

                // Keep locked onto the player, but extend further based on the given velocity (Requires ShouldUpdatePosition returning false to work)
                Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, reverseRotation: false, addGfxOffY: false);
                Projectile.Center = playerCenter + Projectile.velocity * (Timer - 1f);

                // Rotate the sprite
                Projectile.Center = Owner.MountedCenter + OffsetFromPlayer;
                Projectile.scale = 1f + (float)Math.Sin(FullUse * MathHelper.Pi) * 0.2f;

                // Set spriteDirection based on moving left or right. Left -1, right 1
                Projectile.spriteDirection = (Vector2.Dot(Projectile.velocity, Vector2.UnitX) >= 0f).ToDirectionInt();

                // Point towards where it is moving, applied offset for top right of the sprite respecting spriteDirection
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 - MathHelper.PiOver4 * Projectile.spriteDirection;

                // The code in this method is important to align the sprite with the hitbox how we want it to
                SetVisualOffsets();
            }
        }

        public virtual void SetVisualOffsets() // Intentionally blank
        {

        }

        public virtual void ExtraBehavior() // Intentionally blank
        {

        }

        public override void AI()
        {
            Behavior();
            ExtraBehavior();
        }

        public override bool ShouldUpdatePosition() => false;
        public CurveSegment ThrustSegment = new CurveSegment(LinearEasing, 0f, 0f, 1f, 3);
        public CurveSegment HoldSegment = new CurveSegment(SineBumpEasing, 0.2f, 1f, 0.2f);
        public CurveSegment RetractSegment = new CurveSegment(PolyOutEasing, 0.76f, 1f, -0.8f, 3);
        public CurveSegment BumpSegment = new CurveSegment(SineBumpEasing, 0.9f, 0.2f, 0.15f);
        internal float DistanceFromPlayer => PiecewiseAnimation(FullUse, new CurveSegment[] { ThrustSegment, HoldSegment, RetractSegment, BumpSegment });
        public Vector2 OffsetFromPlayer => Projectile.velocity * DistanceFromPlayer * 12f;

        public override void CutTiles()
        {
            // "cutting tiles" refers to breaking pots, grass, queen bee larva, etc.
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.velocity.SafeNormalize(-Vector2.UnitY) * 10f;
            Utils.PlotTileLine(start, end, CollisionWidth, DelegateMethods.CutTiles);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeLength = 12f * Projectile.scale;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.MountedCenter + OffsetFromPlayer, Owner.MountedCenter + OffsetFromPlayer + (Projectile.velocity * bladeLength), 24, ref collisionPoint);
        }
    }
}
