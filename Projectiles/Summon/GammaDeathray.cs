using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
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
    public class GammaDeathray : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Enemy/CragmawBeam";

        public float MaximumLength => projectile.Distance(Main.MouseWorld);
        // How fat the laser is
        public const float LaserSize = 1.35f;
        // Pretty self explanatory
        public const int TotalFadeoutTime = 45;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gamma Ray");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 22;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft = 10000;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 7;
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
            Projectile owner = Main.projectile[(int)projectile.ai[0]];
            if (!owner.active || owner.type != ModContent.ProjectileType<GammaHead>())
                projectile.Kill();

            if (Main.myPlayer == projectile.owner)
            {
                projectile.velocity = projectile.AngleTo(Main.MouseWorld).ToRotationVector2();
                projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;

                // Since the logic for this can only be run by one client, it must constantly be synced.
                projectile.netUpdate = true;
                projectile.netSpam = 0;
            }

            if (owner.active)
            {
                projectile.Center = owner.Center + projectile.AngleTo(Main.MouseWorld).ToRotationVector2() * 12f;
            }
            projectile.localAI[0] += 1f;

            // Fadein/out. The timeleft varies, so this must be done manually.
            if (projectile.localAI[0] < TotalFadeoutTime)
            {
                projectile.scale = MathHelper.Lerp(0.01f, LaserSize, projectile.localAI[0] / TotalFadeoutTime);
            }

            if (projectile.timeLeft < TotalFadeoutTime)
            {
                projectile.scale = MathHelper.Lerp(LaserSize, 0f, 1f - (projectile.timeLeft / (float)TotalFadeoutTime));
            }

            for (int i = 0; i < 6; i++)
            {
                Dust dust = Dust.NewDustPerfect(projectile.Center, (int)CalamityDusts.SulfurousSeaAcid);
                dust.velocity = projectile.velocity.RotatedBy(MathHelper.Pi * 0.75f) * (i >= 3).ToDirectionInt();
                dust.velocity = dust.velocity.RotatedByRandom(0.3f);
                dust.noGravity = true;
                dust.scale = 1f / (float)Math.Cos(i / 6f * MathHelper.PiOver2) / (float)Math.Sqrt(i + 1);
            }

            Vector2 samplingPoint = projectile.Center;

            float[] samples = new float[3];

            float determinedLength = 0f;
            Collision.LaserScan(samplingPoint, projectile.velocity, projectile.width * projectile.scale, MaximumLength, samples);
            for (int i = 0; i < samples.Length; i++)
            {
                determinedLength += samples[i];
            }
            determinedLength /= samples.Length;

            determinedLength = MathHelper.Clamp(determinedLength, MaximumLength * 0.5f, MaximumLength);

            float lerpDelta = 0.5f;
            projectile.localAI[1] = MathHelper.Lerp(projectile.localAI[1], determinedLength, lerpDelta);
            Vector2 beamEndPosiiton = projectile.Center + projectile.velocity * (projectile.localAI[1] - 6f);

            // Draw acid green light across the laser
            DelegateMethods.v3_1 = new Vector3(0.62f, 0.94f, 0.38f);
            Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], projectile.width * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));
        }
        public override void Kill(int timeLeft)
        {
            Main.projectile[(int)projectile.ai[0]].ai[0] = 0f; // Detach this laser from its owner.
            Main.projectile[(int)projectile.ai[0]].netUpdate = true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.velocity == Vector2.Zero)
            {
                return false;
            }
            Texture2D laserTailTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/GammaBeamBegin");
            Texture2D laserBodyTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/GammaBeamMid");
            Texture2D laserHeadTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/GammaBeamEnd");
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
            target.AddBuff(ModContent.BuffType<Irradiated>(), 300);
        }
    }
}
