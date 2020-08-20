using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Enums;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class MidnightSunBeam : ModProjectile
    {
        // How long this laser can exist before it is deleted.
        public const int TrueTimeLeft = 120;

        // Pretty self explanatory
        private const float maximumLength = 1600f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.timeLeft = TrueTimeLeft;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 6;
        }

        // Netcode for sending and receiving shit
        // localAI[0] is the timer, localAI[1] is the laser length

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            Projectile body = Main.projectile[(int)projectile.ai[1]];
            if (body.type != ModContent.ProjectileType<MidnightSunUFO>() || !body.active)
                projectile.Kill();

            if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
            {
                projectile.velocity = -Vector2.UnitY;
            }

            if (Main.projectile[(int)projectile.ai[1]].active)
            {
                projectile.Center = Main.projectile[(int)projectile.ai[1]].Center;
            }

            if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
            {
                projectile.velocity = -Vector2.UnitY;
            }

            // How fat the laser is
            float laserSize = 1f;

            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] >= TrueTimeLeft)
            {
                projectile.Kill();
                return;
            }

            // Causes the effect where the laser appears to expand/contract at the beginning and end of its life
            projectile.scale = (float)Math.Sin(projectile.localAI[0] * MathHelper.Pi / TrueTimeLeft) * 10f * laserSize;
            if (projectile.scale > laserSize)
            {
                projectile.scale = laserSize;
            }

            // The heart of the "sweeping rotation" part of the laser
            // Basically converts the velocity to a rotation, increments some value to that rotation,
            // and then converts the rotation to a velocity
            float velocityAsRotation = body.rotation + MathHelper.PiOver2;
            projectile.rotation = velocityAsRotation - MathHelper.PiOver2;
            projectile.velocity = velocityAsRotation.ToRotationVector2();

            Vector2 samplingPoint = projectile.Center;

            float[] samples = new float[3];

            float determinedLength = 0f;
            Collision.LaserScan(samplingPoint, projectile.velocity, projectile.width * projectile.scale, maximumLength, samples);
            for (int i = 0; i < samples.Length; i++)
            {
                determinedLength += samples[i];
            }
            determinedLength /= samples.Length;

            float lerpDelta = 0.5f;
            projectile.localAI[1] = MathHelper.Lerp(projectile.localAI[1], determinedLength, lerpDelta);
            Vector2 beamEndPosiiton = projectile.Center + projectile.velocity * (projectile.localAI[1] - 6f);
            for (int i = 0; i < 4; i++)
            {
                float theta = projectile.velocity.ToRotation() + Main.rand.NextBool(2).ToDirectionInt() * MathHelper.PiOver2;
                float speed = (float)Main.rand.NextDouble() * 2f + 2f;
                Vector2 velocity = theta.ToRotationVector2() * speed;
                Dust dust = Dust.NewDustDirect(beamEndPosiiton, 0, 0, 185, velocity.X, velocity.Y, 0, default, 1f);
                dust.noGravity = true;
                dust.scale = 1.7f;
            }

            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustPerfect(beamEndPosiiton, 185);
                dust.velocity = Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(55f)).RotatedBy(projectile.rotation);
                dust.noGravity = true;
                dust.scale = 1.2f;
            }

            // Draw light blue light across the laser
            DelegateMethods.v3_1 = new Vector3(0.52f, 0.93f, 0.97f);
            Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], projectile.width * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.velocity == Vector2.Zero)
            {
                return false;
            }
            Texture2D laserTailTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/MidnightSunBeamBegin");
            Texture2D laserBodyTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/MidnightSunBeamMid");
            Texture2D laserHeadTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/MidnightSunBeamEnd");
            float laserLength = projectile.localAI[1];
            Color drawColor = new Color(1f, 1f, 1f) * 0.9f;

            // Laser tail logic

            Main.spriteBatch.Draw(laserTailTexture, projectile.Center - Main.screenPosition, null, drawColor, projectile.rotation, laserTailTexture.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);

            // Laser body logic

            laserLength -= (laserTailTexture.Height / 2 + laserHeadTexture.Height) * projectile.scale;
            Vector2 centerDelta = projectile.Center;
            centerDelta += projectile.velocity * projectile.scale * (float)laserTailTexture.Height / 2f;
            if (laserLength > 0f)
            {
                float laserLengthDelta = 0f;
                Rectangle sourceRectangle = new Rectangle(0, 16 * (projectile.timeLeft / 3 % 5), laserBodyTexture.Width, 16);
                while (laserLengthDelta + 1f < laserLength)
                {
                    if (laserLength - laserLengthDelta < sourceRectangle.Height)
                    {
                        sourceRectangle.Height = (int)(laserLength - laserLengthDelta);
                    }
                    Main.spriteBatch.Draw(laserBodyTexture, centerDelta - Main.screenPosition, new Rectangle?(sourceRectangle), drawColor, projectile.rotation, new Vector2(sourceRectangle.Width / 2f, 0f), projectile.scale, SpriteEffects.None, 0f);
                    laserLengthDelta += sourceRectangle.Height * projectile.scale;
                    centerDelta += projectile.velocity * sourceRectangle.Height * projectile.scale;
                    sourceRectangle.Y += 16;
                    if (sourceRectangle.Y + sourceRectangle.Height > laserBodyTexture.Height)
                    {
                        sourceRectangle.Y = 0;
                    }
                }
            }

            // Laser head logic

            Main.spriteBatch.Draw(laserHeadTexture, centerDelta - Main.screenPosition, null, drawColor, projectile.rotation, laserHeadTexture.Frame(1, 1, 0, 0).Top(), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = projectile.velocity;
            Utils.PlotTileLine(projectile.Center, projectile.Center + unit * projectile.localAI[1], (float)projectile.width * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CutTiles));
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float value = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], 22f * projectile.scale, ref value))
            {
                return true;
            }
            return false;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Electrified, 300);
        }
    }
}
