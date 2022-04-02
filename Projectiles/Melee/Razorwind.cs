using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class Razorwind : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/TornadoProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Razorwind");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.alpha = 255;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.timeLeft = 300;
            projectile.extraUpdates = 2;
            projectile.penetrate = 3;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 8;
        }

        public override void AI()
        {
            projectile.rotation += projectile.velocity.X * 0.1f;

            projectile.alpha -= 5;
            if (projectile.alpha < 50)
            {
                projectile.alpha = 50;
            }

            projectile.localAI[1] += 1f;
            if (projectile.localAI[1] > 4f)
            {
                for (int num468 = 0; num468 < 3; num468++)
                {
                    int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 187, 0f, 0f, 100, new Color(53, Main.DiscoG, 255), 2f);
                    Main.dust[num469].noGravity = true;
                    Main.dust[num469].velocity *= 0f;
                }
            }

            CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 250f, 12f, 20f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 10; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 187, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 100, new Color(53, Main.DiscoG, 255));
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(53, Main.DiscoG, 255, projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
