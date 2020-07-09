using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class BlackHawkBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bullet");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.light = 0.5f;
            projectile.alpha = 255;
            projectile.extraUpdates = 4;
            projectile.scale = 1.18f;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.ignoreWater = true;
            projectile.aiStyle = 1;
            aiType = ProjectileID.BulletHighVelocity;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
			projectile.tileCollide = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 2);
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, 200);
    }
}
