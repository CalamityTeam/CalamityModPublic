using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class BallisticPoisonBombProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Poison Bomb");
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 200;
            projectile.tileCollide = false;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(6))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 14, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            try
            {
                int num223 = (int)(projectile.position.X / 16f) - 1;
                int num224 = (int)((projectile.position.X + (float)projectile.width) / 16f) + 2;
                int num225 = (int)(projectile.position.Y / 16f) - 1;
                int num226 = (int)((projectile.position.Y + (float)projectile.height) / 16f) + 2;
                if (num223 < 0)
                {
                    num223 = 0;
                }
                if (num224 > Main.maxTilesX)
                {
                    num224 = Main.maxTilesX;
                }
                if (num225 < 0)
                {
                    num225 = 0;
                }
                if (num226 > Main.maxTilesY)
                {
                    num226 = Main.maxTilesY;
                }
                for (int num227 = num223; num227 < num224; num227++)
                {
                    for (int num228 = num225; num228 < num226; num228++)
                    {
                        if (Main.tile[num227, num228] != null && !TileID.Sets.Platforms[Main.tile[num227, num228].type] && Main.tile[num227, num228].nactive() && (Main.tileSolid[(int)Main.tile[num227, num228].type] || (Main.tileSolidTop[(int)Main.tile[num227, num228].type] && Main.tile[num227, num228].frameY == 0)))
                        {
                            Vector2 vector19;
                            vector19.X = (float)(num227 * 16);
                            vector19.Y = (float)(num228 * 16);
                            if (projectile.position.X + (float)projectile.width - 4f > vector19.X && projectile.position.X + 4f < vector19.X + 16f && projectile.position.Y + (float)projectile.height - 4f > vector19.Y && projectile.position.Y + 4f < vector19.Y + 16f)
                            {
                                projectile.velocity.X = 0f;
                                projectile.velocity.Y = -0.2f;
                            }
                        }
                    }
                }
            } catch
            {
            }
            if (projectile.owner == Main.myPlayer && projectile.timeLeft <= 3)
            {
                projectile.tileCollide = false;
                projectile.ai[1] = 0f;
                projectile.alpha = 255;
                projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
                projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
                projectile.width = 128;
                projectile.height = 128;
                projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
                projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            }
            projectile.ai[0] += 1f;
            if (projectile.ai[0] > 10f)
            {
                projectile.ai[0] = 10f;
                if (projectile.velocity.Y == 0f && projectile.velocity.X != 0f)
                {
                    projectile.velocity.X = projectile.velocity.X * 0.97f;
                    if (projectile.velocity.X > -0.01f && projectile.velocity.X < 0.01f)
                    {
                        projectile.velocity.X = 0f;
                        projectile.netUpdate = true;
                    }
                }
                projectile.velocity.Y += 0.2f;
            }
            projectile.rotation += projectile.velocity.X * 0.1f;
        }

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 128;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            Main.PlaySound(SoundID.Item, (int)projectile.Center.X, (int)projectile.Center.Y, 14);
            int num251 = Main.rand.Next(3, 5);
            if (projectile.owner == Main.myPlayer)
            {
                for (int num252 = 0; num252 < num251; num252++)
                {
                    Vector2 value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    while (value15.X == 0f && value15.Y == 0f)
                    {
                        value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    }
                    value15.Normalize();
                    value15 *= (float)Main.rand.Next(70, 101) * 0.1f;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value15.X, value15.Y, ModContent.ProjectileType<BallisticPoisonBombSpike>(), (int)(projectile.damage * 0.5), 0f, projectile.owner, 0f, 0f);
                }
                int num320 = Main.rand.Next(8, 13);
                int num3;
                for (int num321 = 0; num321 < num320; num321 = num3 + 1)
                {
                    Vector2 vector15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    vector15.Normalize();
                    vector15 *= (float)Main.rand.Next(10, 201) * 0.01f;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, vector15.X, vector15.Y, ModContent.ProjectileType<BallisticPoisonCloud>(), (int)(projectile.damage * 0.3), 1f, projectile.owner, 0f, (float)Main.rand.Next(-45, 1));
                    num3 = num321;
                }
            }
            for (int num621 = 0; num621 < 5; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 14, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 9; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 6, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 6, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
            }
			projectile.ExplosionGores(3);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Venom, 240);
            projectile.Kill();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Venom, 240);
            projectile.Kill();
        }
    }
}
