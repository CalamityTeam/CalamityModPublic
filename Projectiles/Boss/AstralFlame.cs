using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class AstralFlame : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Homing Flame");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.Calamity().canBreakPlayerDefense = true;
            projectile.width = 50;
            projectile.height = 50;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.alpha = 100;
            projectile.penetrate = 1;
            projectile.timeLeft = 485;
            projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
                projectile.frame = 0;

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

            Lighting.AddLight(projectile.Center, 0.3f, 0.5f, 0.1f);

            int num103 = Player.FindClosest(projectile.Center, 1, 1);
            Vector2 vector11 = Main.player[num103].Center - projectile.Center;
            if (vector11.Length() < 60f)
            {
                projectile.Kill();
                return;
            }

            projectile.ai[0] += 1f;
            if (projectile.ai[0] > 15f)
            {
                int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 0.8f);
                Main.dust[num469].noGravity = true;
                Main.dust[num469].velocity *= 0f;
            }

            if (projectile.ai[0] >= 120f)
            {
                if (projectile.ai[1] < 120f)
                {
                    if (projectile.ai[1] < 15f)
                        projectile.velocity *= 1.02f;

                    float scaleFactor2 = projectile.velocity.Length();
                    vector11.Normalize();
                    vector11 *= scaleFactor2;
                    projectile.velocity = (projectile.velocity * 15f + vector11) / 16f;
                    projectile.velocity.Normalize();
                    projectile.velocity *= scaleFactor2;
                }
                else if (projectile.velocity.Length() < 18f)
                    projectile.velocity *= 1.01f;

                projectile.ai[1] += 1f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, projectile.alpha);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 180);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Zombie, (int)projectile.position.X, (int)projectile.position.Y, 103, 1f, 0f);
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 96;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            for (int num193 = 0; num193 < 2; num193++)
            {
                Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 50, default, 1f);
            }
            for (int num194 = 0; num194 < 20; num194++)
            {
                int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 0, default, 1.5f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 3f;
                num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 173, 0f, 0f, 50, default, 1f);
                Main.dust[num195].velocity *= 2f;
                Main.dust[num195].noGravity = true;
            }
            projectile.Damage();
        }
    }
}
