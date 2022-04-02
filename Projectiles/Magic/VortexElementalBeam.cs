using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace CalamityMod.Projectiles.Magic
{
    public class VortexElementalBeam : ModProjectile
    {
        public const int Lifetime = 30;
        public const float LightningTurnRandomnessFactor = 1.7f;
        public ref float InitialVelocityAngle => ref projectile.ai[0];
        // Technically not a ratio, and more of a seed, but it is used in a 0-2pi squash
        // later in the code to get an arbitrary unit vector (which is then checked).
        public ref float BaseTurnAngleRatio => ref projectile.ai[1];
        public ref float AccumulatedXMovementSpeeds => ref projectile.localAI[0];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 30;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 16;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
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

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(AccumulatedXMovementSpeeds);

        public override void ReceiveExtraAI(BinaryReader reader) => AccumulatedXMovementSpeeds = reader.ReadSingle();

        public override void AI()
        {
            // frameCounter in this context is really just an arbitrary timer
            // which allows random turning to occur.
            projectile.frameCounter++;

            projectile.scale += 0.04f / projectile.MaxUpdates;
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
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D lightningSegmentTexture = Main.projectileTexture[projectile.type];
            projectile.GetAlpha(lightColor);
            Vector2 lightningScale = new Vector2(projectile.scale) / 2f;
            for (int i = 0; i < 3; i++)
            {
                // c_1 and f_1 are general fields that are used for drawing methods in here.
                // c_1 is the color to be used, and f_1 is the opacity.
                if (i == 0)
                {
                    lightningScale = new Vector2(projectile.scale) * 0.6f;
                    DelegateMethods.c_1 = Color.DarkCyan * 0.85f;
                }
                else if (i == 1)
                {
                    lightningScale = new Vector2(projectile.scale) * 0.4f;
                    DelegateMethods.c_1 = Color.Cyan * 0.85f;
                }
                else
                {
                    lightningScale = new Vector2(projectile.scale) * 0.2f;
                    DelegateMethods.c_1 = Color.White;
                }

                DelegateMethods.f_1 = 1f;
                for (int j = projectile.oldPos.Length - 1; j > 0; j--)
                {
                    if (projectile.oldPos[j] != Vector2.Zero)
                    {
                        Vector2 start = projectile.oldPos[j] + projectile.Size * 0.5f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
                        Vector2 end = projectile.oldPos[j - 1] + projectile.Size * 0.5f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
                        Utils.DrawLaser(spriteBatch, lightningSegmentTexture, start, end, lightningScale, DelegateMethods.LightningLaserDraw);
                    }
                }
            }
            return false;
        }
        #endregion

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Electrified, 180);

            for (int i = 0; i < projectile.oldPos.Length - 1; i++)
            {
                // Skip zeroed old positions. They are almost certainly a
                // result of incomplete indices.
                if (projectile.oldPos[i + 1] == Vector2.Zero)
                    continue;

                for (int j = 0; j < 8; j++)
                {
                    Vector2 dustSpawnPosition = Vector2.Lerp(projectile.oldPos[i], projectile.oldPos[i + 1], j / 8f);
                    dustSpawnPosition += projectile.Size * 0.5f;

                    Dust electricity = Dust.NewDustPerfect(dustSpawnPosition, Main.rand.NextBool(2) ? 226 : 229);
                    electricity.velocity = Vector2.Zero;
                    electricity.scale = Main.rand.NextFloat(1.1f, 1.18f);
                    electricity.noGravity = true;
                }
            }
        }
    }
}
