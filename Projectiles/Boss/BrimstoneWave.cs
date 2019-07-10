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
	public class BrimstoneWave : ModProjectile
	{
		private int x;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brimstone Flame Skull");
			Main.projFrames[projectile.type] = 4;
		}

		public override void SetDefaults()
		{
			projectile.width = 40;
			projectile.height = 30;
			projectile.hostile = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.penetrate = 1;
			projectile.timeLeft = 600;
			cooldownSlot = 1;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(x);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			x = reader.ReadInt32();
		}

		public override void AI()
		{
			x++;
			projectile.velocity.Y = (float)(10.0 * Math.Sin(x / 20));
			projectile.frameCounter++;
			if (projectile.frameCounter > 12)
			{
				projectile.frame++;
				projectile.frameCounter = 0;
			}
			if (projectile.frame > 3)
			{
				projectile.frame = 0;
			}
			Lighting.AddLight(projectile.Center, 0.5f, 0f, 0f);
			if (projectile.velocity.X < 0f)
			{
				projectile.spriteDirection = 1;
			}
			else
			{
				projectile.spriteDirection = -1;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = (projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
			Texture2D texture2D13 = Main.projectileTexture[projectile.type];
			int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
			int y6 = num214 * projectile.frame;
			Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, spriteEffects, 0f);
			return false;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(250, 50, 50, projectile.alpha);
		}

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			target.AddBuff(mod.BuffType("AbyssalFlames"), 180);
			target.AddBuff(mod.BuffType("VulnerabilityHex"), 120, true);
		}
	}
}
