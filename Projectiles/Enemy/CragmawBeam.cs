using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Enemy
{
    public class CragmawBeam : ModProjectile
    {
        // How long this laser can exist before it is deleted.
        public const int Lifetime = 120;

        // Pretty self explanatory
        private const float maximumLength = 1200f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gamma Ray");
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 22;
            projectile.hostile = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft = Lifetime;
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
            NPC body = Main.npc[(int)projectile.ai[1]];
            if (!body.active)
                projectile.Kill();

            // Sometimes the beam would readjust to some other, weird angle. This fixes that.
            if (projectile.velocity != -Vector2.UnitY)
            {
                projectile.velocity = -Vector2.UnitY;
                projectile.netUpdate = true;
            }

            if (Main.npc[(int)projectile.ai[1]].active)
            {
                projectile.Center = Main.npc[(int)projectile.ai[1]].Top + new Vector2(0f, 4f);
            }

            // How fat the laser is
            float laserSize = 1.6f;

            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] >= Lifetime)
            {
                projectile.Kill();
                return;
            }

            // Causes the effect where the laser appears to expand/contract at the beginning and end of its life
            projectile.scale = (float)Math.Sin(projectile.localAI[0] * MathHelper.Pi / Lifetime) * 10f * laserSize;
            if (projectile.scale > laserSize)
            {
                projectile.scale = laserSize;
            }

            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;

            Vector2 samplingPoint = projectile.Center;

            float[] samples = new float[3];

            float determinedLength = 0f;
            Collision.LaserScan(samplingPoint, projectile.velocity, projectile.width * projectile.scale, maximumLength, samples);
            for (int i = 0; i < samples.Length; i++)
            {
                determinedLength += samples[i];
            }
            determinedLength /= samples.Length;

            determinedLength = MathHelper.Clamp(determinedLength, maximumLength * 0.5f, maximumLength);

            float lerpDelta = 0.5f;
            projectile.localAI[1] = MathHelper.Lerp(projectile.localAI[1], determinedLength, lerpDelta);
            Vector2 beamEndPosiiton = projectile.Center + projectile.velocity * (projectile.localAI[1] - 6f);

            // Release acid along the beam periodically.
            if (projectile.timeLeft % 20 == 19 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 acidSpawnPosition = projectile.Center + projectile.velocity * projectile.localAI[1] * Main.rand.NextFloat(0.1f, 0.8f);
                if (!Main.player[Player.FindClosest(acidSpawnPosition, 1, 1)].WithinRange(acidSpawnPosition, 300f))
                    Projectile.NewProjectile(acidSpawnPosition, Main.rand.NextVector2CircularEdge(8f, 8f), ModContent.ProjectileType<CragmawAcidDrop>(), projectile.damage / 2, 0f);
            }

            if (WorldGen.SolidTile((int)(beamEndPosiiton.X / 16), (int)(beamEndPosiiton.Y / 16)))
            {
                for (int i = 0; i < 4; i++)
                {
                    float theta = projectile.velocity.ToRotation() + Main.rand.NextBool(2).ToDirectionInt() * MathHelper.PiOver2;
                    float speed = (float)Main.rand.NextDouble() * 2f + 2f;
                    Vector2 velocity = theta.ToRotationVector2() * speed;
                    Dust dust = Dust.NewDustDirect(beamEndPosiiton, 0, 0, (int)CalamityDusts.SulfurousSeaAcid, velocity.X, velocity.Y, 0, default, 1f);
                    dust.noGravity = true;
                    dust.scale = 2.1f;
                }

                for (int i = 0; i < 8; i++)
                {
                    Dust dust = Dust.NewDustPerfect(beamEndPosiiton, (int)CalamityDusts.SulfurousSeaAcid);
                    dust.velocity = Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(55f)).RotatedBy(projectile.rotation);
                    dust.noGravity = true;
                    dust.scale = 1.8f;
                }
                for (int i = 0; i < 16; i++)
                {
                    Dust dust = Dust.NewDustPerfect(beamEndPosiiton, (int)CalamityDusts.SulfurousSeaAcid);
                    dust.velocity = (i / 16f * MathHelper.TwoPi).ToRotationVector2() * 3f;
                    dust.noGravity = true;
                    dust.scale = 2.4f;
                }
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
            Texture2D laserTailTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/CragmawMireBeamBegin");
            Texture2D laserBodyTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/CragmawMireBeamMid");
            Texture2D laserHeadTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/CragmawMireBeamEnd");
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
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 300);
        }
    }
}
