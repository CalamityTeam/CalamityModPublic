using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class FlareExplosionHuge : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Flare Explosion");
		}

		public override void SetDefaults()
		{
			projectile.width = 400;
			projectile.height = 400;
			projectile.hostile = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.penetrate = -1;
			projectile.timeLeft = 60;
			cooldownSlot = 1;
		}

		public override void AI()
		{
			Lighting.AddLight(projectile.Center, 3f, 3f, 0f);
			for (int num468 = 0; num468 < 3; num468++)
			{
				int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, default(Color), 0.35f);
				Main.dust[num469].noGravity = true;
				Main.dust[num469].velocity *= 0f;
			}
		}

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			target.AddBuff(mod.BuffType("HolyLight"), 60);
		}
	}
}
