using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class TerraBulletSplit : ModProjectile
    {
        private float speed = 0f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bullet");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.alpha = 255;
            projectile.timeLeft = 120;
            projectile.extraUpdates = 1;
            aiType = ProjectileID.Bullet;
        }

        public override bool? CanHitNPC(NPC target) => projectile.timeLeft < 90 && target.CanBeChasedBy(projectile);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesFromEdge(projectile, 0, lightColor);
            return false;
        }

        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 85;
            }
            float x2 = projectile.position.X - projectile.velocity.X / 10f;
            float y2 = projectile.position.Y - projectile.velocity.Y / 10f;
            int num137 = Dust.NewDust(new Vector2(x2, y2), 1, 1, 74, 0f, 0f, 0, default, 0.8f);
            Main.dust[num137].alpha = projectile.alpha;
            Main.dust[num137].position.X = x2;
            Main.dust[num137].position.Y = y2;
            Main.dust[num137].velocity *= 0f;
            Main.dust[num137].noGravity = true;

            if (speed == 0f)
                speed = projectile.velocity.Length();

            if (projectile.timeLeft < 90)
                CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 450f, speed, 12f);
        }
    }
}
