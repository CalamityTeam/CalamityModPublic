using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Enums;
using Terraria.ModLoader;
using CalamityMod.Buffs.DamageOverTime;
using Terraria.GameContent.Events;

namespace CalamityMod.Projectiles.Summon
{
    public class UniverseSplitterHugeBeam : ModProjectile
    {
        // Pretty self explanatory
        public const int TotalFadeoutTime = 25;
        public const int TimeLeft = 180;
        public const float MaximumLength = 3000f;
        // How fat the laser is
        public const float LaserSize = 1.45f;

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

        public override void AI()
        {
            projectile.velocity = (projectile.velocity.ToRotation() + projectile.ai[0]).ToRotationVector2();
            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;

            projectile.localAI[0]++;

            // Fadein/out. The timeleft varies, so this must be done manually.
            if (projectile.localAI[0] < TotalFadeoutTime)
            {
                projectile.scale = MathHelper.Lerp(0.01f, LaserSize, projectile.localAI[0] / TotalFadeoutTime);
            }

            if (projectile.timeLeft < TotalFadeoutTime)
            {
                projectile.scale = MathHelper.Lerp(LaserSize, 0f, 1f - (projectile.timeLeft / (float)TotalFadeoutTime));
            }

            Vector2 samplingPoint = projectile.Top;

            float[] samples = new float[12];

            float determinedLength = 0f;
            Collision.LaserScan(samplingPoint, projectile.velocity, projectile.width * projectile.scale, MaximumLength, samples);
            for (int i = 0; i < samples.Length; i++)
            {
                determinedLength += samples[i];
            }
            determinedLength /= samples.Length;

            determinedLength = MathHelper.Clamp(determinedLength, MaximumLength * 0.3f, MaximumLength);

            float lerpDelta = 0.5f;
            projectile.localAI[1] = MathHelper.Lerp(projectile.localAI[1], determinedLength - 20f, lerpDelta);
            if (projectile.localAI[0] < 30f)
            {
                projectile.localAI[1] = MathHelper.Lerp(72f, projectile.localAI[1], projectile.localAI[0] / 30f);
            }
            Vector2 beamEndPosition = projectile.Center + projectile.velocity * (projectile.localAI[1] - 6f);
            if (projectile.localAI[0] >= 30f &&
                projectile.localAI[0] <= 35f)
            {
                if (projectile.localAI[0] == 30f)
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/TeslaCannonFire"), projectile.Center);
                for (int i = 0; i < 75; i++)
                {
                    Dust dust = Dust.NewDustPerfect(beamEndPosition, 269);
                    dust.velocity = Main.rand.NextVector2Circular(16f, 11f);
                    dust.velocity = dust.velocity.SafeNormalize(Vector2.UnitY) * new Vector2(5f, 3.5f);
                    dust.scale = Main.rand.NextFloat(1.2f, 1.5f);
                    dust.noGravity = true;

                    dust = Dust.NewDustPerfect(beamEndPosition, 269);
                    dust.velocity = (i / 75f * MathHelper.TwoPi).ToRotationVector2().RotatedByRandom(0.2f) * new Vector2(16f, 11f) * 1.3f;
                    dust.scale = Main.rand.NextFloat(1.4f, 1.75f);
                    dust.noGravity = true;
                }
            }

            if (projectile.localAI[0] > TimeLeft - 50f)
            {
                float light = 1f;
                if (projectile.localAI[0] < TimeLeft - 30f)
                {
                    light = MathHelper.Lerp(0f, 1f, (projectile.localAI[0] - (TimeLeft - 30f)) / 30f);
                }
                MoonlordDeathDrama.RequestLight(light, beamEndPosition);
            }

            // Draw acid green light across the laser
            DelegateMethods.v3_1 = new Vector3(0.62f, 0.94f, 0.38f);
            Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], projectile.width * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.velocity == Vector2.Zero)
            {
                return false;
            }
            Texture2D laserTailTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/UniverseSplitterHugeBeamEnd");
            Texture2D laserBodyTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/UniverseSplitterHugeBeamMid");
            float laserLength = projectile.localAI[1];
            Color drawColor = new Color(1f, 1f, 1f) * 0.9f;

            // Laser body logic

            laserLength -= laserTailTexture.Height / 2 * projectile.scale;
            Vector2 centerDelta = projectile.Center - 76f * projectile.velocity * projectile.scale;
            centerDelta += projectile.velocity * projectile.scale * laserTailTexture.Height / 2f;
            if (laserLength > 0f)
            {
                float laserLengthDelta = 0f;
                Rectangle sourceRectangle = new Rectangle(0, 76 * (projectile.timeLeft / 3 % 5), laserBodyTexture.Width, 76);
                while (laserLengthDelta + 1f < laserLength)
                {
                    if (laserLength - laserLengthDelta < sourceRectangle.Height)
                    {
                        sourceRectangle.Height = (int)(laserLength - laserLengthDelta);
                    }
                    Main.spriteBatch.Draw(laserBodyTexture, centerDelta - Main.screenPosition, new Rectangle?(sourceRectangle), drawColor, projectile.rotation, new Vector2(sourceRectangle.Width / 2f, 0f), projectile.scale, SpriteEffects.None, 0f);
                    laserLengthDelta += sourceRectangle.Height * projectile.scale;
                    centerDelta += projectile.velocity * sourceRectangle.Height * projectile.scale;
                    sourceRectangle.Y += (int)(76 * projectile.scale);
                    if (sourceRectangle.Y + sourceRectangle.Height > laserBodyTexture.Height)
                    {
                        sourceRectangle.Y = 0;
                    }
                }
            }

            // Laser tail logic
            centerDelta += projectile.velocity * projectile.scale * 38f;
            Rectangle tailFrameRectangle = new Rectangle(0, 76 * (projectile.timeLeft / 3 % 5), laserTailTexture.Width, 76);
            Main.spriteBatch.Draw(laserTailTexture, centerDelta - Main.screenPosition, tailFrameRectangle, drawColor, projectile.rotation, new Vector2(148f, 76f) / 2f, projectile.scale, SpriteEffects.None, 0f);

            return false;
        }

        public override bool ShouldUpdatePosition() => false;

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
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity * (projectile.localAI[1] + 76f), 22f * projectile.scale, ref value))
            {
                return true;
            }
            return false;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Electrified, 180);
            target.AddBuff(ModContent.BuffType<Nightwither>(), 180);
        }
    }
}
