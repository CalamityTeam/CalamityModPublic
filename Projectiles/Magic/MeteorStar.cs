using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
	public class MeteorStar : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Meteor Star");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            Main.projFrames[projectile.type] = 3;
		}

		public override void SetDefaults()
		{
			projectile.width = 42;
			projectile.height = 34;
			projectile.friendly = true;
			projectile.magic = true;
			projectile.timeLeft = 361;
		}

		public override void AI()
		{
            Player player = Main.player[projectile.owner];
			bool flippedGravity = player.gravDir == -1f;

            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }

            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.5f / 255f, (255 - projectile.alpha) * 0.5f / 255f, (255 - projectile.alpha) * 0.15f / 255f);

			bool explodingSoon = projectile.timeLeft <= 120;
			if (projectile.soundDelay <= 0)
			{
				projectile.soundDelay = 30 + Main.rand.Next(40);
				if (explodingSoon)
					projectile.soundDelay -= 30;
				if (Main.rand.NextBool(10) || explodingSoon)
				{
					Main.PlaySound(SoundID.Item9, projectile.Center);
				}
			}
			if (Main.rand.NextBool(20) || explodingSoon && Main.rand.NextBool(3))
			{
				int idx = Gore.NewGore(projectile.Center, projectile.velocity * 0.2f, Main.rand.Next(16, 18), 1f);
				Main.gore[idx].velocity *= 0.66f;
				Main.gore[idx].velocity += projectile.velocity * 0.3f;
			}
			if (explodingSoon)
			{
				for (int i = 0; i < 3; i++)
				{
					int smoke = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
					Dust dust = Main.dust[smoke];
					dust.velocity *= 0.3f;
					dust.position.X = projectile.Center.X + 4f + Main.rand.NextFloat(-6f, 6f);
					dust.position.Y = projectile.Center.Y + Main.rand.NextFloat(-6f, 6f);
					dust.noGravity = true;
				}
			}

			if (Main.myPlayer == projectile.owner)
			{
				if (player.channel)
				{
					float speed = 14f;
					float mouseDistX = (float)Main.mouseX + Main.screenPosition.X - projectile.Center.X;
					float mouseDistY = (float)Main.mouseY + Main.screenPosition.Y - projectile.Center.Y;
					if (player.gravDir == -1f)
					{
						mouseDistY = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - projectile.Center.Y;
					}
					Vector2 mouseVec = new Vector2(mouseDistX, mouseDistY);
					float mouseDist = mouseVec.Length();
					if (mouseDist > speed)
					{
						mouseDist = speed / mouseDist;
						mouseVec.X *= mouseDist;
						mouseVec.Y *= mouseDist;
						int mouseSpeedX = (int)(mouseVec.X * 1000f);
						int projSpeedX = (int)(projectile.velocity.X * 1000f);
						int mouseSpeedY = (int)(mouseVec.Y * 1000f);
						int projSpeedY = (int)(projectile.velocity.Y * 1000f);
						if (mouseSpeedX != projSpeedX || mouseSpeedY != projSpeedY)
						{
							projectile.netUpdate = true;
						}
						projectile.velocity.X = mouseVec.X;
						projectile.velocity.Y = mouseVec.Y;
					}
					else
					{
						int mouseSpeedX = (int)(mouseVec.X * 1000f);
						int projSpeedX = (int)(projectile.velocity.X * 1000f);
						int mouseSpeedY = (int)(mouseVec.Y * 1000f);
						int projSpeedY = (int)(projectile.velocity.Y * 1000f);
						if (mouseSpeedX != projSpeedX || mouseSpeedY != projSpeedY)
						{
							projectile.netUpdate = true;
						}
						projectile.velocity.X = mouseVec.X;
						projectile.velocity.Y = mouseVec.Y;
					}
					player.direction = (projectile.velocity.X > 0).ToDirectionInt();
					player.velocity = projectile.velocity;
					if (projectile.velocity.Y > 16f)
					{
						projectile.velocity.Y = 16f;
						player.velocity.Y = 16f;
					}
					if (player.mount != null)
					{
						player.mount.Dismount(player);
					}
					if (!flippedGravity)
						player.Bottom = projectile.Center;
					else
						player.Top = projectile.Center;
				}
				else
				{
					Explode(true);
				}
			}

			projectile.tileCollide = projectile.velocity != Vector2.Zero && ++projectile.ai[0] >= 5f;

            // Die immediately if the owner of this projectile is clipping into tiles because of its movement.
            if (Collision.SolidCollision(player.position, player.width, player.height) && projectile.velocity != Vector2.Zero)
            {
                player.velocity.Y = 0f;
				Explode();
            }

			if (projectile.timeLeft <= 1)
				Explode();
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => Explode();

		public override void OnHitPvp(Player target, int damage, bool crit) => Explode();

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			Explode();
            return base.OnTileCollide(oldVelocity);
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);

			// Draw the main texture in fullbright
			Texture2D texture = Main.projectileTexture[projectile.type];

			int frameHeight = texture.Height / Main.projFrames[projectile.type];
			int frameY = frameHeight * projectile.frame;
			float scale = projectile.scale;
			float rotation = projectile.rotation;

			Rectangle rectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
			Vector2 origin = rectangle.Size() / 2f;

			SpriteEffects spriteEffects = SpriteEffects.None;
			if (projectile.spriteDirection == -1)
				spriteEffects = SpriteEffects.FlipHorizontally;
			Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), rectangle, Color.White, rotation, origin, scale, spriteEffects, 0f);
			return false;
		}

		private void Explode(bool reducedDmg = false)
		{
			CalamityGlobalProjectile.ExpandHitboxBy(projectile, 64);
			Main.PlaySound(SoundID.Item14, projectile.Center);
			Vector2 spawnPos = projectile.Center;
			spawnPos.Y -= 70f;
			if (reducedDmg)
				projectile.damage /= 6;
			Projectile.NewProjectile(spawnPos, Vector2.Zero, ModContent.ProjectileType<MeteorStarExplosion>(), projectile.damage * 3, projectile.knockBack * 3f, projectile.owner, reducedDmg.ToInt());

			for (int i = 0; i < 10; i++)
			{
				int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1.2f);
				Main.dust[idx].velocity *= 3f;
				if (Main.rand.NextBool(2))
				{
					Main.dust[idx].scale = 0.5f;
					Main.dust[idx].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
				}
			}
			for (int i = 0; i < 5; i++)
			{
				Gore.NewGore(projectile.position, projectile.velocity * 0.05f, Main.rand.Next(16, 18), 1f);
			}
			projectile.Kill();
		}
	}
}
