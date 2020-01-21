using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class EventHorizonStar : ModProjectile
    {
		private int counter = 0;
		Vector2 initialPosition;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Event Horizon Star");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.magic = true;
            projectile.tileCollide = false;
			projectile.timeLeft = 600;
			projectile.alpha = 180;
        }

        public override void AI()
        {
			//rotation
            projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.01f * (float)projectile.direction;

			//sound effects
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 20 + Main.rand.Next(40);
                if (Main.rand.NextBool(5))
                {
                    Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 9);
                }
            }

			//dust effects
            if (Main.rand.NextBool(10))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 262, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 0, default, 0.75f);
            }

            projectile.localAI[0]++;

            Vector2 playerCenter = Main.player[projectile.owner].Center;
			float centerX = projectile.Center.X;
			float centerY = projectile.Center.Y;

			if (counter == 0)
			{
				initialPosition = playerCenter;
				counter++;
			}
			else if (playerCenter != initialPosition)
			{
				playerCenter = initialPosition;
			}

			float xDist = playerCenter.X - centerX;
			float yDist = playerCenter.Y - centerY;
            float radius = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));

            if (projectile.localAI[0] > 10 && projectile.localAI[0] < 100)
            {
                projectile.ai[1] += 1f / 60f;

                if (projectile.ai[1] > 0)
                {
                    projectile.ai[0] += MathHelper.ToRadians(5f) / projectile.ai[1];
                    projectile.Center = playerCenter + projectile.ai[0].ToRotationVector2() * radius;
                }
            }

			//homing
			if (projectile.localAI[0] >= 100)
			{
				float homingRange = 800f;
				bool homeIn = false;
				for (int i = 0; i < Main.npc.Length; i++)
				{
					if (Main.npc[i].CanBeChasedBy(projectile, false))
					{
						float num476 = Main.npc[i].position.X + (float)(Main.npc[i].width / 2);
						float num477 = Main.npc[i].position.Y + (float)(Main.npc[i].height / 2);
						float num478 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num476) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num477);
						if (num478 < homingRange)
						{
							homingRange = num478;
							centerX = num476;
							centerY = num477;
							homeIn = true;
						}
					}
				}
				if (homeIn)
				{
					projectile.extraUpdates = 1;
					float homingStrength = 20f;
					Vector2 vector35 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
					float num484 = centerX - vector35.X;
					float num485 = centerY - vector35.Y;
					float num486 = (float)Math.Sqrt((double)(num484 * num484 + num485 * num485));
					num486 = homingStrength / num486;
					num484 *= num486;
					num485 *= num486;
					projectile.velocity.X = (projectile.velocity.X * 30f + num484) / 31f;
					projectile.velocity.Y = (projectile.velocity.Y * 30f + num485) / 31f;
				}
				else
					projectile.extraUpdates = 0;
			}
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Daybreak, 300);
            Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<EventHorizonBlackhole>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

		public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
		{
            Texture2D texture = Main.projectileTexture[projectile.type];
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (projectile.spriteDirection == -1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, texture.Width, texture.Height)), new Color(255, 255, 255, 127), projectile.rotation, texture.Size() / 2f, projectile.scale, spriteEffects, 0f);
		}
    }
}
