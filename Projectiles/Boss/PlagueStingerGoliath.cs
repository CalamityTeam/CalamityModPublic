using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
	public class PlagueStingerGoliath : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Plague Homing Stinger");
		}

		public override void SetDefaults()
		{
			projectile.width = 10;
			projectile.height = 10;
			projectile.aiStyle = 1;
			projectile.hostile = true;
			projectile.penetrate = -1;
			projectile.tileCollide = true;
			projectile.timeLeft = 300;
			aiType = 270;
		}

		public override void AI()
		{
			projectile.velocity.X *= 1.01f;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(mod.BuffType("Plague"), 180);
		}

		public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (projectile.spriteDirection == 1)
			{
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
			Vector2 center = new Vector2(projectile.Center.X, projectile.Center.Y);
			Vector2 vector11 = new Vector2((float)(Main.projectileTexture[projectile.type].Width / 2), (float)(Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type] / 2));
			Vector2 vector = center - Main.screenPosition;
			vector -= new Vector2((float)mod.GetTexture("Projectiles/Boss/PlagueStingerGoliathGlow").Width, (float)(mod.GetTexture("Projectiles/Boss/PlagueStingerGoliathGlow").Height / Main.projFrames[projectile.type])) * 1f / 2f;
			vector += vector11 * 1f + new Vector2(0f, 0f + 4f + projectile.gfxOffY);
			Microsoft.Xna.Framework.Color color = new Microsoft.Xna.Framework.Color(127 - projectile.alpha, 127 - projectile.alpha, 127 - projectile.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.Red);
			Main.spriteBatch.Draw(mod.GetTexture("Projectiles/Boss/PlagueStingerGoliathGlow"), vector,
				null, color, projectile.rotation, vector11, 1f, spriteEffects, 0f);
		}
	}
}