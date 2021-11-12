using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class MidnightSunShot : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Midnight Shot");
            Main.projFrames[projectile.type] = 1;
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.alpha = 255;
            projectile.MaxUpdates = 3;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.aiStyle = 1;
            aiType = ProjectileID.BulletHighVelocity;
            projectile.penetrate = 1;
            projectile.timeLeft = 120;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 2);
            return false;
        }
    }
}
