using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class Light : ModProjectile
    {
        private const int speedTimerMax = 60;
        private int speedTimer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Light");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            projectile.rotation += 0.5f;
            speedTimer--;
            if (speedTimer <= 0)
            {
                speedTimer = speedTimerMax;
                projectile.velocity *= -1f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 2);
            return false;
        }
    }
}
