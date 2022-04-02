using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles.Rogue
{
	public class CraniumSMASH : ModProjectile
	{
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cranium SMASH");
		}

		public override void SetDefaults()
		{
			projectile.width = 192;
			projectile.height = 192;
			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.penetrate = -1;
			projectile.timeLeft = 10;
			projectile.Calamity().rogue = true;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = -2;
		}

		public override void AI()
		{
			if (projectile.ai[0] == 0f)
				SpawnExplosionDust();
			if (projectile.ai[0] <= 1f)
				projectile.ai[0]++;
		}

		void SpawnExplosionDust()
		{
			Main.PlaySound(SoundID.Item, (int)projectile.Center.X, (int)projectile.Center.Y, 14);
			CalamityUtils.ExplosionGores(projectile.Center, 3);
			for (int num194 = 0; num194 < 25; num194++)
			{
				int num195 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 135, 0f, 0f, 100, default, 2f);
				Main.dust[num195].noGravity = true;
				Main.dust[num195].velocity *= 0f;
			}
		}
	}
}
