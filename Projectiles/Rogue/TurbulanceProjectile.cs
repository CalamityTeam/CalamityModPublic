using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CalamityMod.Projectiles.Rogue
{
    public class TurbulanceProjectile : ModProjectile
    {
    	public int stealthStrikeTimer = 0;

    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Turbulance");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 2;
            projectile.ignoreWater = true;
            projectile.Calamity().rogue = true;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (projectile.ai[0] < 0.2f)
            {
                projectile.ai[0] += 0.1f;
            }
            else
            {
                projectile.tileCollide = true;
            }

            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 2.355f;
            if (projectile.spriteDirection == -1)
            {
                projectile.rotation -= 1.57f;
            }

            if (Main.rand.Next(5) == 0)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 187, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 100, new Color(53, Main.DiscoG, 255));
            }
            if (Main.rand.Next(5) == 0)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 16, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }

            if (projectile.Calamity().stealthStrike) //Stealth strike
			{
				stealthStrikeTimer++;
				if (stealthStrikeTimer >= 14)
				{
					Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<TurbulanceWindSlash>(), projectile.damage, projectile.knockBack / 2, projectile.owner, 1f, 1f);
					stealthStrikeTimer = 0;
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
            if (projectile.owner == Main.myPlayer)
            {
				for (int num252 = 0; num252 < 4; num252++)
				{
					Vector2 value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
					while (value15.X == 0f && value15.Y == 0f)
					{
						value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
					}
					value15.Normalize();
					value15 *= (float)Main.rand.Next(70, 101) * 0.1f;
					Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value15.X, value15.Y, ModContent.ProjectileType<TurbulanceWindSlash>(), projectile.damage / 3, projectile.knockBack / 3, Main.myPlayer, 0f, (crit ? 1f /*homing*/: 0f/*not homing*/));
				}
			}
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.owner == Main.myPlayer)
            {
                for (int num252 = 0; num252 < 4; num252++)
                {
                    Vector2 value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    while (value15.X == 0f && value15.Y == 0f)
                    {
                        value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    }
                    value15.Normalize();
                    value15 *= (float)Main.rand.Next(70, 101) * 0.1f;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value15.X, value15.Y, ModContent.ProjectileType<TurbulanceWindSlash>(), projectile.damage / 3, projectile.knockBack / 3, Main.myPlayer, 0f, 0f);
                }
			}
			return true;
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }
    }
}
