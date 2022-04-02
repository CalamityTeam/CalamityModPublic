using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class JewelProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ruby Bolt");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

        public override void AI()
        {
            projectile.rotation += 0.3f * (float)projectile.direction;
            for (int index = 0; index < 2; ++index)
            {
                int ruby = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 90, projectile.velocity.X, projectile.velocity.Y, 90, new Color(), 1.2f);
                Dust dust = Main.dust[ruby];
                dust.noGravity = true;
                dust.velocity *= 0.3f;
            }
        }

        public override Color? GetAlpha(Color drawColor) => new Color(255, 50, 50, 0);

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Dig, (int)projectile.position.X, (int)projectile.position.Y, 1, 1f, 0f);
            for (int index1 = 0; index1 < 15; ++index1)
            {
                int ruby = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 90, projectile.oldVelocity.X, projectile.oldVelocity.Y, 50, new Color(), 1.2f);
                Dust dust = Main.dust[ruby];
                dust.noGravity = true;
                dust.scale *= 1.25f;
                dust.velocity *= 0.5f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
