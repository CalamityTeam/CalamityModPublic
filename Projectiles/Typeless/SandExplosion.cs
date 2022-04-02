using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class SandExplosion : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sand");
        }

        public override void SetDefaults()
        {
            projectile.width = 64;
            projectile.height = 64;
            projectile.timeLeft = 1;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.maxPenetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            for (int i = 0; i <= 360; i += 6)
            {
                Vector2 dustspeed = new Vector2(5f, 5f).RotatedBy(MathHelper.ToRadians(i));
                float size = Main.rand.NextFloat(1.1f, 1.6f);
                int d = Dust.NewDust(projectile.Center, projectile.width, projectile.height, 85, dustspeed.X, dustspeed.Y, 0, default, size);
                Main.dust[d].noGravity = true;
                Main.dust[d].position = projectile.Center;
            }
        }
    }
}
