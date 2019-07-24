using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
	public class PlagueStingerGoliathV2 : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Exploding Plague Stinger");
		}

		public override void SetDefaults()
		{
			projectile.width = 10;
			projectile.height = 10;
			projectile.aiStyle = 0;
			aiType = 55;
			projectile.hostile = true;
			projectile.penetrate = -1;
			projectile.extraUpdates = 2;
			projectile.tileCollide = true;
			projectile.timeLeft = 1200;
		}

		public override void AI()
		{
			projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
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
			vector -= new Vector2((float)mod.GetTexture("Projectiles/Boss/PlagueStingerGoliathV2Glow").Width, (float)(mod.GetTexture("Projectiles/Boss/PlagueStingerGoliathV2Glow").Height / Main.projFrames[projectile.type])) * 1f / 2f;
			vector += vector11 * 1f + new Vector2(0f, 0f + 4f + projectile.gfxOffY);
			Microsoft.Xna.Framework.Color color = new Microsoft.Xna.Framework.Color(127 - projectile.alpha, 127 - projectile.alpha, 127 - projectile.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.Red);
			Main.spriteBatch.Draw(mod.GetTexture("Projectiles/Boss/PlagueStingerGoliathV2Glow"), vector,
				null, color, projectile.rotation, vector11, 1f, spriteEffects, 0f);
		}

		public override void Kill(int timeLeft)
		{
			Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 14);
			if (projectile.owner == Main.myPlayer)
			{
				Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("PlagueExplosion"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
			}
		}
	}
}