using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Enemy
{
    public class CrabBoulder : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Boulder");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 26;
            projectile.hostile = true;
            projectile.timeLeft = 480;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {
            if (projectile.ai[0]++ < 30f)
            {
                projectile.scale = MathHelper.Lerp(0.004f, 1f, projectile.ai[0] / 30f);
            }
            else
            {
                if (projectile.velocity.Y < 12f)
                    projectile.velocity.Y += 0.18f;
            }
            projectile.tileCollide = projectile.ai[0] > 70f;
            projectile.rotation += Math.Sign(projectile.velocity.X) * 0.08f;
        }
        public override void Kill(int timeLeft)
        {
            Utils.PoofOfSmoke(projectile.Center);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
