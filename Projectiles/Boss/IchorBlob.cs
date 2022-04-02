using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class IchorBlob : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ichor Blob");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            Main.projFrames[projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            projectile.width = 52;
            projectile.height = 56;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

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
            if (projectile.position.Y > projectile.ai[1] - 32f)
                projectile.tileCollide = true;

            // Deal no damage and increment the variable used to kill the projectile after 15 seconds
            projectile.localAI[1] += 1f;
            if (projectile.localAI[1] > 900f)
            {
                projectile.localAI[0] += 10f;
                projectile.damage = 0;
            }

            // Kill the projectile 26 frames after it stops dealing damage
            if (projectile.localAI[0] > 255f)
            {
                projectile.Kill();
                projectile.localAI[0] = 255f;
            }

            // Add yellow light based on alpha
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.2f / 255f, (255 - projectile.alpha) * 0.16f / 255f, (255 - projectile.alpha) * 0.04f / 255f);

            // Adjust projectile visibility based on the kill timer
            projectile.alpha = (int)(100.0 + projectile.localAI[0] * 0.7);

            if (projectile.velocity.Y != 0f && projectile.ai[0] == 0f)
            {
                // Rotate based on velocity, only do this here, because it's falling
                projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) - MathHelper.PiOver2;

                projectile.frameCounter++;
                if (projectile.frameCounter > 6)
                {
                    projectile.frame++;
                    projectile.frameCounter = 0;
                }
                if (projectile.frame > 1)
                    projectile.frame = 0;
            }
            else
            {
                // Prevent sliding
                projectile.velocity.X = 0f;

                // Do not animate falling frames
                projectile.ai[0] = 1f;

                if (projectile.frame < 2)
                {
                    // Set frame to blob and frame counter to 0
                    projectile.frame = 2;
                    projectile.frameCounter = 0;

                    // Play squish sound
                    Main.PlaySound(SoundID.NPCDeath21, projectile.Center);

                    // Emit dust
                    float num50 = 1.6f;
                    float num51 = 0.8f;
                    float num52 = 2f;
                    Vector2 value3 = (projectile.rotation - MathHelper.PiOver2).ToRotationVector2();
                    Vector2 value4 = value3 * projectile.velocity.Length();
                    for (int num53 = 0; num53 < 10; num53++)
                    {
                        int num54 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 170, 0f, 0f, 200, default, num50);
                        Dust dust = Main.dust[num54];
                        dust.position = projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * projectile.width / 2f;
                        dust.noGravity = true;
                        dust.velocity.Y -= 2f;
                        dust.velocity *= 3f;
                        dust.velocity += value4 * Main.rand.NextFloat();
                        num54 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 170, 0f, 0f, 100, default, num51);
                        dust.position = projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * projectile.width / 2f;
                        dust.velocity.Y -= 2f;
                        dust.velocity *= 2f;
                        dust.noGravity = true;
                        dust.fadeIn = 1f;
                        dust.velocity += value4 * Main.rand.NextFloat();
                    }
                    for (int num55 = 0; num55 < 5; num55++)
                    {
                        int num56 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 170, 0f, 0f, 0, default, num52);
                        Dust dust = Main.dust[num56];
                        dust.position = projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(projectile.velocity.ToRotation()) * projectile.width / 3f;
                        dust.noGravity = true;
                        dust.velocity.Y -= 2f;
                        dust.velocity *= 0.5f;
                        dust.velocity += value4 * (0.6f + 0.6f * Main.rand.NextFloat());
                    }
                }

                projectile.rotation = 0f;

                projectile.frameCounter++;
                if (projectile.frameCounter > 6)
                {
                    projectile.frame++;
                    projectile.frameCounter = 0;
                }
                if (projectile.frame > 5)
                    projectile.frame = 5;
            }

            // Do velocity code after the frame code, to avoid messing anything up
            // Reduce x velocity every frame
            projectile.velocity.X *= 0.99f;

            // Stop falling if water or lava is hit
            if (projectile.wet || projectile.lavaWet)
            {
                projectile.velocity.Y = 0f;
            }
            else
            {
                // Fall
                projectile.velocity.Y += 0.1f;
                if (projectile.velocity.Y > 6f)
                    projectile.velocity.Y = 6f;
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = false;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(projectile.Center, 16f, targetHitbox);

        public override bool CanHitPlayer(Player target) => projectile.localAI[1] <= 900f && projectile.localAI[1] > 120f;

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.localAI[1] > 900f)
            {
                byte b2 = (byte)((26f - (projectile.localAI[1] - 900f)) * 10f);
                byte a2 = (byte)(projectile.alpha * (b2 / 255f));
                return new Color(b2, b2, b2, a2);
            }
            return new Color(255, 255, 255, projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor);
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (projectile.localAI[1] <= 900f && projectile.localAI[1] > 120f)
                target.AddBuff(BuffID.Ichor, 240);
        }
    }
}
