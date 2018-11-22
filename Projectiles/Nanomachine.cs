using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
	public class Nanomachine : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bullet");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}
    	
		public override void SetDefaults()
		{
			projectile.width = 8;
			projectile.height = 8;
			projectile.aiStyle = 1;
			projectile.friendly = true;
			projectile.ranged = true;
			projectile.penetrate = 1;
			projectile.alpha = 255;
			projectile.timeLeft = 120;
			projectile.extraUpdates = 1;
			aiType = ProjectileID.Bullet;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
			for (int k = 0; k < projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
				Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
				spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
			}
			return true;
		}
		
		public override void AI()
        {
			for (int num136 = 0; num136 < 10; num136++)
			{
				float x2 = projectile.position.X - projectile.velocity.X / 10f * (float)num136;
				float y2 = projectile.position.Y - projectile.velocity.Y / 10f * (float)num136;
				int num137 = Dust.NewDust(new Vector2(x2, y2), 1, 1, 229, 0f, 0f, 0, default(Color), 0.5f);
				Main.dust[num137].alpha = projectile.alpha;
				Main.dust[num137].position.X = x2;
				Main.dust[num137].position.Y = y2;
				Main.dust[num137].velocity *= 0f;
				Main.dust[num137].noGravity = true;
			}
			float num138 = (float)Math.Sqrt((double)(projectile.velocity.X * projectile.velocity.X + projectile.velocity.Y * projectile.velocity.Y));
			float num139 = projectile.localAI[0];
			if (num139 == 0f)
			{
				projectile.localAI[0] = num138;
				num139 = num138;
			}
			float num140 = projectile.position.X;
			float num141 = projectile.position.Y;
			float num142 = 800f;
			bool flag4 = false;
			int num143 = 0;
			if (projectile.ai[1] == 0f)
			{
				for (int num144 = 0; num144 < 200; num144++)
				{
					if (Main.npc[num144].CanBeChasedBy(projectile, false) && (projectile.ai[1] == 0f || projectile.ai[1] == (float)(num144 + 1)))
					{
						float num145 = Main.npc[num144].position.X + (float)(Main.npc[num144].width / 2);
						float num146 = Main.npc[num144].position.Y + (float)(Main.npc[num144].height / 2);
						float num147 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num145) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num146);
						if (num147 < num142 && Collision.CanHit(new Vector2(projectile.position.X + (float)(projectile.width / 2), projectile.position.Y + (float)(projectile.height / 2)), 1, 1, Main.npc[num144].position, Main.npc[num144].width, Main.npc[num144].height))
						{
							num142 = num147;
							num140 = num145;
							num141 = num146;
							flag4 = true;
							num143 = num144;
						}
					}
				}
				if (flag4)
				{
					projectile.ai[1] = (float)(num143 + 1);
				}
				flag4 = false;
			}
			if (projectile.ai[1] > 0f)
			{
				int num148 = (int)(projectile.ai[1] - 1f);
				if (Main.npc[num148].active && Main.npc[num148].CanBeChasedBy(projectile, true) && !Main.npc[num148].dontTakeDamage)
				{
					float num149 = Main.npc[num148].position.X + (float)(Main.npc[num148].width / 2);
					float num150 = Main.npc[num148].position.Y + (float)(Main.npc[num148].height / 2);
					float num151 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num149) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num150);
					if (num151 < 1000f)
					{
						flag4 = true;
						num140 = Main.npc[num148].position.X + (float)(Main.npc[num148].width / 2);
						num141 = Main.npc[num148].position.Y + (float)(Main.npc[num148].height / 2);
					}
				}
				else
				{
					projectile.ai[1] = 0f;
				}
			}
			if (!projectile.friendly)
			{
				flag4 = false;
			}
			if (flag4)
			{
				float num152 = num139;
				Vector2 vector13 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
				float num153 = num140 - vector13.X;
				float num154 = num141 - vector13.Y;
				float num155 = (float)Math.Sqrt((double)(num153 * num153 + num154 * num154));
				num155 = num152 / num155;
				num153 *= num155;
				num154 *= num155;
				int num156 = 12;
				projectile.velocity.X = (projectile.velocity.X * (float)(num156 - 1) + num153) / (float)num156;
				projectile.velocity.Y = (projectile.velocity.Y * (float)(num156 - 1) + num154) / (float)num156;
			}
		}
		
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	target.AddBuff(BuffID.Confused, 200);
        }
	}
}