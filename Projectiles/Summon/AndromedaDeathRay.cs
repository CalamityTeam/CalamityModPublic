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
	public class AndromedaDeathRay : ModProjectile
    {
        // How long this laser can exist before it is deleted.
        public const int TrueTimeLeft = 25;

        // Pretty self explanatory
        private const float maximumLength = 2000f;

        public float AngularMultiplier = 0f;

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
            projectile.timeLeft = TrueTimeLeft;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 4;
        }

        // Netcode for sending and receiving shit
        // localAI[0] is the timer, localAI[1] is the laser length

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(projectile.localAI[1]);
            writer.Write(AngularMultiplier);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            projectile.localAI[1] = reader.ReadSingle();
            AngularMultiplier = reader.ReadSingle();
        }

        public override void AI()
        {
            Projectile owner = Main.projectile[(int)projectile.ai[0]];
            Player player = Main.player[projectile.owner];

            if (owner.type != ModContent.ProjectileType<GiantIbanRobotOfDoom>() || !owner.active)
                projectile.Kill();
            if (owner.active)
            {
                projectile.Center = player.Center + new Vector2(owner.spriteDirection == 1 ? 48f : 22f, -28f);
                if (player.Calamity().andromedaState == AndromedaPlayerState.SmallRobot)
                {
                    projectile.Center = player.Center + new Vector2(owner.spriteDirection == 1 ? 24f : 2f, 0f);
                }
            }

            // How fat the laser is
            float laserSize = 0.6f + (float)Math.Sin(projectile.localAI[0] / 7f) * 0.025f; // The sine gives a pulsating effect to the laser

            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] >= TrueTimeLeft)
            {
                projectile.Kill();
                return;
            }

            // Causes the effect where the laser appears to expand/contract at the beginning and end of its life
            projectile.scale = (float)Math.Sin(projectile.localAI[0] * MathHelper.Pi / TrueTimeLeft) * 5f * laserSize;
            if (projectile.scale > laserSize)
            {
                projectile.scale = laserSize;
            }

            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;

            // Generate dust form the beginning of the ray, giving an illusion of electricity escaping the laser cannon.
            if (!Main.dedServ)
            {
                for (int i = 0; i < 4; i++)
                {
                    Dust dust = Dust.NewDustPerfect(projectile.Center, 133);
                    dust.velocity = projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(2f, 3.5f) + Main.player[projectile.owner].velocity * 1.5f;
                    dust.fadeIn = Main.rand.NextFloat(0.6f, 0.85f);
                    if (player.Calamity().andromedaState == AndromedaPlayerState.SmallRobot)
                    {
                        dust.fadeIn = 0.05f;
                        dust.scale = 0.8f;
                    }
                    dust.noGravity = true;
                }
            }

            Vector2 samplingPoint = projectile.Center;

            float[] samples = new float[3];

            float determinedLength = 0f;
            Collision.LaserScan(samplingPoint, projectile.velocity, projectile.width * projectile.scale, maximumLength, samples);
            for (int i = 0; i < samples.Length; i++)
            {
                determinedLength += samples[i];
            }
            determinedLength /= samples.Length;

            projectile.localAI[1] = MathHelper.Lerp(projectile.localAI[1], determinedLength, 0.5f);
            Vector2 beamEndPosiiton = projectile.Center + projectile.velocity * projectile.localAI[1];
            // Dust at the end of the beam, if there's a tile there.
            if (Collision.SolidCollision(beamEndPosiiton, 16, 16) && !Main.dedServ)
            {
                if (AngularMultiplier == 0f)
                    AngularMultiplier = Main.rand.NextFloat(1f, 4f);
                float angle = projectile.localAI[0] / 18f;
                float x = (float)Math.Sin(angle * AngularMultiplier) * (float)Math.Cos(angle);
                float y = (float)Math.Cos(angle * AngularMultiplier) * (float)Math.Sin(angle);
                Vector2 velocity = new Vector2(x * 4.5f, y * 2f);

                for (int i = 0; i < 3; i++)
                {
                    Dust dust = Dust.NewDustPerfect(beamEndPosiiton + angle.ToRotationVector2() * 8f, 133);
                    dust.velocity = velocity.RotatedBy(MathHelper.TwoPi / 3f * i);
                    dust.scale = (float)Math.Cos(angle) + 1.2f;
                    dust.noGravity = true;
                }
            }

            // Draw light blue light across the laser
            DelegateMethods.v3_1 = Color.SkyBlue.ToVector3();
            Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], projectile.width * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.velocity == Vector2.Zero)
            {
                return false;
            }
            Texture2D laserTailTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/AndromedaDeathrayBegin");
            Texture2D laserBodyTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/AndromedaDeathrayMid");
            Texture2D laserHeadTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/AndromedaDeathrayEnd");
            float laserLength = projectile.localAI[1];
            Color drawColor = new Color(1f, 1f, 1f) * 0.9f;

            // Laser tail logic

            Main.spriteBatch.Draw(laserTailTexture, projectile.Center - Main.screenPosition, null, drawColor, projectile.rotation, laserTailTexture.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);

            // Laser body logic

            laserLength -= (laserTailTexture.Height / 2 + laserHeadTexture.Height) * projectile.scale;
            Vector2 centerDelta = projectile.Center;
            centerDelta += projectile.velocity * projectile.scale * laserTailTexture.Height / 2f;
            if (laserLength > 0f)
            {
                float laserLengthDelta = 0f;
                Rectangle sourceRectangle = new Rectangle(0, 0, laserBodyTexture.Width, laserBodyTexture.Height);
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

            Main.spriteBatch.Draw(laserHeadTexture, centerDelta - Main.screenPosition, null, drawColor, projectile.rotation, laserHeadTexture.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
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
    }
}
