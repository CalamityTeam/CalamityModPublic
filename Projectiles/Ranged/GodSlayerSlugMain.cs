using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Projectiles.Ranged
{
    public class GodSlayerSlugMain : ModProjectile
    {
        private const int Lifetime = 600;
        private const int NoDrawFrames = 2;
        
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("God Slayer Slug");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.ignoreWater = true;
            projectile.aiStyle = 1;
            aiType = 242;
            projectile.alpha = 255;
            projectile.extraUpdates = 5;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
            projectile.timeLeft = Lifetime;
        }

        public override void AI()
        {
            if (projectile.timeLeft < Lifetime - NoDrawFrames)
                projectile.alpha = 100;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 140);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if(projectile.timeLeft < Lifetime - NoDrawFrames)
                CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 0, drawCentered: false);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(0, (int)projectile.Center.X, (int)projectile.Center.Y, 1, 1f, 0f);
            return true;
        }
    }
}
