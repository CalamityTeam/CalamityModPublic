using System;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Typeless
{
    public class CursorProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aestheticus Crystal");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 1;
            projectile.alpha = 255;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            if (projectile.alpha > 0)
                projectile.alpha -= 15;

            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;

            if (projectile.timeLeft > 285)
                return;

            float num29 = 5f;
            float num30 = 300f;
            float scaleFactor = 6f;
            Vector2 value7 = new Vector2(10f, 20f);
            //float num31 = 1f;
            int num32 = 3 * projectile.MaxUpdates;
            int num33 = Utils.SelectRandom(Main.rand, new int[]
            {
                246,
                242,
                229,
                226,
                247
            });
            int num34 = 255;
            if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;
                projectile.localAI[0] = (float)-(float)Main.rand.Next(48);
            }
            else if (projectile.ai[1] == 1f && projectile.owner == Main.myPlayer)
            {
                int num35 = -1;
                float num36 = num30;
                for (int num37 = 0; num37 < 200; num37++)
                {
                    if (Main.npc[num37].active && Main.npc[num37].CanBeChasedBy(projectile, false))
                    {
                        Vector2 center3 = Main.npc[num37].Center;
                        float num38 = Vector2.Distance(center3, projectile.Center);
                        if (num38 < num36 && num35 == -1 && Collision.CanHitLine(projectile.Center, 1, 1, center3, 1, 1))
                        {
                            num36 = num38;
                            num35 = num37;
                        }
                    }
                }
                if (num36 < 4f)
                {
                    projectile.Kill();
                    return;
                }
                if (num35 != -1)
                {
                    projectile.ai[1] = num29 + 1f;
                    projectile.ai[0] = (float)num35;
                    projectile.netUpdate = true;
                }
            }
            else if (projectile.ai[1] > num29)
            {
                projectile.ai[1] += 1f;
                int num39 = (int)projectile.ai[0];
                if (!Main.npc[num39].active || !Main.npc[num39].CanBeChasedBy(projectile, false))
                {
                    projectile.ai[1] = 1f;
                    projectile.ai[0] = 0f;
                    projectile.netUpdate = true;
                }
                else
                {
                    projectile.velocity.ToRotation();
                    Vector2 vector6 = Main.npc[num39].Center - projectile.Center;
                    if (vector6.Length() < 10f)
                    {
                        projectile.Kill();
                        return;
                    }
                    if (vector6 != Vector2.Zero)
                    {
                        vector6.Normalize();
                        vector6 *= scaleFactor;
                    }
                    float num40 = 30f;
                    projectile.velocity = (projectile.velocity * (num40 - 1f) + vector6) / num40;
                }
            }
            if (projectile.ai[1] >= 1f && projectile.ai[1] < num29)
            {
                projectile.ai[1] += 1f;
                if (projectile.ai[1] == num29)
                {
                    projectile.ai[1] = 1f;
                }
            }
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] == 48f)
            {
                projectile.localAI[0] = 0f;
            }
            if (Main.rand.NextBool(12))
            {
                Vector2 value9 = -Vector2.UnitX.RotatedByRandom(0.19634954631328583).RotatedBy((double)projectile.velocity.ToRotation(), default);
                int num44 = Dust.NewDust(projectile.position, projectile.width, projectile.height, num34, 0f, 0f, 160, default, 1f);
                Main.dust[num44].velocity *= 0.1f;
                Main.dust[num44].position = projectile.Center + value9 * (float)projectile.width / 2f + projectile.velocity * 2f;
                Main.dust[num44].fadeIn = 0.9f;
            }
            if (Main.rand.NextBool(18))
            {
                Vector2 value10 = -Vector2.UnitX.RotatedByRandom(0.39269909262657166).RotatedBy((double)projectile.velocity.ToRotation(), default);
                int num46 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 107, 0f, 0f, 155, default, 0.8f);
                Main.dust[num46].velocity *= 0.3f;
                Main.dust[num46].position = projectile.Center + value10 * (float)projectile.width / 2f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num46].fadeIn = 1.4f;
                }
            }
            if (Main.rand.NextBool(8))
            {
                Vector2 value11 = -Vector2.UnitX.RotatedByRandom(0.78539818525314331).RotatedBy((double)projectile.velocity.ToRotation(), default);
                int num48 = Dust.NewDust(projectile.position, projectile.width, projectile.height, num33, 0f, 0f, 0, default, 1f);
                Main.dust[num48].velocity *= 0.3f;
                Main.dust[num48].noGravity = true;
                Main.dust[num48].position = projectile.Center + value11 * (float)projectile.width / 2f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num48].fadeIn = 1.4f;
                }
            }
            if (Main.rand.NextBool(6))
            {
                Vector2 value13 = -Vector2.UnitX.RotatedByRandom(0.19634954631328583).RotatedBy((double)projectile.velocity.ToRotation(), default);
                int num50 = Dust.NewDust(projectile.position, projectile.width, projectile.height, num34, 0f, 0f, 100, default, 1f);
                Main.dust[num50].velocity *= 0.3f;
                Main.dust[num50].position = projectile.Center + value13 * (float)projectile.width / 2f;
                Main.dust[num50].fadeIn = 1.2f;
                Main.dust[num50].scale = 1.5f;
                Main.dust[num50].noGravity = true;
            }
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.2f / 255f, (255 - projectile.alpha) * 0.2f / 255f, (255 - projectile.alpha) * 0.2f / 255f);
            int num154 = 14;
            int num155 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width - num154 * 2, projectile.height - num154 * 2, 234, 0f, 0f, 100, default, 0.8f);
            Main.dust[num155].velocity *= 0.1f;
            Main.dust[num155].velocity += projectile.velocity * 0.5f;
            Main.dust[num155].noGravity = true;
            if (Main.rand.NextBool(12))
            {
                int num156 = 16;
                int num157 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width - num156 * 2, projectile.height - num156 * 2, 159, 0f, 0f, 100, default, 1f);
                Main.dust[num157].velocity *= 0.25f;
                Main.dust[num157].velocity += projectile.velocity * 0.5f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 2);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Vaporfied>(), 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Vaporfied>(), 120);
        }

        public override void Kill(int timeLeft)
        {
            Vector2 velocity = projectile.velocity;
            velocity.Normalize();
            velocity *= 4f;

            int spread = 45;
            int numProj = 4;
            float rotation = MathHelper.ToRadians(spread);
            float baseSpeed = (float)Math.Sqrt(velocity.X * velocity.X + velocity.Y * velocity.Y);
            double startAngle = Math.Atan2(velocity.X, velocity.Y) - rotation / 2;
            double deltaAngle = rotation / (float)numProj;
            double offsetAngle;

            for (int i = 0; i < numProj; i++)
            {
                offsetAngle = startAngle + deltaAngle * i;
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, baseSpeed * (float)Math.Sin(offsetAngle), baseSpeed * (float)Math.Cos(offsetAngle),
                    ModContent.ProjectileType<CursorProjSplit>(), projectile.damage / 3, projectile.knockBack * 0.33f, projectile.owner, 0f, 0f);
            }

            Main.PlaySound(SoundID.Item110, projectile.Center);
        }
    }
}
