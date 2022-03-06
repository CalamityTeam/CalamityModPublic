using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
	public class TrackingDiskLaser : ModProjectile
	{
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

		public float Time
		{
			get => projectile.localAI[0];
			set => projectile.localAI[0] = value;
		}
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Laser");
		}

		public override void SetDefaults()
		{
			projectile.width = 2;
			projectile.height = 2;
			projectile.friendly = true;
			projectile.Calamity().rogue = true;
			projectile.tileCollide = false;
			projectile.penetrate = 1;
			projectile.extraUpdates = 100;
			projectile.timeLeft = 600;
		}

		public override void AI()
		{
			Lighting.AddLight(projectile.Center, 0.2f, 0.1f, 0f);

			Time++;
			if (Time >= 10f)
			{
				for (int i = 0; i < 2; i++)
				{
					Dust dust = Dust.NewDustDirect(projectile.Center, 0, 0, 182, 0f, 0f, 160, default, 2f);
					dust.position = projectile.Center;
					dust.velocity = projectile.velocity;
					dust.scale = projectile.scale;
					dust.noGravity = true;
				}
			}
		}

		public override void Kill(int timeLeft)
		{
			CalamityGlobalProjectile.ExpandHitboxBy(projectile, 60);
			projectile.maxPenetrate = -1;
			projectile.penetrate = -1;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 10;
			projectile.Damage();
		}
	}
}
