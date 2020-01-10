using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Dusts;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Rogue
{
    public class RegulusRiotProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Regulus Riot");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 28;
            projectile.friendly = true;
            projectile.penetrate = 3;
            projectile.extraUpdates = 1;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.Calamity().rogue = true;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 15;
        }

        public override void AI()
        {
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 20;
            }
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
			int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, ModContent.DustType<AstralBlue>(), 0f, 0f, 100, default, 1f);
			Main.dust[num469].noGravity = true;
			Main.dust[num469].velocity *= 0f;
			num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1f);
			Main.dust[num469].noGravity = true;
			Main.dust[num469].velocity *= 0f;
            projectile.ai[0] += 1f;
            int num1013 = 0;
            if (projectile.velocity.Length() <= 8f) //4
            {
                num1013 = 1;
            }
            if (num1013 == 0)
            {
                projectile.rotation -= 0.104719758f;

                if (projectile.ai[0] >= 30f)
                {
					projectile.extraUpdates = 2;
                    projectile.velocity *= 0.98f;
                    projectile.rotation -= 0.0174532924f;
                }
                if (projectile.velocity.Length() < 8.2f) //4.1
                {
                    projectile.velocity.Normalize();
                    projectile.velocity *= 4f;
                    projectile.ai[0] = 0f;
					projectile.extraUpdates = 1;
                }
            }
            else if (num1013 == 1)
            {
                int num3;
                projectile.rotation -= 0.104719758f;
                Vector2 vector145 = projectile.Center;
                float num1015 = 300f;
                bool flag59 = false;
                int num1016 = 0;
                if (projectile.ai[1] == 0f)
                {
                    for (int num1017 = 0; num1017 < 200; num1017 = num3 + 1)
                    {
                        if (Main.npc[num1017].CanBeChasedBy(projectile, false))
                        {
                            Vector2 center13 = Main.npc[num1017].Center;
                            if (projectile.Distance(center13) < num1015)
                            {
                                num1015 = projectile.Distance(center13);
                                vector145 = center13;
                                flag59 = true;
                                num1016 = num1017;
                            }
                        }
                        num3 = num1017;
                    }
                    if (flag59)
                    {
                        if (projectile.ai[1] != (float)(num1016 + 1))
                        {
                            projectile.netUpdate = true;
                        }
                        projectile.ai[1] = (float)(num1016 + 1);
                    }
                    flag59 = false;
                }
                if (projectile.ai[1] != 0f)
                {
                    int num1018 = (int)(projectile.ai[1] - 1f);
                    if (Main.npc[num1018].active && Main.npc[num1018].CanBeChasedBy(projectile, true) && projectile.Distance(Main.npc[num1018].Center) < 1000f)
                    {
                        flag59 = true;
                        vector145 = Main.npc[num1018].Center;
                    }
                }
                if (!projectile.friendly)
                {
                    flag59 = false;
                }
                if (flag59)
                {
                    float num1019 = 24f;
                    int num1020 = 10;
                    Vector2 vector146 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                    float num1021 = vector145.X - vector146.X;
                    float num1022 = vector145.Y - vector146.Y;
                    float num1023 = (float)Math.Sqrt((double)(num1021 * num1021 + num1022 * num1022));
                    num1023 = num1019 / num1023;
                    num1021 *= num1023;
                    num1022 *= num1023;
                    projectile.velocity.X = (projectile.velocity.X * (float)(num1020 - 1) + num1021) / (float)num1020;
                    projectile.velocity.Y = (projectile.velocity.Y * (float)(num1020 - 1) + num1022) / (float)num1020;
                }
            }
            if (projectile.ai[0] >= 180f)
            {
                projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 180);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 2);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item10, projectile.position);
            for (int i = 0; i < 10; i++)
            {
                int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, ModContent.DustType<AstralBlue>(), 0f, 0f, 100, default, 1.5f);
                Main.dust[num469].noGravity = true;
                Main.dust[num469].velocity *= 0f;
            }
            for (int i = 0; i < 10; i++)
            {
                int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1.5f);
                Main.dust[num469].noGravity = true;
                Main.dust[num469].velocity *= 0f;
            }
			if (projectile.Calamity().stealthStrike)
			{
                if (projectile.owner == Main.myPlayer)
                {
                    float spread = 60f * 0.0174f;
                    double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 6f;
                    double offsetAngle;
                    int i;
                    for (i = 0; i < 3; i++)
                    {
                        offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 2f), (float)(Math.Cos(offsetAngle) * 2f), ModContent.ProjectileType<RegulusEnergy>(), (int)((double)projectile.damage * 0.4), projectile.knockBack, projectile.owner, 0f, 0f);
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 2f), (float)(-Math.Cos(offsetAngle) * 2f), ModContent.ProjectileType<RegulusEnergy>(), (int)((double)projectile.damage * 0.4), projectile.knockBack, projectile.owner, 0f, 0f);
                    }
                }
			}
        }
    }
}
