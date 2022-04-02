using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
	public class ExoGladProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

		private float counter = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orb");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Vector2 value7 = new Vector2(5f, 10f);
            counter += 1f;
            if (counter == 48f)
            {
                counter = 0f;
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    int dustType = i == 0 ? 107 : 234;
					if (Main.rand.NextBool(4))
					{
						dustType = 269;
					}
                    Vector2 offset = Vector2.UnitX * -12f;
                    offset = -Vector2.UnitY.RotatedBy((double)(counter * 0.1308997f + (float)i * MathHelper.Pi), default) * value7;
                    int exo = Dust.NewDust(projectile.Center, 0, 0, dustType, 0f, 0f, 160, default, 1.5f);
                    Main.dust[exo].noGravity = true;
                    Main.dust[exo].position = projectile.Center + offset;
                    Main.dust[exo].velocity = projectile.velocity;
                    int dusters = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 0.8f);
                    Main.dust[dusters].noGravity = true;
                    Main.dust[dusters].velocity *= 0f;
                }
            }

			CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 250f, 12f, 20f);
        }

		public override void Kill(int timeLeft)
        {
			int dustType = Utils.SelectRandom(Main.rand, new int[]
			{
				107,
				234,
				269
			});
            for (int k = 0; k < 4; k++)
            {
                int exo = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, dustType, (float)(projectile.direction * 2), 0f, 150, default, 1f);
                Main.dust[exo].noGravity = true;
            }
        }

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			OnHitEffects(target.Center);
			target.ExoDebuffs();
		}

		public override void OnHitPvp(Player target, int damage, bool crit)
		{
			OnHitEffects(target.Center);
			target.ExoDebuffs();
		}

		private void OnHitEffects(Vector2 targetPos)
		{
            float swordKB = projectile.knockBack;
			int swordDmg = (int)(projectile.damage * 0.25);
            int numSwords = Main.rand.Next(1,4);
			int spearAmt = Main.rand.Next(1,4);
			if (projectile.owner == Main.myPlayer)
			{
				for (int i = 0; i < numSwords; ++i)
				{
					CalamityUtils.ProjectileBarrage(projectile.Center, targetPos, Main.rand.NextBool(), 1000f, 1400f, 80f, 900f, Main.rand.NextFloat(24f, 30f), ModContent.ProjectileType<ExoGladiusBeam>(), swordDmg, swordKB, projectile.owner);
				}

				for (int n = 0; n < spearAmt; n++)
				{
					CalamityUtils.ProjectileRain(targetPos, 400f, 100f, -1000f, -800f, 29f, ModContent.ProjectileType<ExoGladSpears>(), swordDmg, swordKB, projectile.owner);
				}
			}
        }
    }
}
