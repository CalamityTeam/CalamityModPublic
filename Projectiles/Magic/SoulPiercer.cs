using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class SoulPiercer : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Piercer");
		}

		public override void SetDefaults()
		{
			projectile.width = 4;
			projectile.height = 4;
			projectile.friendly = true;
			projectile.magic = true;
			projectile.penetrate = -1;
			projectile.extraUpdates = 100;
			projectile.timeLeft = 240;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 5;
		}

		public override void AI()
		{
			projectile.localAI[0] += 1f;
			if (projectile.localAI[0] > 9f)
			{
				for (int num447 = 0; num447 < 4; num447++)
				{
					Vector2 vector33 = projectile.position;
					vector33 -= projectile.velocity * ((float)num447 * 0.25f);
					projectile.alpha = 255;
					int num448 = Dust.NewDust(vector33, 1, 1, 173, 0f, 0f, 0, default, 0.5f);
					Main.dust[num448].position = vector33;
					Main.dust[num448].scale = (float)Main.rand.Next(70, 110) * 0.007f;
					Main.dust[num448].velocity *= 0.2f;
				}
			}
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			for (int x = 0; x < 3; x++)
			{
				float xPos = projectile.ai[0] > 0 ? projectile.position.X + 500 : projectile.position.X - 500;
				Vector2 vector2 = new Vector2(xPos, projectile.position.Y + Main.rand.Next(-500, 501));
				float num80 = xPos;
				float speedX = (float)target.position.X - vector2.X;
				float speedY = (float)target.position.Y - vector2.Y;
				float dir = (float)Math.Sqrt((double)(speedX * speedX + speedY * speedY));
				dir = 10 / num80;
				speedX *= dir * 150;
				speedY *= dir * 150;
				if (projectile.owner == Main.myPlayer)
				{
					Projectile.NewProjectile(vector2.X, vector2.Y, speedX, speedY, mod.ProjectileType("SoulPiercerBolt"), (int)((double)projectile.damage * 0.5), 0f, projectile.owner);
				}
			}
		}
	}
}
