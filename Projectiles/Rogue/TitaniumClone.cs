using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class TitaniumClone : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/TitaniumShuriken";

        private static float RotationIncrement = 0.22f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shuriken");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 28;
            projectile.alpha = 150;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 200;
            projectile.Calamity().rogue = true;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            projectile.rotation += RotationIncrement;
            projectile.ai[0] += 1f;
            if (projectile.ai[0] > 30f)
            {
                CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 200f, 12f, 20f);
            }
        }

        public override bool CanDamage() => projectile.ai[0] >= 30f;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
