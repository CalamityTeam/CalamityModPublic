using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using CalamityMod.CalPlayer;

namespace CalamityMod.Projectiles.Patreon
{
	public class PerforaMini : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Perforamini");
			Main.projFrames[projectile.type] = 8;
			Main.projPet[projectile.type] = true;
		}

		public override void SetDefaults()
		{
			projectile.netImportant = true;
			projectile.width = 32;
			projectile.height = 32;
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.timeLeft *= 5;
			projectile.tileCollide = false;
		}

		public override void AI()
		{
			Player player = Main.player[projectile.owner];
			Vector2 perfcenter = projectile.Center;
			Vector2 vectorperf = player.Center - perfcenter;
			float playerdistance = vectorperf.Length();
			if (!player.active)
			{
				projectile.active = false;
				return;
			}

			//Delete the projectile if the player doesnt have the buff or is very far away (dunno if this needs to be deleted)
			if (!player.HasBuff(mod.BuffType("BloodBound")) || playerdistance >= 4000f)
			{
				projectile.Kill();
			}

			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			if (player.dead)
			{
				modPlayer.perfmini = false;
			}
			if (modPlayer.perfmini)
			{
				projectile.timeLeft = 2;
			}

			float num16 = 0.5f;
			projectile.tileCollide = false;
			int num17 = 100;
			Vector2 vector3 = new Vector2(projectile.position.X + projectile.width * 0.5f, projectile.position.Y + projectile.height * 0.5f);
			float num18 = Main.player[projectile.owner].position.X + (Main.player[projectile.owner].width / 2) - vector3.X;
			float num19 = Main.player[projectile.owner].position.Y + (Main.player[projectile.owner].height / 2) - vector3.Y;
			num18 += 60 * -(float)Main.player[projectile.owner].direction;
			num19 -= 60f;
			float num20 = (float)Math.Sqrt(num18 * num18 + num19 * num19);
			float num21 = 18f;
			if (num20 < num17 && Main.player[projectile.owner].velocity.Y == 0f &&
				projectile.position.Y + projectile.height <= Main.player[projectile.owner].position.Y + Main.player[projectile.owner].height &&
				!Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
			{
				projectile.ai[0] = 0f;
				if (projectile.velocity.Y < -6f)
				{
					projectile.velocity.Y = -6f;
				}
			}
			if (num20 > 2000f)
			{
				projectile.position.X = Main.player[projectile.owner].Center.X - (projectile.width / 2);
				projectile.position.Y = Main.player[projectile.owner].Center.Y - (projectile.height / 2);
				projectile.netUpdate = true;
			}
			if (num20 < 50f)
			{
				if (Math.Abs(projectile.velocity.X) > 2f || Math.Abs(projectile.velocity.Y) > 2f)
				{
					projectile.velocity *= 0.99f;
				}
				num16 = 0.01f;
			}
			else
			{
				if (num20 < 100f)
				{
					num16 = 0.1f;
				}
				if (num20 > 300f)
				{
					num16 = 1f;
				}
				num20 = num21 / num20;
				num18 *= num20;
				num19 *= num20;
			}
			if (projectile.velocity.X < num18)
			{
				projectile.velocity.X = projectile.velocity.X + num16;
				if (num16 > 0.05f && projectile.velocity.X < 0f)
				{
					projectile.velocity.X = projectile.velocity.X + num16;
				}
			}
			if (projectile.velocity.X > num18)
			{
				projectile.velocity.X = projectile.velocity.X - num16;
				if (num16 > 0.05f && projectile.velocity.X > 0f)
				{
					projectile.velocity.X = projectile.velocity.X - num16;
				}
			}
			if (projectile.velocity.Y < num19)
			{
				projectile.velocity.Y = projectile.velocity.Y + num16;
				if (num16 > 0.05f && projectile.velocity.Y < 0f)
				{
					projectile.velocity.Y = projectile.velocity.Y + num16 * 2f;
				}
			}
			if (projectile.velocity.Y > num19)
			{
				projectile.velocity.Y = projectile.velocity.Y - num16;
				if (num16 > 0.05f && projectile.velocity.Y > 0f)
				{
					projectile.velocity.Y = projectile.velocity.Y - num16 * 2f;
				}
			}
			if (projectile.velocity.X >= 0.25)
			{
				projectile.direction = 1;
			}
			else if (projectile.velocity.X < -0.25)
			{
				projectile.direction = -1;
			}

			//Dust
			if (Main.rand.NextBool(50))
			{
				int d1 = Dust.NewDust(projectile.Center, projectile.width, projectile.height, 5, 0f, 0f, 100, default, 1.5f);
				int d2 = Dust.NewDust(projectile.Center, projectile.width, projectile.height, 170, 0f, 0f, 170, default, 0.5f);
				Main.dust[d2].noLight = true;
				Main.dust[d1].position = projectile.Center;
				Main.dust[d2].position = projectile.Center;
			}

			//Tilting and change directions
			projectile.spriteDirection = projectile.direction;
			projectile.rotation = projectile.velocity.X * 0.1f;

			//Animation
			projectile.frameCounter++;
			if (projectile.frameCounter > 6)
			{
				projectile.frame++;
				projectile.frameCounter = 0;
			}
			if (projectile.frame > 5)
			{
				projectile.frame = 0;
			}
		}
	}
}
