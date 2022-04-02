using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class DreadmineTurret : ModProjectile
    {
        public float count = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dreadmine Turret");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.sentry = true;
            projectile.timeLeft = Projectile.SentryLifeTime;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                projectile.localAI[0] += 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = damage2;
            }
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }
            projectile.velocity = new Vector2(0f, (float)Math.Sin((double)(6.28318548f * projectile.ai[0] / 300f)) * 0.5f);
            projectile.ai[0] += 1f;
            if (projectile.ai[0] >= 300f)
            {
                projectile.ai[0] = 0f;
                projectile.netUpdate = true;
            }
            if (Main.rand.NextBool(15))
            {
                int num = 0;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == ModContent.ProjectileType<Dreadmine>())
                    {
                        num++;
                    }
                }
                if (Main.rand.Next(15) >= num && num < 10)
                {
                    int num2 = 50;
                    int num3 = 24;
                    int num4 = 90;
                    for (int j = 0; j < num2; j++)
                    {
                        int num5 = Main.rand.Next(200 - j * 2, 400 + j * 2);
                        Vector2 center = projectile.Center;
                        center.X += (float)Main.rand.Next(-num5, num5 + 1);
                        center.Y += (float)Main.rand.Next(-num5, num5 + 1);
                        if (!Collision.SolidCollision(center, num3, num3))
                        {
                            center.X += (float)(num3 / 2);
                            center.Y += (float)(num3 / 2);
                            if (Collision.CanHit(new Vector2(projectile.Center.X, projectile.position.Y), 1, 1, center, 1, 1) ||
                                Collision.CanHit(new Vector2(projectile.Center.X, projectile.position.Y - 50f), 1, 1, center, 1, 1))
                            {
                                int num6 = (int)center.X / 16;
                                int num7 = (int)center.Y / 16;
                                bool flag = false;
                                if (Main.rand.NextBool(3) && Main.tile[num6, num7] != null && Main.tile[num6, num7].wall > 0)
                                {
                                    flag = true;
                                }
                                else
                                {
                                    center.X -= (float)(num4 / 2);
                                    center.Y -= (float)(num4 / 2);
                                    if (Collision.SolidCollision(center, num4, num4))
                                    {
                                        center.X += (float)(num4 / 2);
                                        center.Y += (float)(num4 / 2);
                                        flag = true;
                                    }
                                }
                                if (flag)
                                {
                                    for (int k = 0; k < Main.maxProjectiles; k++)
                                    {
                                        if (Main.projectile[k].active && Main.projectile[k].owner == Main.myPlayer &&
                                            Main.projectile[k].type == ModContent.ProjectileType<Dreadmine>() && (center - Main.projectile[k].Center).Length() < 48f)
                                        {
                                            flag = false;
                                            break;
                                        }
                                    }
                                    if (flag && Main.myPlayer == projectile.owner)
                                    {
                                        Projectile.NewProjectile(center.X, center.Y, 0f, 0f, ModContent.ProjectileType<Dreadmine>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public override bool CanDamage()
        {
            return false;
        }
    }
}
