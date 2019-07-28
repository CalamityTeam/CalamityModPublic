using System;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class CoralSpike : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Coral Spike");
		}

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.aiStyle = 14;
            projectile.penetrate = 4;
            projectile.timeLeft = 360;
            projectile.magic = true;
        }

        public override void AI()
        {
        	projectile.velocity.X *= 0.9f;
        	projectile.velocity.Y *= 0.99f;
        	projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
        }
    }
}
