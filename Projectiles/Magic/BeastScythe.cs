using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class BeastScythe : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scythe");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 50;
            projectile.height = 54;
            projectile.alpha = 255;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.netImportant = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.magic = true;
            projectile.extraUpdates = 1;
            projectile.tileCollide = false;
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

            projectile.rotation += 0.5f;

            Lighting.AddLight(projectile.Center, 0.35f, 0f, 0.35f);
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 173, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f);
            }

            projectile.ai[0] += 1f;
            if (projectile.ai[0] <= 30f)
            {
                projectile.velocity *= 0.999f;
            }
            if (Main.myPlayer == projectile.owner && projectile.ai[0] == 30f)
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
                else
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
            if (projectile.ai[0] >= 30f)
            {
                projectile.velocity *= 1.001f;

                CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 200f, 12f, 20f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 180);
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.ShadowFlame, 90);
            }
            else if (Main.rand.NextBool(2))
            {
                target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
            }
            else
            {
                target.AddBuff(BuffID.Frostburn, 150);
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 14);
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 100;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.damage /= 2;
            projectile.Damage();
            bool flag = WorldGen.SolidTile(Framing.GetTileSafely((int)projectile.position.X / 16, (int)projectile.position.Y / 16));
            for (int m = 0; m < 4; m++)
            {
                Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 173, 0f, 0f, 100, default, 1.5f);
            }
            for (int n = 0; n < 4; n++)
            {
                int num10 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 173, 0f, 0f, 0, default, 2.5f);
                Main.dust[num10].noGravity = true;
                Main.dust[num10].velocity *= 3f;
                if (flag)
                {
                    Main.dust[num10].noLight = true;
                }
                num10 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 173, 0f, 0f, 100, default, 1.5f);
                Main.dust[num10].velocity *= 2f;
                Main.dust[num10].noGravity = true;
                if (flag)
                {
                    Main.dust[num10].noLight = true;
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
