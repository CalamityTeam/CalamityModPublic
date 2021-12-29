using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class MourningSkull : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mourning Skull");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            if (projectile.ai[0] < 0f)
            {
                projectile.alpha = 0;
            }
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 50;
            }
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }

            if (projectile.velocity.X < 0f)
            {
                projectile.spriteDirection = -1;
                projectile.rotation = (float)Math.Atan2((double)-(double)projectile.velocity.Y, (double)-(double)projectile.velocity.X);
            }
            else
            {
                projectile.spriteDirection = 1;
                projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X);
            }

            if (projectile.ai[0] >= 0f && projectile.ai[0] < 200f)
            {
                int num554 = (int)projectile.ai[0];
                if (Main.npc[num554].active)
                {
                    float num555 = 8f;
                    Vector2 vector44 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                    float num556 = Main.npc[num554].position.X - vector44.X;
                    float num557 = Main.npc[num554].position.Y - vector44.Y;
                    float num558 = (float)Math.Sqrt((double)(num556 * num556 + num557 * num557));
                    num558 = num555 / num558;
                    num556 *= num558;
                    num557 *= num558;
                    projectile.velocity.X = (projectile.velocity.X * 14f + num556) / 15f;
                    projectile.velocity.Y = (projectile.velocity.Y * 14f + num557) / 15f;
                }
                else
                {
                    float num559 = 1000f;
                    int num3;
                    for (int num560 = 0; num560 < 200; num560 = num3 + 1)
                    {
                        if (Main.npc[num560].CanBeChasedBy(projectile, false))
                        {
                            float num561 = Main.npc[num560].position.X + (float)(Main.npc[num560].width / 2);
                            float num562 = Main.npc[num560].position.Y + (float)(Main.npc[num560].height / 2);
                            float num563 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num561) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num562);
                            if (num563 < num559 && Collision.CanHit(projectile.position, projectile.width, projectile.height, Main.npc[num560].position, Main.npc[num560].width, Main.npc[num560].height))
                            {
                                num559 = num563;
                                projectile.ai[0] = (float)num560;
                            }
                        }
                        num3 = num560;
                    }
                }

                if (projectile.velocity.X < 0f)
                {
                    projectile.spriteDirection = -1;
                    projectile.rotation = (float)Math.Atan2((double)-(double)projectile.velocity.Y, (double)-(double)projectile.velocity.X);
                }
                else
                {
                    projectile.spriteDirection = 1;
                    projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X);
                }

                int num564 = 8;
                int num565 = Dust.NewDust(new Vector2(projectile.position.X + (float)num564, projectile.position.Y + (float)num564), projectile.width - num564 * 2, projectile.height - num564 * 2, Main.rand.NextBool(2) ? 5 : 6, 0f, 0f, 0, default, 1f);
                Dust dust = Main.dust[num565];
                dust.velocity *= 0.5f;
                dust = Main.dust[num565];
                dust.velocity += projectile.velocity * 0.5f;
                Main.dust[num565].noGravity = true;
                Main.dust[num565].noLight = true;
                Main.dust[num565].scale = 1.4f;
                return;
            }

            projectile.Kill();
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, Main.rand.Next(0, 128));
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item10, projectile.position);
            for (int num621 = 0; num621 < 5; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 5, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 10; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 6, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 6, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 300);
            if (projectile.owner == Main.myPlayer)
            {
                for (int k = 0; k < 2; k++)
                {
                    Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 174, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
                    Projectile.NewProjectile(projectile.position.X, projectile.position.Y, (float)Main.rand.Next(-35, 36) * 0.2f, (float)Main.rand.Next(-35, 36) * 0.2f, ModContent.ProjectileType<TinyFlare>(),
                     (int)((double)projectile.damage * 0.35), projectile.knockBack * 0.35f, Main.myPlayer, 0f, 0f);
                }
            }
        }
    }
}
