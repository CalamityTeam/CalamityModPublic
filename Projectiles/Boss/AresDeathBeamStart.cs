using CalamityMod.Events;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class AresDeathBeamStart : ModProjectile
    {
        private const int maxFrames = 5;
        private int frameDrawn = 0;

        public override void SetStaticDefaults()
        {
            // Ares' eight-pointed-star laser beams
            DisplayName.SetDefault("Exo Overload Beam");
        }

        public override void SetDefaults()
        {
            projectile.Calamity().canBreakPlayerDefense = true;
            projectile.width = 30;
            projectile.height = 30;
            projectile.hostile = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft = 600;
            cooldownSlot = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(frameDrawn);
            writer.Write(projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            frameDrawn = reader.ReadInt32();
            projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            // Difficulty modes
            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || malice;
            bool revenge = CalamityWorld.revenge || malice;
            bool expertMode = Main.expertMode || malice;

            Vector2? vector78 = null;

            if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
                projectile.velocity = -Vector2.UnitY;

            if (Main.npc[(int)projectile.ai[1]].active && Main.npc[(int)projectile.ai[1]].type == ModContent.NPCType<AresBody>())
            {
                Vector2 fireFrom = new Vector2(Main.npc[(int)projectile.ai[1]].Center.X - 1f, Main.npc[(int)projectile.ai[1]].Center.Y + 23f) + Vector2.Normalize(projectile.velocity) * 35f;
                projectile.position = fireFrom - new Vector2(projectile.width, projectile.height) / 2f;
            }
            else
                projectile.Kill();

            float scaleLimit = 1f;
            float duration = AresBody.deathrayDuration;
			float rotationSpeed = Main.npc[(int)projectile.ai[1]].Calamity().newAI[2] - AresBody.deathrayTelegraphDuration;
            if (Main.npc[(int)projectile.ai[1]].Calamity().newAI[0] != (float)AresBody.Phase.Deathrays)
            {
                projectile.Kill();
                return;
            }

            projectile.scale = (float)Math.Sin(rotationSpeed * (float)Math.PI / duration) * 10f * scaleLimit;
            if (projectile.scale > scaleLimit)
                projectile.scale = scaleLimit;

            float num804 = projectile.velocity.ToRotation();
            float divisor = malice ? 300f : death ? 320f : revenge ? 330f : expertMode ? 340f : 360f;
            float rotationAmt = MathHelper.Lerp(0f, MathHelper.TwoPi / divisor, rotationSpeed / duration);
            num804 += rotationAmt;
            projectile.rotation = num804 - MathHelper.PiOver2;
            projectile.velocity = num804.ToRotationVector2();

            float num805 = 3f; //3f
            float num806 = projectile.width;

            Vector2 samplingPoint = projectile.Center;
            if (vector78.HasValue)
                samplingPoint = vector78.Value;

            float[] array3 = new float[(int)num805];
            Collision.LaserScan(samplingPoint, projectile.velocity, num806 * projectile.scale, 3600f, array3);
            float num807 = 0f;
            for (int num808 = 0; num808 < array3.Length; num808++)
            {
                num807 += array3[num808];
            }
            num807 /= num805;

            // Fire laser through walls at max length if target cannot be seen
            if (!Collision.CanHitLine(Main.npc[(int)projectile.ai[1]].Center, 1, 1, Main.player[Main.npc[(int)projectile.ai[1]].target].Center, 1, 1))
            {
                num807 = 3600f;
            }

            float amount = 0.5f;
            projectile.localAI[0] = MathHelper.Lerp(projectile.localAI[0], num807, amount); // Length of laser, linear interpolation

            // Spawn dust at the end of the beam
            int dustType = 107;
            Vector2 dustPos = projectile.Center + projectile.velocity * (projectile.localAI[0] - 14f);
            for (int i = 0; i < 2; i++)
            {
                float dustRot = projectile.velocity.ToRotation() + ((Main.rand.Next(2) == 1) ? -1f : 1f) * MathHelper.PiOver2;
                float dustVelMult = (float)Main.rand.NextDouble() * 2f + 2f;
                Vector2 dustVel = new Vector2((float)Math.Cos(dustRot) * dustVelMult, (float)Math.Sin(dustRot) * dustVelMult);
                int dust = Dust.NewDust(dustPos, 0, 0, dustType, dustVel.X, dustVel.Y, 0, new Color(0, 255, 255), 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale = 1.7f;
            }

            if (Main.rand.NextBool(5))
            {
                Vector2 dustRot = projectile.velocity.RotatedBy(MathHelper.PiOver2, default) * ((float)Main.rand.NextDouble() - 0.5f) * projectile.width;
                int dust = Dust.NewDust(dustPos + dustRot - Vector2.One * 4f, 8, 8, dustType, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
                Main.dust[dust].velocity *= 0.5f;
                Main.dust[dust].velocity.Y = -Math.Abs(Main.dust[dust].velocity.Y);
            }

            DelegateMethods.v3_1 = new Vector3(0.9f, 0.3f, 0.3f);
            Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[0], projectile.width * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.velocity == Vector2.Zero || !Main.npc[(int)projectile.ai[1]].active || Main.npc[(int)projectile.ai[1]].type != ModContent.NPCType<AresBody>())
                return false;

            Texture2D beamStart = Main.projectileTexture[projectile.type];
            Texture2D beamMiddle = ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/AresDeathBeamMiddle");
            Texture2D beamEnd = ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/AresDeathBeamEnd");

            float drawLength = projectile.localAI[0];
            Color color = new Color(250, 250, 250, 100);

            if ((Main.npc[(int)projectile.ai[1]].Calamity().newAI[2] - AresBody.deathrayTelegraphDuration) % 5f == 0f)
            {
                frameDrawn++;
                if (frameDrawn >= maxFrames)
                    frameDrawn = 0;
            }

            // Draw start of beam
            Vector2 vector = projectile.Center - Main.screenPosition;
            Rectangle? sourceRectangle = new Rectangle(0, beamStart.Height / maxFrames * frameDrawn, beamStart.Width, beamStart.Height / maxFrames);
            spriteBatch.Draw(beamStart, vector, sourceRectangle, color, projectile.rotation, new Vector2(beamStart.Width, beamStart.Height / maxFrames) / 2f, projectile.scale, SpriteEffects.None, 0f);

            // Draw middle of beam
            drawLength -= (beamStart.Height / maxFrames / 2 + beamEnd.Height / maxFrames) * projectile.scale;
            Vector2 center = projectile.Center;
            center += projectile.velocity * projectile.scale * beamStart.Height / maxFrames / 2f;
            if (drawLength > 0f)
            {
                float i = 0f;
                int middleFrameDrawn = frameDrawn;
                while (i + 1f < drawLength)
                {
                    Rectangle rectangle = new Rectangle(0, beamMiddle.Height / maxFrames * middleFrameDrawn, beamMiddle.Width, beamMiddle.Height / maxFrames);

                    if (drawLength - i < rectangle.Height)
                        rectangle.Height = (int)(drawLength - i);

                    spriteBatch.Draw(beamMiddle, center - Main.screenPosition, rectangle, color, projectile.rotation, new Vector2(rectangle.Width / 2f, 0f), projectile.scale, SpriteEffects.None, 0f);

                    middleFrameDrawn++;
                    if (middleFrameDrawn >= maxFrames)
                        middleFrameDrawn = 0;

                    i += rectangle.Height * projectile.scale;
                    center += projectile.velocity * rectangle.Height * projectile.scale;

                    rectangle.Y += beamMiddle.Height / maxFrames;
                    if (rectangle.Y + rectangle.Height > beamMiddle.Height / maxFrames)
                        rectangle.Y = 0;
                }
            }

            // Draw end of beam
            Vector2 vector2 = center - Main.screenPosition;
            sourceRectangle = new Rectangle(0, beamEnd.Height / maxFrames * frameDrawn, beamEnd.Width, beamEnd.Height / maxFrames);
            spriteBatch.Draw(beamEnd, vector2, sourceRectangle, color, projectile.rotation, new Vector2(beamEnd.Width, beamEnd.Height / maxFrames) / 2f, projectile.scale, SpriteEffects.None, 0f);

            return false;
        }

        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = projectile.velocity;
            Utils.PlotTileLine(projectile.Center, projectile.Center + unit * projectile.localAI[0], projectile.width * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CutTiles));
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float num6 = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[0], 30f * projectile.scale, ref num6))
            {
                return true;
            }
            return false;
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)	
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
