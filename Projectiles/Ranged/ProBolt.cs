using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class ProBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bolt");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 3;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 1;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            //Rotation
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi) + MathHelper.ToRadians(90) *projectile.direction;

            Lighting.AddLight(projectile.Center, new Vector3(158, 240, 240) * (1.5f / 255));

            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f)
            {
                Vector2 dspeed = -projectile.velocity * 0.8f;
                float x = projectile.Center.X - projectile.velocity.X / 10f;
                float y = projectile.Center.Y - projectile.velocity.Y / 10f;
                int num135 = Dust.NewDust(new Vector2(x, y), 1, 1, 160, 0f, 0f, 0, default, 1.25f);
                Main.dust[num135].alpha = projectile.alpha;
                Main.dust[num135].position.X = x;
                Main.dust[num135].position.Y = y;
                Main.dust[num135].velocity = dspeed;
                Main.dust[num135].noGravity = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesFromEdge(projectile, 0, lightColor);
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
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 10; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 160, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f); //206 160 226
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 7;
        }
    }
}
