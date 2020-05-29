using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class HolyAura : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Aura");
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.hostile = true;
			projectile.aiStyle = -1;
			aiType = -1;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
			projectile.ignoreWater = true;
            projectile.timeLeft = 210;
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D value = Main.projectileTexture[projectile.type];
			Vector2 origin = value.Size() / 2f;
			float num = Main.GlobalTime % 10f / 10f;
			Vector2 value2 = projectile.Center - Main.screenPosition;
			float[] array = new float[15];
			float[] array2 = new float[15];
			float[] array3 = new float[15];
			float[] array4 = new float[15];
			float num2 = 0.5f;
			int num3 = 210;
			num2 = CalamityUtils.GetLerpValue(0f, 60f, projectile.timeLeft, clamped: true) * CalamityUtils.GetLerpValue(num3, num3 - 60, projectile.timeLeft, clamped: true);
			float amount = CalamityUtils.GetLerpValue(0f, 60f, projectile.timeLeft, clamped: true) * CalamityUtils.GetLerpValue(num3, 90f, projectile.timeLeft, clamped: true);
			amount = CalamityUtils.GetLerpValue(0.2f, 0.5f, amount, clamped: true);
			float num4 = 800f / value.Width;
			float num5 = num4 * 0.8f;
			float num6 = (num4 - num5) / 15f;
			float num7 = 30f;
			float num8 = 300f;
			Vector2 value3 = new Vector2(6f, 6f);

			for (int i = 0; i < 15; i++)
			{
				float num9 = (float)Math.Sin(num * ((float)Math.PI * 2f) + (float)Math.PI / 2f + i / 2f);

				array[i] = num9 * (num8 - i * 3f);

				array2[i] = (float)Math.Sin(num * ((float)Math.PI * 2f) * 2f + (float)Math.PI / 3f + i) * num7;
				array2[i] -= i * 3f;

				array3[i] = i / 15f * 2f + num;
				array3[i] = (num9 * 0.5f + 0.5f) * 0.6f + num;

				array4[i] = num5 + (i + 1) * num6;
				array4[i] *= 0.3f;

				Color color = Main.hslToRgb(array3[i] % 1f, 1f, 0.5f) * num2 * amount;

				bool underworld = projectile.ai[0] == 2f;
				if (Main.dayTime)
				{
					color.R = 255;
					if (underworld)
						color.B = 0;
				}
				else
				{
					color.B = 255;
					if (underworld)
						color.G = 0;
					else
						color.R = 0;
				}

				color.A /= 4;

				int fadeTime = 30;
				if (projectile.timeLeft < fadeTime)
				{
					float amount2 = projectile.timeLeft / (float)fadeTime;

					if (color.R > 0)
						color.R = (byte)MathHelper.Lerp(0, color.R, amount2);
					if (color.G > 0)
						color.G = (byte)MathHelper.Lerp(0, color.G, amount2);
					if (color.B > 0)
						color.B = (byte)MathHelper.Lerp(0, color.B, amount2);

					color.A = (byte)MathHelper.Lerp(0, color.A, amount2);
				}

				float rotation = (float)Math.PI / 2f + num9 * ((float)Math.PI / 4f) * -0.3f + (float)Math.PI * i;

				spriteBatch.Draw(value, value2 + new Vector2(array[i], array2[i]), null, color, rotation, origin, new Vector2(array4[i], array4[i]) * value3, SpriteEffects.None, 0);
			}

			return false;
		}
	}
}
