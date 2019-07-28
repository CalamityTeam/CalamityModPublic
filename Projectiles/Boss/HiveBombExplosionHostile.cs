using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class HiveBombExplosionHostile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hive Bomb Explosion");
		}

		public override void SetDefaults()
		{
			projectile.width = 100;
			projectile.height = 100;
			projectile.hostile = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.penetrate = -1;
			projectile.timeLeft = 5;
		}

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			target.AddBuff(mod.BuffType("Plague"), 500);
		}
	}
}
