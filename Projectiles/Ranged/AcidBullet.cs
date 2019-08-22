using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class AcidBullet : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Acid Bullet");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
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
			projectile.timeLeft = 600;
			projectile.extraUpdates = 1;
			aiType = ProjectileID.Bullet;
		}

		public override void AI()
		{
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0f) / 255f, ((255 - projectile.alpha) * 0.25f) / 255f, ((255 - projectile.alpha) * 0f) / 255f);
        	projectile.localAI[0] += 1f;
        	if (projectile.localAI[0] >= 4f)
        	{
				for (int num136 = 0; num136 < 10; num136++)
				{
					float x2 = projectile.position.X - projectile.velocity.X / 10f * (float)num136;
					float y2 = projectile.position.Y - projectile.velocity.Y / 10f * (float)num136;
					int num137 = Dust.NewDust(new Vector2(x2, y2), 1, 1, 107, 0f, 0f, 0, default(Color), 0.5f);
					Main.dust[num137].alpha = projectile.alpha;
					Main.dust[num137].position.X = x2;
					Main.dust[num137].position.Y = y2;
					Main.dust[num137].velocity *= 0f;
					Main.dust[num137].noGravity = true;
				}
        	}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
			Main.PlaySound(0, (int)projectile.position.X, (int)projectile.position.Y, 1, 1f, 0f);
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

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	target.AddBuff(mod.BuffType("Plague"), 360);
        }

		public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 107, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
	}
}
