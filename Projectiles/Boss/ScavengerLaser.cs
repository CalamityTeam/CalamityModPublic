using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class ScavengerLaser : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Homing Dart");
            Main.projFrames[projectile.type] = 5;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.timeLeft = 600;
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
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;

            projectile.frameCounter++;
            if (projectile.frameCounter > 5)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 4)
                projectile.frame = 0;

            projectile.alpha -= 40;
            if (projectile.alpha < 0)
                projectile.alpha = 0;

            if (projectile.alpha < 40)
            {
                int num805 = Dust.NewDust(projectile.Center - Vector2.One * 5f, 10, 10, 244, -projectile.velocity.X / 3f, -projectile.velocity.Y / 3f, 150, Color.Transparent, 0.6f);
                Main.dust[num805].noGravity = true;
            }

            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (projectile.ai[1] == -1f)
            {
                if (projectile.velocity.Length() < 18f)
                    projectile.velocity *= 1.05f;
                else
                    projectile.tileCollide = true;

                return;
            }

            if (projectile.ai[0] == 0f)
            {
                projectile.localAI[0] += 1f;
                if (projectile.localAI[0] >= 45f)
                {
                    projectile.localAI[0] = 0f;
                    projectile.ai[0] = 1f;
                    projectile.ai[1] = Player.FindClosest(projectile.position, projectile.width, projectile.height);
                    projectile.netUpdate = true;
                }

                projectile.velocity.X = projectile.velocity.RotatedBy(0.0, default).X;
                projectile.velocity.X = MathHelper.Clamp(projectile.velocity.X, -6f, 6f);

                projectile.velocity.Y = projectile.velocity.Y - 0.08f;
                if (projectile.velocity.Y > 0f)
                    projectile.velocity.Y = projectile.velocity.Y - 0.2f;
                if (projectile.velocity.Y < -7f)
                    projectile.velocity.Y = -7f;
            }
            else if (projectile.ai[0] == 1f)
            {
                if (Main.player[(int)projectile.ai[1]].Center.Y > projectile.Center.Y + 90f)
                {
                    projectile.ai[0] = 2f;
                    projectile.netUpdate = true;
                }

                projectile.velocity.X = projectile.velocity.RotatedBy(0.0, default).X;
                projectile.velocity.X = MathHelper.Clamp(projectile.velocity.X, -6f, 6f);

                projectile.velocity.Y = projectile.velocity.Y - 0.08f;
                if (projectile.velocity.Y > 0f)
                    projectile.velocity.Y = projectile.velocity.Y - 0.2f;
                if (projectile.velocity.Y < -7f)
                    projectile.velocity.Y = -7f;
            }
            else if (projectile.ai[0] == 2f)
            {
                if (Main.player[(int)projectile.ai[1]].Center.Y < projectile.Center.Y)
                    projectile.tileCollide = true;

                float speed = revenge ? 9f : 7.5f;

                Vector2 vector70 = Main.player[(int)projectile.ai[1]].Center - projectile.Center;
                if (vector70.Length() < 20f)
                {
                    projectile.Kill();
                    return;
                }

                vector70.Normalize();
                vector70 *= 14f;
                vector70 = Vector2.Lerp(projectile.velocity, vector70, 0.6f);
                if (vector70.Y < speed)
                    vector70.Y = speed;

                float num804 = 0.4f;
                if (projectile.velocity.X < vector70.X)
                {
                    projectile.velocity.X = projectile.velocity.X + num804;
                    if (projectile.velocity.X < 0f && vector70.X > 0f)
                        projectile.velocity.X = projectile.velocity.X + num804;
                }
                else if (projectile.velocity.X > vector70.X)
                {
                    projectile.velocity.X = projectile.velocity.X - num804;
                    if (projectile.velocity.X > 0f && vector70.X < 0f)
                        projectile.velocity.X = projectile.velocity.X - num804;
                }

                if (projectile.velocity.Y < vector70.Y)
                {
                    projectile.velocity.Y = projectile.velocity.Y + num804;
                    if (projectile.velocity.Y < 0f && vector70.Y > 0f)
                        projectile.velocity.Y = projectile.velocity.Y + num804;
                }
                else if (projectile.velocity.Y > vector70.Y)
                {
                    projectile.velocity.Y = projectile.velocity.Y - num804;
                    if (projectile.velocity.Y > 0f && vector70.Y < 0f)
                        projectile.velocity.Y = projectile.velocity.Y - num804;
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 180);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 50, 50, projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Zombie, (int)projectile.position.X, (int)projectile.position.Y, 103, 1f, 0f);
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 64;
            projectile.position.X = projectile.position.X - (projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (projectile.height / 2);
            for (int num193 = 0; num193 < 2; num193++)
            {
                Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 31, 0f, 0f, 100, default, 1.5f);
            }
            for (int num194 = 0; num194 < 20; num194++)
            {
                int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 0, default, 2.5f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 3f;
                num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, default, 1.5f);
                Main.dust[num195].velocity *= 2f;
                Main.dust[num195].noGravity = true;
            }
            projectile.Damage();
        }
    }
}
