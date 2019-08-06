using CalamityMod.Items.Weapons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class VenusianBolt : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bolt");
		}

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 600;
            projectile.magic = true;
        }

        public override void AI()
        {
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.25f) / 255f, ((255 - projectile.alpha) * 0.2f) / 255f, ((255 - projectile.alpha) * 0.01f) / 255f);
        	if (projectile.wet && !projectile.lavaWet)
        	{
        		projectile.Kill();
        	}
			if (projectile.localAI[0] == 0f)
			{
				Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 73);
				projectile.localAI[0] += 1f;
			}
			for (int num457 = 0; num457 < 5; num457++)
			{
				int num458 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 55, 0f, 0f, 100, default(Color), 1.2f);
				Main.dust[num458].noGravity = true;
				Main.dust[num458].velocity *= 0.5f;
				Main.dust[num458].velocity += projectile.velocity * 0.1f;
			}
        }

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(BuffID.OnFire, 600);
		}

		public override void Kill(int timeLeft)
        {
        	if (projectile.owner == Main.myPlayer)
        	{
                int explosionDamage = VenusianTrident.BaseDamage;
                float explosionKB = 6f;
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("VenusianExplosion"), explosionDamage, explosionKB, projectile.owner, 0f, 0f);

                int cinderDamage = (int)(VenusianTrident.BaseDamage * 0.75f);
                float cinderKB = 0f;
                Vector2 cinderPos = projectile.oldPosition + 0.5f * projectile.Size;
                int numCinders = Main.rand.Next(7, 10);
				for (int i = 0; i < numCinders; i++)
				{
					Vector2 cinderVel = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
					while (cinderVel.X == 0f && cinderVel.Y == 0f)
					{
						cinderVel = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
					}
					cinderVel.Normalize();
					cinderVel *= (float)Main.rand.Next(70, 101) * 0.1f;
					Projectile.NewProjectile(cinderPos, cinderVel, mod.ProjectileType("VenusianFlame"), cinderDamage, cinderKB, projectile.owner, 0f, 0f);
				}
        	}
        }
    }
}
