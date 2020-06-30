using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class VeriumBulletProj : ModProjectile
    {
		private float speed = 0f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bullet");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 1;
            aiType = ProjectileID.Bullet;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override bool PreAI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            projectile.spriteDirection = projectile.direction;
            if (Main.rand.NextBool(2))
            {
                int purple = Dust.NewDust(projectile.position, 1, 1, 70, 0f, 0f, 0, default, 0.5f);
                Main.dust[purple].alpha = projectile.alpha;
                Main.dust[purple].velocity *= 0f;
                Main.dust[purple].noGravity = true;
            }

			if (speed == 0f)
				speed = projectile.velocity.Length();
			CalamityGlobalProjectile.HomeInOnNPC(projectile, false, 300f, speed, 12f);
            return false;
        }
    }
}
