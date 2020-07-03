using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.BaseProjectiles
{
    public abstract class BaseLaserbeamProjectile : ModProjectile
    {
        public float RotationalSpeed
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }
        public float Time
        {
            get => projectile.localAI[0];
            set => projectile.localAI[0] = value;
        }
        public float LaserLength
        {
            get => projectile.localAI[1];
            set => projectile.localAI[1] = value;
        }
        public virtual void Behavior()
        {
            // Attach to some arbitrary thing/position optionally. (The ai[1] value is a reserved for this in vanilla's Phantasmal Deathray)
            AttachToSomething();

            // Ensure the the velocity is a unit vector and is not a <0,0> vector.
            projectile.velocity = projectile.velocity.SafeNormalize(-Vector2.UnitY);

            Time++;
            if (Time >= Lifetime)
            {
                projectile.Kill();
                return;
            }

            DetermineScale();

            UpdateLaserMotion();

            float idealLaserLength = DetermineLaserLength();
            LaserLength = MathHelper.Lerp(LaserLength, idealLaserLength, 0.9f); // Very quickly approach the ideal laser length.

            DelegateMethods.v3_1 = LightCastColor.ToVector3();
            Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * LaserLength, projectile.width * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));
        }
        public virtual void UpdateLaserMotion()
        {
            // This part is rather complicated at a glance.
            // What it's doing is converting the velocity to an angle, doing something with that angle, and changing it back into the velocity.
            // In this case, "doing something with that angle" means incrementing it by a constant.
            // You could attempt to make it intelligent by having it move towards the target like the Last Prism, but that's not done here.
            // This allows one to cause an arcing motion.

            float updatedVelocityDirection = projectile.velocity.ToRotation() + RotationalSpeed;
            projectile.rotation = updatedVelocityDirection - MathHelper.PiOver2; // Pretty much all lasers have a vertical sheet.
            projectile.velocity = updatedVelocityDirection.ToRotationVector2();
        }
        public virtual void DetermineScale()
        {
            projectile.scale = (float)Math.Sin(Time / Lifetime * MathHelper.Pi) * 6f * MaxScale;
            if (projectile.scale > MaxScale)
            {
                projectile.scale = MaxScale;
            }
        }
        public virtual void AttachToSomething() { } // Does nothing by default.
        public virtual float DetermineLaserLength() => MaxLaserLength; // Go with the default length and ignore any obstacles by default.
        public virtual void ExtraBehavior() { }
        public override void AI()
        {
            Behavior();
            ExtraBehavior();
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            // This should never happen, but just in case-
            if (projectile.velocity == Vector2.Zero)
                return false;

            // Start texture drawing.
            spriteBatch.Draw(LaserBeginTexture, 
                             projectile.Center - Main.screenPosition, 
                             null,
                             LaserOverlayColor,
                             projectile.rotation,
                             LaserBeginTexture.Size() / 2f,
                             projectile.scale, 
                             SpriteEffects.None,
                             0f);

            // Prepare things for body drawing.
            float laserBodyLength = LaserLength;
            laserBodyLength -= (LaserBeginTexture.Height / 2 + LaserEndTexture.Height) * projectile.scale;
            Vector2 centerOnLaser = projectile.Center;
            centerOnLaser += projectile.velocity * projectile.scale * LaserBeginTexture.Height / 2f;

            // Body drawing.
            if (laserBodyLength > 0f)
            {
                float laserOffset = LaserMiddleTexture.Height * projectile.scale;
                float incrementalBodyLength = 0f;
                while (incrementalBodyLength + 1f < laserBodyLength)
                {
                    spriteBatch.Draw(LaserMiddleTexture, 
                                     centerOnLaser - Main.screenPosition, 
                                     null, 
                                     LaserOverlayColor, 
                                     projectile.rotation,
                                     LaserMiddleTexture.Width * 0.5f * Vector2.UnitX, 
                                     projectile.scale, 
                                     SpriteEffects.None,
                                     0f);
                    incrementalBodyLength += laserOffset;
                    centerOnLaser += projectile.velocity * laserOffset;
                }
            }

            // End texture drawing.
            if (Math.Abs(LaserLength - DetermineLaserLength()) < 30f)
            {
                Vector2 laserEndCenter = centerOnLaser - Main.screenPosition;
                spriteBatch.Draw(LaserEndTexture,
                                 laserEndCenter,
                                 null,
                                 LaserOverlayColor,
                                 projectile.rotation,
                                 LaserEndTexture.Frame(1, 1, 0, 0).Top(),
                                 projectile.scale,
                                 SpriteEffects.None,
                                 0f);
            }
            return false;
        }
        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackMelee;
            Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * LaserLength, projectile.Size.Length() * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CutTiles));
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
                return true;
            float unused = 69420f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity * LaserLength, projectile.Size.Length() * projectile.scale, ref unused))
            {
                return true;
            }
            return false;
        }

        #region Virtual Values
        public virtual float Lifetime => 120f;
        public virtual float MaxScale => 1f;
        public virtual float MaxLaserLength => 2400f; // Be careful with this. Going too high will cause lag.
        public virtual Color LightCastColor => Color.White;
        public virtual Color LaserOverlayColor => Color.White * 0.9f;
        public virtual Texture2D LaserBeginTexture { get; }
        public virtual Texture2D LaserMiddleTexture { get; }
        public virtual Texture2D LaserEndTexture { get; }
        #endregion
    }
}
