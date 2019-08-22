using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Patreon
{
	public class RoxSlam : ModProjectile
	{
		public override void SetDefaults()
		{
			projectile.width = 50;
			projectile.height = 50;
			projectile.friendly = true;
			projectile.melee = true;
			projectile.timeLeft = 400;
			projectile.tileCollide = true;
		}

		public override void AI()
		{
			Player player = Main.player[Main.myPlayer];
			//Kills the projectile if the alt attack ended
			if (player.itemAnimation == 0)
			{
				projectile.Kill();
			}
			//Makes the projectiles follow the player
			projectile.velocity.X = player.velocity.X;
			projectile.velocity.Y = player.velocity.Y;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			//This will only happen if the cooldown was full
			if (projectile.ai[0] == 1)
			{
				Player player = Main.player[Main.myPlayer];
				//Bounce
				player.velocity.Y = -18f;
				//Spawns the shockwave
				Projectile.NewProjectile(projectile.position.X + 25, projectile.position.Y + 25, 0f, 0f, mod.ProjectileType("RoxShockwave"), 300, 12, projectile.owner);
				Main.PlaySound(2, projectile.position, 14);
				projectile.Kill();
				//Pretty things
				for (int dustexplode = 0; dustexplode < 360; dustexplode++)
				{
					Vector2 dustd = new Vector2(17f, 17f).RotatedBy(MathHelper.ToRadians(dustexplode));
					int d = Dust.NewDust(projectile.Center, projectile.width, projectile.height, 51, dustd.X, dustd.Y, 100, default(Color), 3f);
					Main.dust[d].noGravity = true;
					Main.dust[d].position = projectile.Center;
				}
				return false;
			}
			//If the cooldown wasnt full, just bounce
			else
			{
				Player player = Main.player[Main.myPlayer];
				player.velocity.Y = -14f;
				Main.PlaySound(21, projectile.position);
				projectile.Kill();

				return false;
			}
		}
	}
}
