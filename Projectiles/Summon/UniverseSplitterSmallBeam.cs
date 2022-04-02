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
	public class UniverseSplitterSmallBeam : ModProjectile
    {
        // Pretty self explanatory
        public const int FadeinTime = 25;
        public const int MovementTime = 60;
        public const int TimeLeft = 120;
        public const float TotalSweeps = 4f;
        public const float MaximumLength = 3000f;
        // How fat the laser is
        public const float LaserSize = 1.2f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Universe Splitter Ray");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft = TimeLeft;
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

        public static float CosineInterpolation(float start, float end, float time)
        {
            float cosineTime = (1f - (float)Math.Cos(MathHelper.Pi * time)) * 0.5f;
            return start * (1 - cosineTime) + end * cosineTime;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            if (projectile.localAI[0] <= MovementTime)
            {
                projectile.velocity = (projectile.ai[0] + (float)Math.Cos(projectile.localAI[0] / MovementTime * TotalSweeps) *
                                       CosineInterpolation(1f, 0f, projectile.localAI[0] / MovementTime) * UniverseSplitterField.SmallBeamAngleMax).ToRotationVector2();
            }
            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;

            projectile.localAI[0] += 1f;

            // Fadein/out. The timeleft varies, so this must be done manually.
            if (projectile.localAI[0] < FadeinTime)
            {
                projectile.scale = MathHelper.Lerp(0.01f, LaserSize, projectile.localAI[0] / FadeinTime);
            }
            if (projectile.localAI[0] > MovementTime &&
                projectile.localAI[0] <= MovementTime + 10f)
            {
                projectile.scale = MathHelper.Lerp(LaserSize, 3.25f, (projectile.localAI[0] - MovementTime) / 10f);
            }
            if (projectile.localAI[0] > TimeLeft - 15f)
            {
                projectile.scale = MathHelper.Lerp(3.25f, 0.01f, (projectile.localAI[0] - (TimeLeft - 15f)) / 15f);
            }

            projectile.localAI[1] = MathHelper.Lerp(projectile.localAI[1], MaximumLength, 0.5f);
            Vector2 beamEndPosiiton = projectile.Center + projectile.velocity * (projectile.localAI[1] - 6f);

            // Draw white light across the laser
            DelegateMethods.v3_1 = Color.White.ToVector3();
            Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], projectile.width * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.velocity == Vector2.Zero)
            {
                return false;
            }
            Texture2D laserTailTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/UniverseSplitterSmallBeamBegin");
            Texture2D laserBodyTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/UniverseSplitterSmallBeamMid");
            Texture2D laserHeadTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/UniverseSplitterSmallBeamEnd");
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
                Rectangle sourceRectangle = new Rectangle(0, 14 * (projectile.timeLeft / 3 % 5), laserBodyTexture.Width, 16);
                while (laserLengthDelta + 1f < laserLength)
                {
                    if (laserLength - laserLengthDelta < sourceRectangle.Height)
                    {
                        sourceRectangle.Height = (int)(laserLength - laserLengthDelta);
                    }
                    Main.spriteBatch.Draw(laserBodyTexture, centerDelta - Main.screenPosition, new Rectangle?(sourceRectangle), drawColor, projectile.rotation, new Vector2(sourceRectangle.Width / 2f, 0f), projectile.scale, SpriteEffects.None, 0f);
                    laserLengthDelta += sourceRectangle.Height * projectile.scale;
                    centerDelta += projectile.velocity * sourceRectangle.Height * projectile.scale;
                    sourceRectangle.Y += 14;
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
            target.AddBuff(BuffID.Electrified, 180);
        }
    }
}
