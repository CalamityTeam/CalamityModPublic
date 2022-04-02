using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class HeavenfallenStardiskBoomerang : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/HeavenfallenStardisk";

        private bool explode = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heavenfallen Stardisk");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 34;
            projectile.alpha = 255;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.netImportant = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            if (projectile.Calamity().stealthStrike)
            {
                if (projectile.timeLeft % 5f == 0f) //every 5 ticks
                {
                    if (Main.rand.NextBool(2))
                    {
                        int energyAmt = Main.rand.Next(1, 4); //1 to 3 energy
                        for (int n = 0; n < energyAmt; n++)
                        {
                            CalamityUtils.ProjectileRain(projectile.Center, 400f, 100f, 500f, 800f, 29f, ModContent.ProjectileType<HeavenfallenEnergy>(), (int)(projectile.damage * 0.4), projectile.knockBack * 0.4f, projectile.owner);
                        }
                    }
                }
            }

            if (projectile.alpha > 0)
            {
                projectile.alpha -= 20;
            }
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }

            for (int i = 0; i < 2; i++)
            {
                int blueDust = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, ModContent.DustType<AstralBlue>(), 0f, 0f, 100, default, 1f);
                Main.dust[blueDust].noGravity = true;
                Main.dust[blueDust].velocity *= 0f;
            }
            for (int i = 0; i < 2; i++)
            {
                int orangeDust = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1f);
                Main.dust[orangeDust].noGravity = true;
                Main.dust[orangeDust].velocity *= 0f;
            }

            projectile.rotation += 0.5f;

            if (player.position.Y != player.oldPosition.Y && projectile.ai[0] == 0f)
            {
                explode = true;
            }

            projectile.ai[0] += 1f;

            if (Main.myPlayer == projectile.owner && projectile.ai[0] == 20f)
            {
                if (player.channel)
                {
                    float num115 = 20f;
                    float num116 = (float)Main.mouseX + Main.screenPosition.X - projectile.Center.X;
                    float num117 = (float)Main.mouseY + Main.screenPosition.Y - projectile.Center.Y;
                    if (player.gravDir == -1f)
                    {
                        num117 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - projectile.Center.Y;
                    }
                    float num118 = (float)Math.Sqrt((double)(num116 * num116 + num117 * num117));
                    if (num118 > num115)
                    {
                        num118 = num115 / num118;
                        num116 *= num118;
                        num117 *= num118;
                        int num119 = (int)(num116 * 1000f);
                        int num120 = (int)(projectile.velocity.X * 1000f);
                        int num121 = (int)(num117 * 1000f);
                        int num122 = (int)(projectile.velocity.Y * 1000f);
                        if (num119 != num120 || num121 != num122)
                        {
                            projectile.netUpdate = true;
                        }
                        projectile.velocity.X = num116;
                        projectile.velocity.Y = num117;
                    }
                    else
                    {
                        int num123 = (int)(num116 * 1000f);
                        int num124 = (int)(projectile.velocity.X * 1000f);
                        int num125 = (int)(num117 * 1000f);
                        int num126 = (int)(projectile.velocity.Y * 1000f);
                        if (num123 != num124 || num125 != num126)
                        {
                            projectile.netUpdate = true;
                        }
                        projectile.velocity.X = num116;
                        projectile.velocity.Y = num117;
                    }
                }
                else if (projectile.ai[0] == 20f)
                {
                    projectile.netUpdate = true;
                    float num127 = 20f;
                    Vector2 vector11 = projectile.Center;
                    float num128 = (float)Main.mouseX + Main.screenPosition.X - vector11.X;
                    float num129 = (float)Main.mouseY + Main.screenPosition.Y - vector11.Y;
                    if (player.gravDir == -1f)
                    {
                        num129 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector11.Y;
                    }
                    float num130 = (float)Math.Sqrt((double)(num128 * num128 + num129 * num129));
                    if (num130 == 0f || projectile.ai[0] < 0f)
                    {
                        vector11 = player.Center;
                        num128 = projectile.Center.X - vector11.X;
                        num129 = projectile.Center.Y - vector11.Y;
                        num130 = (float)Math.Sqrt((double)(num128 * num128 + num129 * num129));
                    }
                    num130 = num127 / num130;
                    num128 *= num130;
                    num129 *= num130;
                    projectile.velocity.X = num128;
                    projectile.velocity.Y = num129;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 240);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 240);
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[projectile.owner];

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

            if (explode && player.position.Y != player.oldPosition.Y)
            {
                if (projectile.owner == Main.myPlayer)
                {
                    float spread = 45f * 0.0174f;
                    double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 8f;
                    double offsetAngle;
                    for (int i = 0; i < 4; i++)
                    {
                        offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 2f), (float)(Math.Cos(offsetAngle) * 2f), ModContent.ProjectileType<HeavenfallenEnergy>(), (int)(projectile.damage * 0.4), projectile.knockBack * 0.4f, projectile.owner, 0f, 0f);
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 2f), (float)(-Math.Cos(offsetAngle) * 2f), ModContent.ProjectileType<HeavenfallenEnergy>(), (int)(projectile.damage * 0.4), projectile.knockBack * 0.4f, projectile.owner, 0f, 0f);
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 2);
            return false;
        }
    }
}
