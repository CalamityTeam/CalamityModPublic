using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Projectiles.Patreon
{
	public class Rox1 : ModProjectile
	{
		public override void SetDefaults()
		{
			projectile.width = 20;
			projectile.height = 20;
			projectile.friendly = true;
			projectile.melee = true;
			projectile.extraUpdates = 1;
		}

		public override void AI()
		{
			//Gravity
			projectile.velocity.Y = projectile.velocity.Y + 0.13f;
			if (projectile.velocity.Y > 16f)
			{
				projectile.velocity.Y = 16f;
			}
			//Projectile rotation
			projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
			projectile.spriteDirection = projectile.direction;
			//Dust trail
			if (Main.rand.Next(10) == 0)
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, 191, projectile.velocity.X * 0.4f, projectile.velocity.Y * 0.4f, 160, default(Color), 0.7f);
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			//Changes the texture of the projectile
			if (projectile.ai[0] == 1f)
			{
				Texture2D texture = mod.GetTexture("Projectiles/Patreon/Rox2");
				Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, texture.Width, 20)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture.Width / 2f, 20 / 2f), projectile.scale, SpriteEffects.None, 0f); return false;
			}
			if (projectile.ai[0] == 2f)
			{
				Texture2D texture = mod.GetTexture("Projectiles/Patreon/Rox3");
				Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, texture.Width, 20)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture.Width / 2f, 20 / 2f), projectile.scale, SpriteEffects.None, 0f);
				return false;
			}
			return true;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			for (int i = 0; i < 8; i++)
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, 191, -projectile.velocity.X * 0.4f, -projectile.velocity.Y * 0.4f, 120, default(Color), 1.2f);
			}
			Main.PlaySound(0, projectile.position);
			projectile.Kill();
			return false;
		}
	}
}
