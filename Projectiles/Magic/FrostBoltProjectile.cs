using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class FrostBoltProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ball");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 2;
            projectile.timeLeft = 480;
            projectile.magic = true;
            projectile.coldDamage = true;
        }

        public override void AI()
        {
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 25;
                if (projectile.alpha < 0)
                    projectile.alpha = 0;
            }
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.01f / 255f, (255 - projectile.alpha) * 0.3f / 255f, (255 - projectile.alpha) * 0.45f / 255f);
            for (int num105 = 0; num105 < 2; num105++)
            {
                float num99 = projectile.velocity.X / 3f * (float)num105;
                float num100 = projectile.velocity.Y / 3f * (float)num105;
                int num101 = 4;
                int frostDust = Dust.NewDust(new Vector2(projectile.position.X + (float)num101, projectile.position.Y + (float)num101), projectile.width - num101 * 2, projectile.height - num101 * 2, 92, 0f, 0f, 100, default, 1.2f);
                Dust dust = Main.dust[frostDust];
                dust.noGravity = true;
                dust.velocity *= 0.1f;
                dust.velocity += projectile.velocity * 0.1f;
                dust.position.X -= num99;
                dust.position.Y -= num100;
            }
            if (Main.rand.NextBool(10))
            {
                int num103 = 4;
                int frostDustSmol = Dust.NewDust(new Vector2(projectile.position.X + (float)num103, projectile.position.Y + (float)num103), projectile.width - num103 * 2, projectile.height - num103 * 2, 92, 0f, 0f, 100, default, 0.6f);
                Main.dust[frostDustSmol].velocity *= 0.25f;
                Main.dust[frostDustSmol].velocity += projectile.velocity * 0.5f;
            }
            projectile.rotation += 0.3f * (float)projectile.direction;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.penetrate--;
            if (projectile.penetrate <= 0)
            {
                projectile.Kill();
            }
            else
            {
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
                Main.PlaySound(SoundID.Item10, projectile.position);
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item27, projectile.position);
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 92, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 60);
        }
    }
}
