using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
	public class DestroyerHomingLaser : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Homing Laser");
		}

		public override void SetDefaults()
		{
			projectile.width = 4;
			projectile.height = 4;
			projectile.hostile = true;
			projectile.ignoreWater = true;
			projectile.alpha = 255;
			projectile.penetrate = 1;
			projectile.timeLeft = 600;
			projectile.scale = 1.8f;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(projectile.localAI[1]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			projectile.localAI[1] = reader.ReadSingle();
		}

		public override void AI()
		{
			projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
			Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0f) / 255f, ((255 - projectile.alpha) * 0.75f) / 255f, ((255 - projectile.alpha) * 0f) / 255f);
			if (projectile.alpha > 0)
			{
				projectile.alpha -= 125;
			}
			if (projectile.alpha < 0)
			{
				projectile.alpha = 0;
			}
			if (projectile.localAI[1] == 0f)
			{
				Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 33);
			}
			projectile.localAI[1] += 1f;
			if (projectile.localAI[1] >= 120f)
			{
				int num103 = (int)Player.FindClosest(projectile.Center, 1, 1);
				projectile.ai[0] += 1f;
				if (projectile.ai[0] < 360f)
				{
					float scaleFactor2 = projectile.velocity.Length();
					Vector2 vector11 = Main.player[num103].Center - projectile.Center;
					vector11.Normalize();
					vector11 *= scaleFactor2;
					projectile.velocity = (projectile.velocity * 24f + vector11) / 25f;
					projectile.velocity.Normalize();
					projectile.velocity *= scaleFactor2;
				}
			}
			if (projectile.velocity.Length() < 18f)
			{
				projectile.velocity *= 1.02f;
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			if (projectile.alpha < 200)
			{
				return new Color(255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha, 0);
			}
			return Color.Transparent;
		}
	}
}