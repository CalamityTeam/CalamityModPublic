using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class AstralStar : ModProjectile
    {
    	private int noTileHitCounter = 120;

    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Star");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
			ProjectileID.Sets.TrailingMode[projectile.type] = 1;
		}

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.alpha = 50;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
		}

        public override void AI()
        {
			if (projectile.ai[0] == 2f)
			{
				projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).rogue = true;
			}
        	int randomToSubtract = Main.rand.Next(1, 3);
        	noTileHitCounter -= randomToSubtract;
        	if (noTileHitCounter == 0)
        	{
        		projectile.tileCollide = true;
        	}
        	if (projectile.soundDelay == 0)
			{
				projectile.soundDelay = 20 + Main.rand.Next(40);
				if (Main.rand.Next(5) == 0)
				{
					Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 9);
				}
			}
			projectile.alpha -= 15;
			int num58 = 150;
			if (projectile.Center.Y >= projectile.ai[1])
			{
				num58 = 0;
			}
			if (projectile.alpha < num58)
			{
				projectile.alpha = num58;
			}
			projectile.localAI[0] += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.01f * (float)projectile.direction;
			projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.01f * (float)projectile.direction;
			if (Main.rand.Next(16) == 0)
			{
				Vector2 value3 = Vector2.UnitX.RotatedByRandom(1.5707963705062866).RotatedBy((double)projectile.velocity.ToRotation(), default(Vector2));
				int num59 = Dust.NewDust(projectile.position, projectile.width, projectile.height, mod.DustType("AstralOrange"), projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 150, default(Color), 1.2f);
				Main.dust[num59].velocity = value3 * 0.66f;
				Main.dust[num59].position = projectile.Center + value3 * 12f;
			}
			if (Main.rand.Next(48) == 0)
			{
				int num60 = Gore.NewGore(projectile.Center, new Vector2(projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f), 16, 1f);
				Main.gore[num60].velocity *= 0.66f;
				Main.gore[num60].velocity += projectile.velocity * 0.3f;
			}
			if (projectile.ai[1] == 1f)
			{
				projectile.light = 0.9f;
				if (Main.rand.Next(10) == 0)
				{
					Dust.NewDust(projectile.position, projectile.width, projectile.height, mod.DustType("AstralOrange"), projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 150, default(Color), 1.2f);
				}
				if (Main.rand.Next(20) == 0)
				{
					Gore.NewGore(projectile.position, new Vector2(projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f), Main.rand.Next(16, 18), 1f);
				}
			}
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 100, 250, projectile.alpha);
        }

        public override void Kill(int timeLeft)
        {
        	projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
			projectile.width = 50;
			projectile.height = 50;
			projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            for (int num621 = 0; num621 < 5; num621++)
			{
				int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, mod.DustType("AstralOrange"), 0f, 0f, 100, default(Color), 1.2f);
				Main.dust[num622].velocity *= 3f;
				if (Main.rand.Next(2) == 0)
				{
					Main.dust[num622].scale = 0.5f;
					Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
				}
			}
			for (int num623 = 0; num623 < 5; num623++)
			{
				int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, mod.DustType("AstralOrange"), 0f, 0f, 100, default(Color), 1.7f);
				Main.dust[num624].noGravity = true;
				Main.dust[num624].velocity *= 5f;
				num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, mod.DustType("AstralOrange"), 0f, 0f, 100, default(Color), 1f);
				Main.dust[num624].velocity *= 2f;
			}
			for (int num480 = 0; num480 < 3; num480++)
			{
				Gore.NewGore(projectile.position, new Vector2(projectile.velocity.X * 0.05f, projectile.velocity.Y * 0.05f), Main.rand.Next(16, 18), 1f);
			}
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 2);
			return false;
		}
	}
}
