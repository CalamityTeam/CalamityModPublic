using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class AortaYoyo : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aorta");
            ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type] = 11f;
            ProjectileID.Sets.YoyosMaximumRange[projectile.type] = 260f;
            ProjectileID.Sets.YoyosTopSpeed[projectile.type] = 8.5f;

            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.aiStyle = 99;
            projectile.width = 16;
            projectile.height = 16;
            projectile.scale = 1f;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = -1;
            projectile.MaxUpdates = 2;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            CalamityGlobalProjectile.MagnetSphereHitscan(projectile, 240f, 6f, 180f, 3, ModContent.ProjectileType<Blood2>(), 0.25);
            if ((projectile.position - Main.player[projectile.owner].position).Length() > 3200f) //200 blocks
                projectile.Kill();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
