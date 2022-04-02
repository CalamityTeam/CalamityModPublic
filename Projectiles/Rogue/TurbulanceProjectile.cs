using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Projectiles.Rogue
{
	public class TurbulanceProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Turbulance";

    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Turbulance");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 20;
            projectile.friendly = true;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 2;
            projectile.ignoreWater = true;
            projectile.Calamity().rogue = true;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
			projectile.ai[0]++;
			projectile.tileCollide = projectile.ai[0] >= 2f;

            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(45f);

            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 187, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 100, new Color(53, Main.DiscoG, 255));
            }
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 16, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }

            if (projectile.Calamity().stealthStrike) //Stealth strike
			{
				if (projectile.timeLeft % 14 == 0)
				{
					Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<TurbulanceWindSlash>(), projectile.damage, projectile.knockBack / 2, projectile.owner, 1f, 1f);
				}
			}
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i <= 10; i++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 187, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f, 50, new Color(53, Main.DiscoG, 255));
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			OnHitEffects(crit);
		}

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
			OnHitEffects(crit);
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			OnHitEffects(false);
			return true;
		}

		private void OnHitEffects(bool homeIn)
		{
            if (projectile.owner == Main.myPlayer)
            {
				for (int w = 0; w < 4; w++)
				{
					Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
					Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<TurbulanceWindSlash>(), projectile.damage / 3, projectile.knockBack / 3, Main.myPlayer, 0f, homeIn ? 1f : 0f);
				}
			}
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
