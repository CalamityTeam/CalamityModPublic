using CalamityMod.Dusts;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class BrimstoneBall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Fireball");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.hostile = true;
            projectile.scale = 1.2f;
            projectile.penetrate = 6;
            projectile.aiStyle = 14;
            aiType = ProjectileID.ThornBall;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            projectile.rotation += 0.12f * projectile.direction;
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.25f / 255f, (255 - projectile.alpha) * 0.05f / 255f, (255 - projectile.alpha) * 0.05f / 255f);
            for (int num468 = 0; num468 < 2; num468++)
            {
                Vector2 dspeed = -projectile.velocity * 0.7f;
                int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, (int)CalamityDusts.Brimstone, 0f, 0f, 150, default, 1.1f);
                Main.dust[num469].noGravity = true;
                Main.dust[num469].velocity = dspeed;
            }

			float num1247 = 0.5f;
			for (int num1248 = 0; num1248 < Main.maxProjectiles; num1248++)
			{
				if (Main.projectile[num1248].active)
				{
					if (num1248 != projectile.whoAmI && Main.projectile[num1248].type == projectile.type)
					{
						if (Vector2.Distance(projectile.Center, Main.projectile[num1248].Center) < 96f)
						{
							if (projectile.position.X < Main.projectile[num1248].position.X)
								projectile.velocity.X -= num1247;
							else
								projectile.velocity.X += num1247;

							if (projectile.position.Y < Main.projectile[num1248].position.Y)
								projectile.velocity.Y -= num1247;
							else
								projectile.velocity.Y += num1247;
						}
					}
				}
			}
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.penetrate--;
            if (projectile.penetrate <= 0)
            {
                projectile.Kill();
            }
            else
            {
                projectile.ai[0] += 0.1f;
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
                projectile.velocity *= 0.98f;
            }
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);
        }
    }
}
