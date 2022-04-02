using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
namespace CalamityMod.Projectiles.Summon
{
    public class DaedalusLightning : ModProjectile
    {
        internal PrimitiveTrail LightningDrawer;

        public const int MaximumBranchingIterations = 3;
        public const float LightningTurnRandomnessFactor = 1.7f;
        public ref float InitialVelocityAngle => ref projectile.ai[0];
        // Technically not a ratio, and more of a seed, but it is used in a 0-2pi squash
        // later in the code to get an arbitrary unit vector (which is then checked).
        public ref float BaseTurnAngleRatio => ref projectile.ai[1];
        public ref float AccumulatedXMovementSpeeds => ref projectile.localAI[0];
        public ref float BranchingIteration => ref projectile.localAI[1];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Daedalus Lightning");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 100;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.alpha = 255;
            projectile.penetrate = 2;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.extraUpdates = 20;
            projectile.timeLeft = 45 * projectile.extraUpdates;

            // Readjust the velocity magnitude the moment this projectile is created
            // to make velocity setting outside the scope of this projectile less irritating
            // to consider alongside extraUpdate multipliers.
            // Also set the initial angle.
            if (projectile.velocity != Vector2.Zero)
            {
                projectile.velocity /= projectile.extraUpdates;
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(AccumulatedXMovementSpeeds);
            writer.Write(BranchingIteration);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            AccumulatedXMovementSpeeds = reader.ReadSingle();
            BranchingIteration = reader.ReadSingle();
        }

        public override void AI()
        {
            // frameCounter in this context is really just an arbitrary timer
            // which allows random turning to occur.
            projectile.frameCounter++;

            projectile.scale = (float)Math.Sin(MathHelper.Pi * projectile.timeLeft / (45f * (projectile.MaxUpdates - 1))) * 4f;
            if (projectile.scale > 1f)
                projectile.scale = 1f;

            Lighting.AddLight(projectile.Center, Color.Pink.ToVector3());
            if (projectile.frameCounter >= projectile.extraUpdates * 2)
            {
                projectile.frameCounter = 0;

                float originalSpeed = MathHelper.Min(6f, projectile.velocity.Length());
                UnifiedRandom unifiedRandom = new UnifiedRandom((int)BaseTurnAngleRatio);
                int turnTries = 0;
                Vector2 newBaseDirection = -Vector2.UnitY;
                Vector2 potentialBaseDirection;

                do
                {
                    BaseTurnAngleRatio = unifiedRandom.Next() % 100;
                    potentialBaseDirection = (BaseTurnAngleRatio / 100f * MathHelper.TwoPi).ToRotationVector2();

                    // Ensure that the new potential direction base is always moving upwards (this is supposed to be somewhat similar to a -UnitY + RotatedBy).
                    potentialBaseDirection.Y = -Math.Abs(potentialBaseDirection.Y);

                    bool canChangeLightningDirection = true;

                    // Potential directions with very little Y speed should not be considered, because this
                    // consequentially means that the X speed would be quite large.
                    if (potentialBaseDirection.Y > -0.02f)
                        canChangeLightningDirection = false;

                    // This mess of math basically encourages movement at the ends of an extraUpdate cycle,
                    // discourages super frequenent randomness as the accumulated X speed changes get larger,
                    // or if the original speed is quite large.
                    if (Math.Abs(potentialBaseDirection.X * (projectile.extraUpdates + 1) * 2f * originalSpeed + AccumulatedXMovementSpeeds) > projectile.MaxUpdates * LightningTurnRandomnessFactor)
                    {
                        canChangeLightningDirection = false;
                    }

                    // If the above checks were all passed, redefine the base direction of the lightning.
                    if (canChangeLightningDirection)
                        newBaseDirection = potentialBaseDirection;

                    turnTries++;
                }
                while (turnTries < 100);

                if (projectile.velocity != Vector2.Zero)
                {
                    AccumulatedXMovementSpeeds += newBaseDirection.X * (projectile.extraUpdates + 1) * 2f * originalSpeed;
                    projectile.velocity = newBaseDirection.RotatedBy(InitialVelocityAngle + MathHelper.PiOver2) * originalSpeed;
                    projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
                }
            }
        }

        #region Drawing
        internal float WidthFunction(float completionRatio)
        {
            float baseWidth = MathHelper.Lerp(2f, 6f, (float)Math.Sin(MathHelper.Pi * 4f * completionRatio) * 0.5f + 0.5f) * projectile.scale;
            return baseWidth * (float)Math.Sin(MathHelper.Pi * completionRatio);
        }
        internal Color ColorFunction(float completionRatio)
        {
            Color baseColor = Color.Lerp(Color.Pink, Color.HotPink, (float)Math.Sin(MathHelper.TwoPi * completionRatio + Main.GlobalTime * 4f) * 0.5f + 0.5f);
            return Color.Lerp(baseColor, Color.Red, ((float)Math.Sin(MathHelper.Pi * completionRatio + Main.GlobalTime * 4f) * 0.5f + 0.5f) * 0.8f);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (LightningDrawer is null)
                LightningDrawer = new PrimitiveTrail(WidthFunction, ColorFunction, PrimitiveTrail.RigidPointRetreivalFunction);

            LightningDrawer.Draw(projectile.oldPos.Where(oldPos => oldPos != Vector2.Zero), projectile.Size * 0.5f - Main.screenPosition, 300);
            return false;
        }
        #endregion
    }
}
