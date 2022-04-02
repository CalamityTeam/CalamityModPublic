using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class OpalStrike : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Strike");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 20;
            projectile.friendly = true;
            projectile.scale = 0.5f;
            projectile.alpha = 200;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.ranged = true;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            if (projectile.scale <= 1.5f)
                projectile.scale *= 1.01f;
            projectile.alpha -= 2;
            if (projectile.alpha == 0)
                projectile.alpha = 200;
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
