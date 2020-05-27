using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
	public class MurasamaSlash : ModProjectile
	{
		public int frameX = 0;
		public int frameY = 0;
		public int currentFrame => frameY + frameX * 14;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Murasama");
		}

		public override void SetDefaults()
		{
			projectile.width = 318;
			projectile.height = 262;
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.tileCollide = false;
			projectile.melee = true;
			projectile.ownerHitCheck = true;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 0;
			projectile.Calamity().trueMelee = true;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Rectangle frame = new Rectangle(frameX * projectile.width, frameY * projectile.height, projectile.width, projectile.height);
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (projectile.spriteDirection == -1)
				spriteEffects = SpriteEffects.FlipHorizontally;
			spriteBatch.Draw(Main.projectileTexture[projectile.type], projectile.Center - Main.screenPosition, frame, Color.White, projectile.rotation, projectile.Size / 2, 1f, spriteEffects, 0f);
			return false;
		}

		public override void AI()
		{
			Player player = Main.player[projectile.owner];

			//Frames and crap
			projectile.frameCounter += 1;
			if (projectile.frameCounter % 5 == 0)
			{
				frameY += 1;
				if (frameY >= 7)
				{
					frameX += 1;
					frameY = 0;
				}
				if (frameX >= 2)
				{
					frameX = 0;
					frameY = 0;
				}
			}

			//Play the droning noise
			projectile.soundDelay--;
			if (projectile.soundDelay <= 0)
			{
				Main.PlaySound(SoundID.Item15, projectile.Center);
				projectile.soundDelay = 24;
			}

			//Create light and dust
			Vector2 origin = projectile.Center + projectile.velocity * 3f;
			Lighting.AddLight(origin, 3f, 0.2f, 0.2f);
			if (Main.rand.NextBool(3))
			{
				int redDust = Dust.NewDust(origin - projectile.Size / 2f, projectile.width, projectile.height, (int)CalamityDusts.Brimstone, projectile.velocity.X, projectile.velocity.Y, 100, default, 2f);
				Main.dust[redDust].noGravity = true;
				Main.dust[redDust].position -= projectile.velocity;
			}

			Vector2 playerPos = player.RotatedRelativePoint(player.MountedCenter, true);
			if (Main.myPlayer == projectile.owner)
			{
				if (player.channel && !player.noItems && !player.CCed)
				{
					float scaleFactor6 = 1f;
					if (player.ActiveItem().shoot == projectile.type)
					{
						scaleFactor6 = player.ActiveItem().shootSpeed * projectile.scale;
					}
					Vector2 direction = Main.MouseWorld - playerPos;
					direction.Normalize();
					if (direction.HasNaNs())
					{
						direction = Vector2.UnitX * (float)player.direction;
					}
					direction *= scaleFactor6;
					if (direction.X != projectile.velocity.X || direction.Y != projectile.velocity.Y)
					{
						projectile.netUpdate = true;
					}
					projectile.velocity = direction;
				}
				else
				{
					projectile.Kill();
				}
			}

			//Rotation
			float rotationAmt = 0f;
			if (projectile.spriteDirection == -1)
			{
				rotationAmt = MathHelper.Pi;
			}
			projectile.rotation = projectile.velocity.ToRotation() + rotationAmt;
			player.itemRotation = (float)Math.Atan2((double)(projectile.velocity.Y * (float)projectile.direction), (double)(projectile.velocity.X * (float)projectile.direction));

			projectile.position.Y = playerPos.Y - projectile.height / 2f;
			projectile.position.X = playerPos.X - ((projectile.width / 2f) * ((projectile.spriteDirection == -1) ? 1.5f : 0.5f));

			//direction
			projectile.spriteDirection = projectile.direction;
			player.ChangeDir(projectile.direction);
			//Prevents the projectile from dying
			projectile.timeLeft = 2;
			//Channel weapon shit
			player.heldProj = projectile.whoAmI;
			player.itemTime = 2;
			player.itemAnimation = 2;
		}

		public override Color? GetAlpha(Color lightColor) => new Color(200, 0, 0, 0);
	}
}
