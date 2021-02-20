using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class DarkEnergyBall : ModProjectile
    {
        private double timeElapsed = 0.0;
        private double circleSize = 1.0;
        private double circleGrowth = 0.02;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Energy");
            Main.projFrames[projectile.type] = 6;
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}

        public override void SetDefaults()
        {
			projectile.Calamity().canBreakPlayerDefense = true;
			projectile.width = 80;
            projectile.height = 80;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 600;
            projectile.penetrate = -1;
            cooldownSlot = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(timeElapsed);
            writer.Write(circleSize);
            writer.Write(circleGrowth);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            timeElapsed = reader.ReadDouble();
            circleSize = reader.ReadDouble();
            circleGrowth = reader.ReadDouble();
        }

        public override void AI()
        {
            timeElapsed += 0.02;
            projectile.velocity.X = (float)(Math.Sin(timeElapsed * (0.5f * projectile.ai[0])) * circleSize);
            projectile.velocity.Y = (float)(Math.Cos(timeElapsed * (0.5f * projectile.ai[0])) * circleSize);
            circleSize += circleGrowth;
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 5)
            {
                projectile.frame = 0;
            }
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);

			Rectangle frame = new Rectangle(0, projectile.frame * Main.projectileTexture[projectile.type].Height, Main.projectileTexture[projectile.type].Width, Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]);
			Color color = Color.Lerp(Color.White, Color.Fuchsia, 0.5f);

			spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Projectiles/Boss/DarkEnergyBallGlow"), projectile.Center - Main.screenPosition, frame, color, projectile.rotation, projectile.Size / 2, 1f, SpriteEffects.None, 0f);

			color = Color.Lerp(Color.White, Color.Cyan, 0.5f);

			spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Projectiles/Boss/DarkEnergyBallGlow2"), projectile.Center - Main.screenPosition, frame, color, projectile.rotation, projectile.Size / 2, 1f, SpriteEffects.None, 0f);

			return false;
		}

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			target.AddBuff(BuffID.VortexDebuff, 60);
		}

		public override void Kill(int timeLeft)
        {
			for (int num621 = 0; num621 < 5; num621++)
			{
				int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 90, 0f, 0f, 100, default, 1.2f);
				Main.dust[num622].velocity *= 3f;
				Main.dust[num622].noGravity = true;
				if (Main.rand.NextBool(2))
				{
					Main.dust[num622].scale = 0.5f;
					Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
				}
			}
			for (int num623 = 0; num623 < 10; num623++)
			{
				int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 90, 0f, 0f, 100, default, 1.7f);
				Main.dust[num624].noGravity = true;
				Main.dust[num624].velocity *= 5f;
				num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 90, 0f, 0f, 100, default, 1f);
				Main.dust[num624].velocity *= 2f;
			}
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)	
        {
			target.Calamity().lastProjectileHit = projectile;
		}
    }
}
