using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class ThunderstormShot : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shot");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 1;
            projectile.alpha = 255;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 25;
        }

        public override void AI()
        {
            projectile.ai[0] += 1f;
            if (projectile.ai[0] > 9f)
            {
                for (int num121 = 0; num121 < 5; num121++)
                {
                    Dust dust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 107, projectile.velocity.X, projectile.velocity.Y, 100, default, 1f)];
                    dust.velocity = Vector2.Zero;
                    dust.position -= projectile.velocity / 5f * (float)num121;
                    dust.noGravity = true;
                    dust.noLight = true;
                    Dust dust2 = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, projectile.velocity.X, projectile.velocity.Y, 100, default, 1f)];
                    dust2.velocity = Vector2.Zero;
                    dust2.position -= projectile.velocity / 5f * (float)num121;
                    dust2.noGravity = true;
                    dust2.noLight = true;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item125, projectile.position);
            for (int n = 0; n < 5; n++)
            {
                float x = projectile.position.X + (float)Main.rand.Next(-200, 200);
                float y = projectile.position.Y - 1500f;
                Vector2 vector = new Vector2(x, y);
                float num13 = projectile.position.X + (float)(projectile.width / 2) - vector.X;
                float num14 = projectile.position.Y + (float)(projectile.height / 2) - vector.Y;
                num13 += (float)Main.rand.Next(-100, 101);
                int num15 = 29;
                float num16 = (float)Math.Sqrt((double)(num13 * num13 + num14 * num14));
                num16 = (float)num15 / num16;
                num13 *= num16;
                num14 *= num16;
                Projectile.NewProjectile(x, y, num13, num14, ModContent.ProjectileType<ThunderstormShotSplit>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
            }
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 36;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            int num226 = 36;
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 1f; //0.75
                vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + projectile.Center;
                Vector2 vector7 = vector6 - projectile.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 229, vector7.X, vector7.Y, 100, default, 1.2f);
                Main.dust[num228].noGravity = true;
                Main.dust[num228].noLight = true;
                Main.dust[num228].velocity = vector7;
            }
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + projectile.Center;
                Vector2 vector7 = vector6 - projectile.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 107, vector7.X, vector7.Y, 100, default, 1.2f);
                Main.dust[num228].noGravity = true;
                Main.dust[num228].noLight = true;
                Main.dust[num228].velocity = vector7;
            }
            projectile.Damage();
        }
    }
}
