using CalamityMod.Buffs.DamageOverTime;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles.Melee
{
	public class BloodExplosion : ModProjectile
	{
		public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blood");
		}

		public override void SetDefaults()
		{
			projectile.width = 100;
			projectile.height = 100;
			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.penetrate = -1;
			projectile.timeLeft = 5;
			projectile.melee = true;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = -1;
		}

		public override void AI()
		{
			if (projectile.ai[1] > 0f)
				return;

			Main.PlaySound(SoundID.Item14, projectile.position);
			for (int d = 0; d < 4; d++)
			{
				int index = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Blood, 0f, 0f, 100, default, 1.5f);
				Main.dust[index].velocity *= 3f;
				if (Main.rand.NextBool(2))
				{
					Main.dust[index].scale = 0.5f;
					Main.dust[index].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
				}
			}
			for (int d = 0; d < 6; d++)
			{
				int index = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Blood, 0f, 0f, 100, default, 2f);
				Main.dust[index].noGravity = true;
				Main.dust[index].velocity *= 5f;
				index = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Blood, 0f, 0f, 100, default, 1.5f);
				Main.dust[index].velocity *= 2f;
			}
			projectile.ai[1]++;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(ModContent.BuffType<BurningBlood>(), 60);
		}
	}
}
