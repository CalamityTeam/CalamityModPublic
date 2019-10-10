using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Astral
{
    public class RadiantStar : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Radiant Star");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			projectile.width = 24;
			projectile.height = 24;
			projectile.friendly = true;
			projectile.penetrate = 4;
			projectile.timeLeft = 300;
			projectile.Calamity().rogue = true;
		}

		public override void AI()
		{
			projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 2.355f;
			if (projectile.spriteDirection == -1)
			{
				projectile.rotation -= 1.57f;
			}
			if (projectile.ai[0] == 1f)
			{
				float num472 = projectile.Center.X;
				float num473 = projectile.Center.Y;
				float num474 = 600f;
				for (int num475 = 0; num475 < 200; num475++)
				{
					if (Main.npc[num475].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[num475].Center, 1, 1) && !Main.npc[num475].boss)
					{
						float num476 = Main.npc[num475].position.X + (float)(Main.npc[num475].width / 2);
						float num477 = Main.npc[num475].position.Y + (float)(Main.npc[num475].height / 2);
						float num478 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num476) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num477);
						if (num478 < num474)
						{
							if (Main.npc[num475].position.X < num472)
							{
								Main.npc[num475].velocity.X += 0.25f;
							}
							else
							{
								Main.npc[num475].velocity.X -= 0.25f;
							}
							if (Main.npc[num475].position.Y < num473)
							{
								Main.npc[num475].velocity.Y += 0.25f;
							}
							else
							{
								Main.npc[num475].velocity.Y -= 0.25f;
							}
						}
					}
				}
			}
			projectile.ai[1] += 1f;
			if (projectile.ai[1] == 25f)
			{
				int numProj = 2;
				float rotation = MathHelper.ToRadians(50);
				if (projectile.owner == Main.myPlayer)
				{
					for (int i = 0; i < numProj + 1; i++)
					{
						Vector2 speed = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
						while (speed.X == 0f && speed.Y == 0f)
						{
							speed = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
						}
						speed.Normalize();
						speed *= ((float)Main.rand.Next(30, 61) * 0.1f) * 2.5f;
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, speed.X, speed.Y, mod.ProjectileType("RadiantStar2"), projectile.damage, projectile.knockBack, projectile.owner,
							(projectile.ai[0] == 1f ? 1f : 0f), 0f);
					}
					Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 14);
					Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("RadiantExplosion"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
					projectile.active = false;
				}
			}
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(mod.BuffType("AstralInfectionDebuff"), 120);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
			return false;
		}

		public override void Kill(int timeLeft)
		{
			Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 27);
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, mod.DustType("AstralBlue"), projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
			}
		}
	}
}
