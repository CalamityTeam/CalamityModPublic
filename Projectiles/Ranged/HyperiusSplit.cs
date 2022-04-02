using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class HyperiusSplit : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Ranged/HyperiusBulletProj";
        private Color currentColor = Color.Black;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hyperius Bad Time");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            // Intentionally large bullet hitbox to make Hyperius swarm more forgiving with hits
            projectile.width = 8;
            projectile.height = 8;
            projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.tileCollide = false;
            projectile.penetrate = 1;
            projectile.timeLeft = 360;
            projectile.extraUpdates = 1;
            aiType = ProjectileID.Bullet;
        }

        public override void AI()
        {
            if (currentColor == Color.Black)
            {
                int startPoint = Main.rand.Next(6);
                projectile.localAI[0] = startPoint;
                currentColor = HyperiusBulletProj.GetStartingColor(startPoint);
            }
            HyperiusBulletProj.Visuals(projectile, ref currentColor);
        }

        // This projectile is always fullbright.
        public override Color? GetAlpha(Color lightColor)
        {
            return currentColor;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.timeLeft == 360)
                return false;
            CalamityUtils.DrawAfterimagesFromEdge(projectile, 0, lightColor);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            const int killDust = 3;
            int[] dustTypes = new int[] { 60, 61, 59 };
            for (int i = 0; i < killDust; ++i)
            {
                int dustType = dustTypes[Main.rand.Next(3)];
                float scale = Main.rand.NextFloat(0.4f, 0.9f);
                float velScale = Main.rand.NextFloat(3f, 5.5f);
                int dustID = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType);
                Main.dust[dustID].noGravity = true;
                Main.dust[dustID].scale = scale;
                Main.dust[dustID].velocity *= velScale;
            }
        }
    }
}
