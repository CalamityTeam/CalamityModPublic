using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class VileClot : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Vile Clot");
		}

		public override void SetDefaults()
		{
			projectile.width = 6;
			projectile.height = 6;
			projectile.hostile = true;
			projectile.penetrate = 1;
			projectile.timeLeft = 300;
		}

		public override void AI()
		{
			projectile.ai[0] += 1f;
			if (projectile.ai[0] > 3f)
			{
				int num104 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y),
					projectile.width, projectile.height, 75, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f, 100, default(Color), 1f);
				Main.dust[num104].noGravity = true;
			}
		}

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			target.AddBuff(BuffID.CursedInferno, 60);
		}
	}
}
