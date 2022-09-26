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
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.timeLeft = 1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.maxPenetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            for (int i = 0; i <= 360; i += 6)
            {
                Vector2 dustspeed = new Vector2(5f, 5f).RotatedBy(MathHelper.ToRadians(i));
                float size = Main.rand.NextFloat(1.1f, 1.6f);
                int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 85, dustspeed.X, dustspeed.Y, 0, default, size);
                Main.dust[d].noGravity = true;
                Main.dust[d].position = Projectile.Center;
            }
        }
    }
}
