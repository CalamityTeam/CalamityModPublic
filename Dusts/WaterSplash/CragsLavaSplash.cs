using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Dusts.WaterSplash
{
	public class CragsLavaSplash : ModDust
	{
        public override void OnSpawn(Dust dust)
        {
			dust.velocity *= 0.1f;
			dust.velocity.Y = -0.5f;
		}

		public override bool Update(Dust dust)
		{
			if (dust.scale > 10f)
			{
				dust.active = false;
			}
			Dust.lavaBubbles++;
			dust.position += dust.velocity;
			if (!dust.noGravity)
			{
				dust.velocity.Y += 0.1f;
			}
			if (dust.noGravity)
			{
				dust.scale += 0.03f;
				if (dust.scale < 1f)
				{
					dust.velocity.Y += 0.075f;
				}
				dust.velocity.X *= 1.08f;
				if (dust.velocity.X > 0f)
				{
					dust.rotation += 0.01f;
				}
				else
				{
					dust.rotation -= 0.01f;
				}
				float num109 = dust.scale * 0.6f;
				if (num109 > 1f)
				{
					num109 = 1f;
				}
				Lighting.AddLight((int)(dust.position.X / 16f), (int)(dust.position.Y / 16f + 1f), num109 * (2.5f / 4), num109 * (1.1f / 4), num109 * (0.1f / 4));
			}
			else
			{
				if (!Collision.WetCollision(new Vector2(dust.position.X, dust.position.Y - 8f), 4, 4))
				{
					dust.scale = 0f;
				}
				else
				{
					dust.alpha += Main.rand.Next(2);
					if (dust.alpha > 255)
					{
						dust.scale = 0f;
					}
					dust.velocity.Y = -0.5f;
					dust.alpha++;
					dust.scale -= 0.01f;
					dust.velocity.Y = -0.2f;
					dust.velocity.X += (float)Main.rand.Next(-10, 10) * 0.002f;
					if ((double)dust.velocity.X < -0.25)
					{
						dust.velocity.X = -0.25f;
					}
					if ((double)dust.velocity.X > 0.25)
					{
						dust.velocity.X = 0.25f;
					}
				}
				float num3 = dust.scale * 0.3f + 0.4f;
				if (num3 > 1f)
				{
					num3 = 1f;
				}
				Lighting.AddLight((int)(dust.position.X / 16f), (int)(dust.position.Y / 16f), num3 * (2.5f / 4), num3 * (1.1f / 4), num3 * (0.1f / 4));
			}
			dust.rotation += dust.velocity.X * 0.5f;
			if (dust.fadeIn > 0f && dust.fadeIn < 100f)
			{
				dust.scale += 0.03f;
				if (dust.scale > dust.fadeIn)
				{
					dust.fadeIn = 0f;
				}
			}
			dust.scale -= 0.01f;
			if (dust.noGravity)
			{
				dust.velocity *= 0.92f;
				if (dust.fadeIn == 0f)
				{
					dust.scale -= 0.04f;
				}
			}
			if (dust.position.Y > Main.screenPosition.Y + (float)Main.screenHeight)
			{
				dust.active = false;
			}
			float num17 = 0.1f;
			if ((double)Dust.dCount == 0.5)
			{
				dust.scale -= 0.001f;
			}
			if ((double)Dust.dCount == 0.6)
			{
				dust.scale -= 0.0025f;
			}
			if ((double)Dust.dCount == 0.7)
			{
				dust.scale -= 0.005f;
			}
			if ((double)Dust.dCount == 0.8)
			{
				dust.scale -= 0.01f;
			}
			if ((double)Dust.dCount == 0.9)
			{
				dust.scale -= 0.02f;
			}
			if ((double)Dust.dCount == 0.5)
			{
				num17 = 0.11f;
			}
			if ((double)Dust.dCount == 0.6)
			{
				num17 = 0.13f;
			}
			if ((double)Dust.dCount == 0.7)
			{
				num17 = 0.16f;
			}
			if ((double)Dust.dCount == 0.8)
			{
				num17 = 0.22f;
			}
			if ((double)Dust.dCount == 0.9)
			{
				num17 = 0.25f;
			}
			if (dust.scale < num17)
			{
				dust.active = false;
			}
			return false;
		}

		public override Color? GetAlpha(Dust dust, Color lightColor)
		{
			float num = (float)(255 - dust.alpha) / 255f;
			int num4;
			int num3;
			int num2;
			num = (num + 3f) / 4f;
			num4 = (int)((int)lightColor.R * num);
			num3 = (int)((int)lightColor.G * num);
			num2 = (int)((int)lightColor.B * num);
			int num6 = lightColor.A - dust.alpha;
			if (num6 < 0)
			{
				num6 = 0;
			}
			if (num6 > 255)
			{
				num6 = 255;
			}
			return new Color(num4, num3, num2, num6);
		}
	}
}
