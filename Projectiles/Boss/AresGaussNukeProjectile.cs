using CalamityMod.Events;
using CalamityMod.Skies;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class AresGaussNukeProjectile : ModProjectile
    {
		private const int timeLeft = 180;

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
			cooldownSlot = 1;
			projectile.timeLeft = timeLeft;
			projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(projectile.localAI[0]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			projectile.localAI[0] = reader.ReadSingle();
		}

		public override void AI()
        {
			if (projectile.position.Y > projectile.ai[1])
				projectile.tileCollide = true;

			// Animation
			projectile.frameCounter++;
            if (projectile.frameCounter >= 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= 12)
                projectile.frame = 0;

			// Rotation
			projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) - MathHelper.PiOver2;

			// Difficulty modes
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

			// Spawn effects
			if (projectile.localAI[0] == 0f)
			{
				projectile.localAI[0] = 1f;
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
					Dust dust = Dust.NewDustPerfect(projectile.Center, 107, -dustVel, 0, default, scale);
					dust.noGravity = true;
				}

				// Gauss sparks
				if (Main.myPlayer == projectile.owner)
				{
					int totalProjectiles = malice ? 18 : 12;
					float radians = MathHelper.TwoPi / totalProjectiles;
					int type = ModContent.ProjectileType<AresGaussNukeProjectileSpark>();
					float velocity = projectile.velocity.Length();
					double angleA = radians * 0.5;
					double angleB = MathHelper.ToRadians(90f) - angleA;
					float velocityX2 = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
					Vector2 spinningPoint = Main.rand.NextBool() ? new Vector2(0f, -velocity) : new Vector2(-velocityX2, -velocity);
					for (int k = 0; k < totalProjectiles; k++)
					{
						Vector2 velocity2 = spinningPoint.RotatedBy(radians * k);
						Projectile.NewProjectile(projectile.Center, velocity2 + Vector2.Normalize(projectile.velocity) * -6f, type, (int)(projectile.damage * 0.5), 0f, Main.myPlayer);
					}
				}
			}

			// Light
			Lighting.AddLight(projectile.Center, 0.2f, 0.25f, 0.05f);

			// Get a target and calculate distance from it
			int target = Player.FindClosest(projectile.Center, 1, 1);
            Vector2 distanceFromTarget = Main.player[target].Center - projectile.Center;

			// Set AI to stop homing, start accelerating
			float stopHomingDistance = malice ? 260f : death ? 280f : revenge ? 290f : expertMode ? 300f : 320f;
            if ((distanceFromTarget.Length() < stopHomingDistance && projectile.ai[0] != -1f) || projectile.ai[0] == 1f)
            {
				projectile.ai[0] = 1f;

				if (projectile.velocity.Length() < 24f)
					projectile.velocity *= 1.025f;

				return;
            }

			// Home in on target
			float scaleFactor = projectile.velocity.Length();
			float inertia = malice ? 6f : death ? 8f : revenge ? 9f : expertMode ? 10f : 12f;
			distanceFromTarget.Normalize();
			distanceFromTarget *= scaleFactor;
			projectile.velocity = (projectile.velocity * inertia + distanceFromTarget) / (inertia + 1f);
			projectile.velocity.Normalize();
			projectile.velocity *= scaleFactor;
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
			return false;
        }

		public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D texture = Main.projectileTexture[projectile.type];
			int height = texture.Height / Main.projFrames[projectile.type];
			int drawStart = height * projectile.frame;
			Vector2 origin = projectile.Size / 2;
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (projectile.spriteDirection == -1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Projectiles/Boss/AresGaussNukeProjectileGlow"), projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, drawStart, texture.Width, height)), Color.White, projectile.rotation, origin, projectile.scale, spriteEffects, 0f);
		}

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(projectile.Center, 45f, targetHitbox);

		public override void Kill(int timeLeft)
        {
			// Nuke explosion sound
			Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/TeslaCannonFire"), projectile.Center);

			// Nuke gores
			Gore.NewGore(projectile.position, projectile.velocity, mod.GetGoreSlot("Gores/Ares/AresGaussNuke1"), 1f);
			Gore.NewGore(projectile.position, projectile.velocity, mod.GetGoreSlot("Gores/Ares/AresGaussNuke3"), 1f);

			// Create a bunch of lightning bolts in the sky
			ExoMechsSky.CreateLightningBolt(12);

			if (Main.myPlayer == projectile.owner && projectile.ai[0] != -1f)
			{
				// Explosion waves
				for (int i = 0; i < 3; i++)
				{
					Projectile explosion = Projectile.NewProjectileDirect(projectile.Center, Vector2.Zero, ModContent.ProjectileType<AresGaussNukeProjectileBoom>(), projectile.damage, 0f, Main.myPlayer);
					if (explosion.whoAmI.WithinBounds(Main.maxProjectiles))
					{
						// Make the max explosion radius decrease over time, creating a ring effect.
						explosion.ai[1] = 800f + i * 90f;
						explosion.localAI[1] = 0.25f;
						explosion.Opacity = MathHelper.Lerp(0.18f, 0.6f, i / 7f) + Main.rand.NextFloat(-0.08f, 0.08f);
						explosion.netUpdate = true;
					}
				}
			}
		}

		public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
		{
			target.Calamity().lastProjectileHit = projectile;
		}
	}
}
