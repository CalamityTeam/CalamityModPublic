using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
	public class ShockblastRound : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Round");
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
			projectile.timeLeft = 600;
			projectile.light = 0.5f;
			projectile.extraUpdates = 1;
			aiType = ProjectileID.Bullet;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("Shockblast"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
			return true;
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
			projectile.ai[0] += 1f;
			if (projectile.ai[0] >= 120f)
			{
				projectile.damage = (int)((double)projectile.damage * 0.995);
				projectile.knockBack = (float)((int)((double)projectile.knockBack * 0.995));
			}
		}
		
		public override bool PreAI()
        {
			for (int num136 = 0; num136 < 5; num136++)
			{
				float x2 = projectile.position.X - projectile.velocity.X / 10f * (float)num136;
				float y2 = projectile.position.Y - projectile.velocity.Y / 10f * (float)num136;
				int num137 = Dust.NewDust(new Vector2(x2, y2), 1, 1, 185, 0f, 0f, 0, default(Color), 1f);
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
			return false;
		}
		
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			if (projectile.owner == Main.myPlayer)
			{
				Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("Shockblast"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
			}
			if (target.type == NPCID.TargetDummy)
			{
				return;
			}
        	float num = (float)damage * 0.1f;
			if ((int)num == 0)
			{
				return;
			}
			if (Main.player[Main.myPlayer].lifeSteal <= 0f)
			{
				return;
			}
			Main.player[Main.myPlayer].lifeSteal -= num;
			int num2 = projectile.owner;
			Projectile.NewProjectile(target.position.X, target.position.Y, 0f, 0f, mod.ProjectileType("TransfusionTrail"), 0, 0f, projectile.owner, (float)num2, num);
        }
	}
}