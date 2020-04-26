using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
namespace CalamityMod.Projectiles.Summon
{
    public class AndromedaDeathLightning : ModProjectile
    {
		public Vector2 StartingVelocity;
		public const int PositioningCheckRate = 40;
		public const int TrueTimeLeft = 120;
		public const float DrawOpacity = 0.5f;
		public const float AngleDifferenceThreshold = MathHelper.Pi / 9f;
		public static readonly Color OuterColor = new Color(166, 205, 252, 0);
		public static readonly Color MiddleColor = new Color(124, 224, 249, 0);
		public static readonly Color InnerColor = new Color(167, 249, 205, 0);
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Lightning");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 80;
			ProjectileID.Sets.TrailingMode[projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			projectile.width = 14;
			projectile.height = 14;
			projectile.friendly = true;
			projectile.alpha = 255;
			projectile.penetrate = -1;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 20;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.extraUpdates = 20;
			projectile.timeLeft = projectile.extraUpdates * TrueTimeLeft;
		}

		public override void AI()
		{
			Player player = Main.player[projectile.owner];
			if (player.ownedProjectileCounts[ModContent.ProjectileType<GiantIbanRobotOfDoom>()] <= 0)
			{
				projectile.Kill();
			}
			if (projectile.localAI[1] == 1f)
			{
				projectile.timeLeft = 2;
				return;
			}
			if (projectile.timeLeft == 20 * 30)
			{
				if (Main.myPlayer == projectile.owner && ++projectile.ai[1] <= 2)
				{
					float angle = MathHelper.ToRadians(Main.rand.NextFloat(25f, 45f));
					for (int i = -1; i <= 1; i += 2)
					{
						Projectile.NewProjectileDirect(projectile.Center,
												 projectile.velocity.RotatedBy(angle * i) * 0.6f,
												 projectile.type,
												 projectile.damage,
												 projectile.knockBack,
												 projectile.owner,
												 checked(projectile.ai[0] + i),
												 projectile.ai[1]);
					}
				}
				projectile.localAI[1] = 1f;
				projectile.velocity = Vector2.Zero;
			}
			projectile.frameCounter++;
			Lighting.AddLight(projectile.Center, Color.LightSkyBlue.ToVector3());
			// Spawn dust and die if there is no movement.
			if (projectile.velocity == Vector2.Zero && projectile.timeLeft >= 20 * 30)
			{
				if (projectile.frameCounter >= PositioningCheckRate)
				{
					projectile.frameCounter = 0;
					bool killImmediately = true;
					// If all past positions are equal (meaning the projectile has not moved at all recently), kill the projectile.
					for (int oldPositionIndex = 1; oldPositionIndex < projectile.oldPos.Length; oldPositionIndex++)
					{
						if (projectile.oldPos[oldPositionIndex] != projectile.oldPos[0])
						{
							killImmediately = false;
						}
					}
					if (killImmediately)
					{
						projectile.Kill();
						return;
					}
				}
				if (Main.rand.NextBool(projectile.extraUpdates) && !Main.dedServ)
				{
					for (int i = 0; i < 2; i++)
					{
						float orthogonalAngle = projectile.rotation + Main.rand.NextBool(2).ToDirectionInt() * MathHelper.PiOver2;
						float dustSpeed = Main.rand.NextFloat(1f, 1.8f);
						Vector2 dustVelocity = orthogonalAngle.ToRotationVector2() * dustSpeed;
						int dustIdx = Dust.NewDust(projectile.Center, 0, 0, 226, dustVelocity.X, dustVelocity.Y, 0, default, 1f);
						Main.dust[dustIdx].noGravity = true;
						Main.dust[dustIdx].scale = 1.2f;
					}
					if (Main.rand.NextBool(5))
					{
						Vector2 dustSpawnOffset = projectile.velocity.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-0.5f, 0.5f) * projectile.width;
						Dust dust = Dust.NewDustDirect(projectile.Center + dustSpawnOffset - Vector2.One * 4f, 8, 8, 226, 0f, 0f, 100, default, 1.5f);
						dust.velocity *= 0.5f;
						dust.velocity.Y = -Math.Abs(dust.velocity.Y); // Force the dust to only move upward
					}
				}
			}
			else if (projectile.frameCounter >= PositioningCheckRate)
			{
				if (projectile.localAI[0] == 0f)
				{
					StartingVelocity = projectile.velocity;
					projectile.localAI[0] = 1f;
				}
				projectile.frameCounter = 0;
				int tries = 0;
				float speed = projectile.velocity.Length();
				UnifiedRandom unifiedRandom = new UnifiedRandom((int)projectile.ai[0]);
				Vector2 newVelocity = -Vector2.UnitY;
				Vector2 randomVector;
				do
				{
					int randomValue = unifiedRandom.Next();
					projectile.ai[0] = randomValue; // To fed as the seed next time this is performed.
					randomValue %= 100;
					float randomAngle = randomValue / 100f * MathHelper.TwoPi;
					randomVector = randomAngle.ToRotationVector2();
					bool changeLightningDirection = true;
					// Don't redirect if the new vector isn't similar in movement to the current velocity.
					if (Math.Abs(randomVector.ToRotation() - StartingVelocity.ToRotation()) > AngleDifferenceThreshold)
					{
						changeLightningDirection = false;
					}
					if (changeLightningDirection)
					{
						newVelocity = randomVector;
						break;
					}
					tries++;
				}
				while (tries < 100);

				if (newVelocity != -Vector2.UnitY)
				{
					projectile.velocity = newVelocity * speed;
					projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Vector2 scaleVector;
			Texture2D lightningSpotTexture = ModContent.GetTexture(Texture);
			projectile.GetAlpha(lightColor);
			// DelegateMethods.c_1 is the color passed into the draw utilities below, and DelegateMethods.f_1 is an opacity multiplier.
			for (int i = 0; i < 3; i++)
			{
				switch (i)
				{
					case 0:
						scaleVector = new Vector2(projectile.scale) * 0.6f;
						DelegateMethods.c_1 = OuterColor;
						break;
					case 1:
						scaleVector = new Vector2(projectile.scale) * 0.4f;
						DelegateMethods.c_1 = MiddleColor;
						break;
					default:
						scaleVector = new Vector2(projectile.scale) * 0.2f;
						DelegateMethods.c_1 = InnerColor;
						break;
				}
				DelegateMethods.f_1 = DrawOpacity;
				// Draw towards the next "old" position from the current one.
				for (int oldPositionIndex = projectile.oldPos.Length - 1; oldPositionIndex > 0; oldPositionIndex--)
				{
					if (!(projectile.oldPos[oldPositionIndex] == Vector2.Zero))
					{
						Vector2 start = projectile.oldPos[oldPositionIndex] + projectile.Size * 0.5f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
						Vector2 end = projectile.oldPos[oldPositionIndex - 1] + projectile.Size * 0.5f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
						Utils.DrawLaser(spriteBatch, lightningSpotTexture, start, end, scaleVector, new Utils.LaserLineFraming(DelegateMethods.LightningLaserDraw));
					}
				}
				if (projectile.oldPos[0] != Vector2.Zero)
				{
					DelegateMethods.f_1 = DrawOpacity;
					Vector2 start = projectile.oldPos[0] + projectile.Size * 0.5f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
					// End is based on the position because when creating its "trail" it relies on the old positions, which always end up at the current, true position.
					Vector2 end = projectile.position + projectile.Size * 0.5f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
					Utils.DrawLaser(spriteBatch, lightningSpotTexture, start, end, scaleVector, new Utils.LaserLineFraming(DelegateMethods.LightningLaserDraw));
				}
			}
			return false;
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			// Draw towards the next "old" position from the current one.
			for (int oldPositionIndex = projectile.oldPos.Length - 1; oldPositionIndex > 0; oldPositionIndex--)
			{
				if (!(projectile.oldPos[oldPositionIndex] == Vector2.Zero))
				{
					Vector2 start = projectile.oldPos[oldPositionIndex] + projectile.Size * 0.5f + Vector2.UnitY * projectile.gfxOffY;
					Vector2 end = projectile.oldPos[oldPositionIndex - 1] + projectile.Size * 0.5f + Vector2.UnitY * projectile.gfxOffY;
					if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end))
						return true;
				}
			}
			return false;
		}
	}
}
