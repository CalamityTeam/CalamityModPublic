using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class GoliathExplosion : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Explosion");
		}

        public override void SetDefaults()
        {
            projectile.width = 160;
            projectile.height = 160;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
			projectile.ranged = true;
			projectile.penetrate = -1;
            projectile.timeLeft = 5;
        }
    }
}
