using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class HolyFire : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Holy Fire");
			Main.projFrames[projectile.type] = 4;
		}

		public override void SetDefaults()
		{
			projectile.width = 26;
			projectile.height = 26;
			projectile.hostile = true;
			projectile.ignoreWater = true;
			projectile.alpha = 255;
			projectile.penetrate = 1;
			projectile.aiStyle = 1;
			projectile.tileCollide = false;
			cooldownSlot = 1;
		}

		public override void AI()
		{
			if (projectile.ai[1] == 0f)
			{
				projectile.ai[1] = 1f;
				Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 20);
			}
			projectile.alpha -= 5;
			if (projectile.alpha <= 0)
			{
				projectile.Kill();
			}
			projectile.frameCounter++;
			if (projectile.frameCounter > 6)
			{
				projectile.frame++;
				projectile.frameCounter = 0;
			}
			if (projectile.frame > 3)
			{
				projectile.frame = 0;
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(250, 150, 0, projectile.alpha);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D texture2D13 = Main.projectileTexture[projectile.type];
			int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
			int y6 = num214 * projectile.frame;
			Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
			return false;
		}

		public override void Kill(int timeLeft)
		{
			int randomShot = Main.rand.Next(2);
			if (randomShot == 0)
			{
				if (projectile.owner == Main.myPlayer)
				{
					Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0.01f, 0f, mod.ProjectileType("HolyFire2"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
					Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, -0.01f, 0f, mod.ProjectileType("HolyFire2"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
				}
			}
			else
			{
				if (projectile.owner == Main.myPlayer)
				{
					Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0.05f, 0f, mod.ProjectileType("HolyFire2"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
					Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, -0.05f, 0f, mod.ProjectileType("HolyFire2"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
				}
			}
			Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 20);
			projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
			projectile.width = 150;
			projectile.height = 150;
			projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
			for (int num621 = 0; num621 < 5; num621++)
			{
				int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, default(Color), 2f);
				if (Main.rand.Next(2) == 0)
				{
					Main.dust[num622].scale = 0.5f;
					Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
				}
			}
			for (int num623 = 0; num623 < 10; num623++)
			{
				int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, default(Color), 3f);
				Main.dust[num624].noGravity = true;
				num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, default(Color), 2f);
			}
		}

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			target.AddBuff(mod.BuffType("HolyLight"), 120);
		}
	}
}
