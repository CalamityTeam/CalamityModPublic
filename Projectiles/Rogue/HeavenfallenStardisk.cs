using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Dusts;
using Terraria.ID;
using CalamityMod.Buffs;
namespace CalamityMod.Projectiles.Rogue
{
    public class HeavenfallenStardisk : ModProjectile
    {
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
                int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, ModContent.DustType<AstralBlue>(), 0f, 0f, 100, default, 1f);
                Main.dust[num469].noGravity = true;
                Main.dust[num469].velocity *= 0f;
            }
            for (int i = 0; i < 2; i++)
            {
                int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1f);
                Main.dust[num469].noGravity = true;
                Main.dust[num469].velocity *= 0f;
            }

            projectile.rotation += 0.5f;

            if (Main.player[projectile.owner].position.Y != Main.player[projectile.owner].oldPosition.Y && projectile.ai[0] == 0f)
            {
                explode = true;
            }

            projectile.ai[0] += 1f;

            if (Main.myPlayer == projectile.owner && projectile.ai[0] == 20f)
            {
                if (Main.player[projectile.owner].channel)
                {
                    float num115 = 20f;
                    Vector2 vector10 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                    float num116 = (float)Main.mouseX + Main.screenPosition.X - vector10.X;
                    float num117 = (float)Main.mouseY + Main.screenPosition.Y - vector10.Y;
                    if (Main.player[projectile.owner].gravDir == -1f)
                    {
                        num117 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector10.Y;
                    }
                    float num118 = (float)Math.Sqrt((double)(num116 * num116 + num117 * num117));
                    num118 = (float)Math.Sqrt((double)(num116 * num116 + num117 * num117));
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
                    Vector2 vector11 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                    float num128 = (float)Main.mouseX + Main.screenPosition.X - vector11.X;
                    float num129 = (float)Main.mouseY + Main.screenPosition.Y - vector11.Y;
                    if (Main.player[projectile.owner].gravDir == -1f)
                    {
                        num129 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector11.Y;
                    }
                    float num130 = (float)Math.Sqrt((double)(num128 * num128 + num129 * num129));
                    if (num130 == 0f || projectile.ai[0] < 0f)
                    {
                        vector11 = new Vector2(Main.player[projectile.owner].position.X + (float)(Main.player[projectile.owner].width / 2), Main.player[projectile.owner].position.Y + (float)(Main.player[projectile.owner].height / 2));
                        num128 = projectile.position.X + (float)projectile.width * 0.5f - vector11.X;
                        num129 = projectile.position.Y + (float)projectile.height * 0.5f - vector11.Y;
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
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 180);
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

            if (explode && Main.player[projectile.owner].position.Y != Main.player[projectile.owner].oldPosition.Y)
            {
                if (projectile.owner == Main.myPlayer)
                {
                    float spread = 45f * 0.0174f;
                    double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 8f;
                    double offsetAngle;
                    int i;
                    for (i = 0; i < 4; i++)
                    {
                        offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 2f), (float)(Math.Cos(offsetAngle) * 2f), ModContent.ProjectileType<HeavenfallenEnergy>(), (int)((double)projectile.damage * 0.4), projectile.knockBack, projectile.owner, 0f, 0f);
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 2f), (float)(-Math.Cos(offsetAngle) * 2f), ModContent.ProjectileType<HeavenfallenEnergy>(), (int)((double)projectile.damage * 0.4), projectile.knockBack, projectile.owner, 0f, 0f);
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 2);
            return false;
        }
    }
}
