using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class AresGaussNukeProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gauss Nuke");
            Main.projFrames[projectile.type] = 12;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
			projectile.Calamity().canBreakPlayerDefense = true;
			projectile.width = 100;
            projectile.height = 100;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 480;
			projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
		}

        public override void AI()
        {
			// Animation
            projectile.frameCounter++;
            if (projectile.frameCounter >= 10)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= 12)
                projectile.frame = 0;

			// Rotation
			projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) - MathHelper.PiOver2;

			Vector2 dustOffset = Vector2.One * 50f;

			if (projectile.ai[1] == 0f)
			{
				projectile.ai[1] = 1f;
				for (int i = 0; i < 25; i++)
				{
					// Choose a random speed and angle
					float dustSpeed = Main.rand.NextFloat(3f, 13f);
					float angleRandom = 0.06f;
					Vector2 dustVel = new Vector2(dustSpeed, 0f).RotatedBy(projectile.velocity.ToRotation());
					dustVel = dustVel.RotatedBy(-angleRandom);
					dustVel = dustVel.RotatedByRandom(2f * angleRandom);

					// Random size
					float scale = Main.rand.NextFloat(0.5f, 1.6f);

					// Spawn dust
					Dust dust = Dust.NewDustPerfect(projectile.Center + dustOffset, 107, -dustVel, 0, default, scale);
					dust.noGravity = true;
				}
			}
			else
			{
				// Generate dust behind the nuke
				for (int i = 0; i < 2; i++)
				{
					Dust dust = Dust.NewDustPerfect(projectile.Center + dustOffset, 107);
					dust.velocity = -projectile.velocity * 0.5f;
					dust.noGravity = true;
				}
			}

			// Light
			Lighting.AddLight(projectile.Center, 0.2f, 0.25f, 0.05f);

			// Get a target and calculate distance from it
            int target = Player.FindClosest(projectile.Center, 1, 1);
            Vector2 distanceFromTarget = Main.player[target].Center - projectile.Center;

			// Set AI to stop homing, start accelerating
			float stopHomingDistance = 300f;
            if (distanceFromTarget.Length() < stopHomingDistance || projectile.ai[0] == 1f)
            {
				projectile.ai[0] = 1f;

				if (projectile.velocity.Length() < 24f)
					projectile.velocity *= 1.025f;

				return;
            }

			// Home in on target
			float scaleFactor = projectile.velocity.Length();
			float inertia = 10f;
			distanceFromTarget.Normalize();
			distanceFromTarget *= scaleFactor;
			projectile.velocity = (projectile.velocity * inertia + distanceFromTarget) / (inertia + 1f);
			projectile.velocity.Normalize();
			projectile.velocity *= scaleFactor;
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);

			Rectangle frame = new Rectangle(0, projectile.frame * Main.projectileTexture[projectile.type].Height, Main.projectileTexture[projectile.type].Width, Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]);

			Color color = Color.Lerp(Color.White, Color.GreenYellow, 0.5f) * projectile.Opacity;

			spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Projectiles/Boss/AresGaussNukeProjectileGlow"), projectile.Center - Main.screenPosition, frame, color, projectile.rotation, projectile.Size / 2, 1f, SpriteEffects.None, 0f);

			return false;
        }

		public override bool CanHitPlayer(Player target)
		{
			Rectangle targetHitbox = target.Hitbox;

			float dist1 = Vector2.Distance(projectile.Center, targetHitbox.TopLeft());
			float dist2 = Vector2.Distance(projectile.Center, targetHitbox.TopRight());
			float dist3 = Vector2.Distance(projectile.Center, targetHitbox.BottomLeft());
			float dist4 = Vector2.Distance(projectile.Center, targetHitbox.BottomRight());

			float minDist = dist1;
			if (dist2 < minDist)
				minDist = dist2;
			if (dist3 < minDist)
				minDist = dist3;
			if (dist4 < minDist)
				minDist = dist4;

			return minDist <= 45f;
		}

		public override void Kill(int timeLeft)
        {
			// Nuke explosion sound
			Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/TeslaCannonFire"), projectile.Center);

			// Nuke gores
			Gore.NewGore(projectile.position, projectile.velocity, mod.GetGoreSlot("Gores/Ares/AresGaussNuke1"), 1f);
			Gore.NewGore(projectile.position, projectile.velocity, mod.GetGoreSlot("Gores/Ares/AresGaussNuke3"), 1f);

			if (Main.myPlayer == projectile.owner)
			{
				// Gauss sparks
				int totalProjectiles = CalamityWorld.malice ? 18 : 12;
				float radians = MathHelper.TwoPi / totalProjectiles;
				int type = ModContent.ProjectileType<AresGaussNukeProjectileSpark>();
				float velocity = 12f;
				double angleA = radians * 0.5;
				double angleB = MathHelper.ToRadians(90f) - angleA;
				float velocityX2 = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
				Vector2 spinningPoint = Main.rand.NextBool() ? new Vector2(0f, -velocity) : new Vector2(-velocityX2, -velocity);
				for (int k = 0; k < totalProjectiles; k++)
				{
					Vector2 velocity2 = spinningPoint.RotatedBy(radians * k);
					Projectile.NewProjectile(projectile.Center, velocity2, type, (int)(projectile.damage * 0.6), 0f, Main.myPlayer);
				}

				// Explosion waves
				for (int i = 0; i < 7; i++)
				{
					Projectile explosion = Projectile.NewProjectileDirect(projectile.Center, Vector2.Zero, ModContent.ProjectileType<AresGaussNukeProjectileBoom>(), projectile.damage, 0f, Main.myPlayer);
					if (explosion.whoAmI.WithinBounds(Main.maxProjectiles))
					{
						explosion.ai[1] = 800f + i * 45f;
						explosion.localAI[1] = 0.25f;
						explosion.Opacity = MathHelper.Lerp(0.18f, 0.6f, i / 7f) + Main.rand.NextFloat(-0.08f, 0.08f);
						explosion.netUpdate = true;
					}
				}
			}
		}
    }
}
