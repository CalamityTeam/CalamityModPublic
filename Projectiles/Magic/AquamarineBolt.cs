using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class AquamarineBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bolt");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.magic = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0f / 255f, (255 - projectile.alpha) * 0.25f / 255f, (255 - projectile.alpha) * 0.25f / 255f);
            int num154 = 14;
            int num155 = Dust.NewDust(projectile.Center, projectile.width - num154 * 2, projectile.height - num154 * 2, 68, 0f, 0f, 100, default, 1.5f);
            Main.dust[num155].noGravity = true;
            Main.dust[num155].velocity *= 0.1f;
            Main.dust[num155].velocity += projectile.velocity * 0.5f;
            if (Main.rand.NextBool(16))
            {
                int num156 = 16;
                int num157 = Dust.NewDust(projectile.Center, projectile.width - num156 * 2, projectile.height - num156 * 2, 68, 0f, 0f, 100, default, 1f);
                Main.dust[num157].velocity *= 0.25f;
                Main.dust[num157].velocity += projectile.velocity * 0.5f;
            }
            projectile.rotation += 0.3f * (float)projectile.direction;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, 200);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
