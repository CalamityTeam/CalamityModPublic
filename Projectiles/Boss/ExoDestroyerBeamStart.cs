using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class ExoDestroyerBeamStart : ModProjectile
    {
		public NPC ThingToAttachTo => Main.npc.IndexInRange((int)projectile.ai[0]) ? Main.npc[(int)projectile.ai[0]] : null;
		public ref float Time => ref projectile.localAI[0];
		public ref float LengthOfLaser => ref projectile.localAI[1];
		public const int Lifetime = 180;
		public const float BeamPosOffset = 16f;

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Thanatos Deathray");
		}

        public override void SetDefaults()
        {
			projectile.Calamity().canBreakPlayerDefense = true;
			projectile.width = 30;
            projectile.height = 30;
            projectile.hostile = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft = 600;
            cooldownSlot = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Time);
			writer.Write(LengthOfLaser);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Time = reader.ReadSingle();
			LengthOfLaser = reader.ReadSingle();
        }

        public override void AI()
        {
			// Difficulty modes
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || malice;
			bool revenge = CalamityWorld.revenge || malice;
			bool expertMode = Main.expertMode || malice;

			// Die if the thing to attach to disappears.
			if (ThingToAttachTo is null || !ThingToAttachTo.active)
			{
				projectile.Kill();
				return;
			}

			// The direction of the host NPC.
			Vector2 hostNPCDirection = Vector2.Normalize(ThingToAttachTo.velocity);

			// Offset to move the beam forward so that it starts inside the NPC's mouth.
			float beamStartForwardsOffset = -8f;

			// Set the starting location of the beam to the center of the NPC.
			projectile.Center = ThingToAttachTo.Center;
			// Add a fixed offset to align with the NPC's spritesheet (?)
			projectile.position += hostNPCDirection * BeamPosOffset + new Vector2(0f, -ThingToAttachTo.gfxOffY);
			// Add the forwards offset, measured in pixels.
			projectile.position += hostNPCDirection * beamStartForwardsOffset;

            Time++;
            if (Time >= Lifetime)
            {
                projectile.Kill();
                return;
            }

			float scale = 1f;
			projectile.scale = (float)Math.Sin(Time * MathHelper.Pi / Lifetime) * 10f * scale;
            if (projectile.scale > scale)
                projectile.scale = scale;

			projectile.rotation = ThingToAttachTo.velocity.ToRotation();
			projectile.velocity = projectile.rotation.ToRotationVector2();

			projectile.rotation = ThingToAttachTo.velocity.ToRotation() - MathHelper.PiOver2;

			float arraySize = 3f;
            Vector2 samplingPoint = projectile.Center;
            float[] samples = new float[(int)arraySize];
            Collision.LaserScan(samplingPoint, projectile.velocity, projectile.width * projectile.scale, 2400f, samples);
            float laserLength = 0f;
            for (int i = 0; i < samples.Length; i++)
            {
                laserLength += samples[i];
            }
            laserLength /= arraySize;

            // Fire laser through walls at max length if target cannot be seen
            if (!Collision.CanHitLine(ThingToAttachTo.Center, 1, 1, Main.player[ThingToAttachTo.target].Center, 1, 1))
                laserLength = 2400f;

			float amount = 0.5f;
			LengthOfLaser = MathHelper.Lerp(LengthOfLaser, laserLength, amount);

			// Fire beams along the laser
			float divisor = expertMode ? 80f : 160f;
			if (ThingToAttachTo.Calamity().newAI[2] % divisor == 0f && projectile.owner == Main.myPlayer)
			{
				Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon"), ThingToAttachTo.Center);
				Vector2 velocity = projectile.velocity;
				velocity.Normalize();
				float distanceBetweenProjectiles = malice ? 160f : death ? 256f : revenge ? 288f : 320f;
				Vector2 fireFrom = ThingToAttachTo.Center + velocity * distanceBetweenProjectiles;
				int projectileAmt = (int)(LengthOfLaser / distanceBetweenProjectiles);
				int type = ModContent.ProjectileType<ExoDestroyerLaser>();
				int damage = projectile.GetProjectileDamage(ThingToAttachTo.type);
				for (int i = 0; i < projectileAmt; i++)
				{
					int totalProjectiles = 2;
					float radians = MathHelper.TwoPi / totalProjectiles;
					for (int j = 0; j < totalProjectiles; j++)
					{
						Vector2 projVelocity = projectile.velocity.RotatedBy(radians * j + MathHelper.PiOver2) * 12f;
						Projectile.NewProjectile(fireFrom, projVelocity, type, damage, 0f, Main.myPlayer, 0f, -1f);
					}
					fireFrom += velocity * distanceBetweenProjectiles;
				}
			}

			int dustType = (int)CalamityDusts.Brimstone;

			// Spawn dust at the start of the beam
			Vector2 dustPos = projectile.Center + projectile.velocity * 14f;
			for (int i = 0; i < 2; i++)
			{
				float dustRot = projectile.velocity.ToRotation() + ((Main.rand.Next(2) == 1) ? -1f : 1f) * MathHelper.PiOver2;
				float dustVelMult = (float)Main.rand.NextDouble() * 2f + 2f;
				Vector2 dustVel = new Vector2((float)Math.Cos(dustRot) * dustVelMult, (float)Math.Sin(dustRot) * dustVelMult);
				int dust = Dust.NewDust(dustPos, 0, 0, dustType, -dustVel.X, -dustVel.Y, 0, default, 1f);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].scale = 1.7f;
			}

			if (Main.rand.NextBool(5))
			{
				Vector2 dustRot = projectile.velocity.RotatedBy(MathHelper.PiOver2, default) * ((float)Main.rand.NextDouble() - 0.5f) * projectile.width;
				int dust = Dust.NewDust(dustPos + dustRot - Vector2.One * 4f, 8, 8, dustType, 0f, 0f, 100, default, 1.5f);
				Main.dust[dust].velocity *= 0.5f;
				Main.dust[dust].velocity.Y = Math.Abs(Main.dust[dust].velocity.Y);
			}

			// Spawn dust at the end of the beam
			dustPos = projectile.Center + projectile.velocity * (LengthOfLaser - 14f);
            for (int i = 0; i < 2; i++)
            {
                float dustRot = projectile.velocity.ToRotation() + ((Main.rand.Next(2) == 1) ? -1f : 1f) * MathHelper.PiOver2;
                float dustVelMult = (float)Main.rand.NextDouble() * 2f + 2f;
                Vector2 dustVel = new Vector2((float)Math.Cos(dustRot) * dustVelMult, (float)Math.Sin(dustRot) * dustVelMult);
                int dust = Dust.NewDust(dustPos, 0, 0, dustType, dustVel.X, dustVel.Y, 0, default, 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale = 1.7f;
            }

            if (Main.rand.NextBool(5))
            {
                Vector2 dustRot = projectile.velocity.RotatedBy(MathHelper.PiOver2, default) * ((float)Main.rand.NextDouble() - 0.5f) * projectile.width;
                int dust = Dust.NewDust(dustPos + dustRot - Vector2.One * 4f, 8, 8, dustType, 0f, 0f, 100, default, 1.5f);
				Main.dust[dust].velocity *= 0.5f;
                Main.dust[dust].velocity.Y = -Math.Abs(Main.dust[dust].velocity.Y);
            }

            DelegateMethods.v3_1 = new Vector3(0.3f, 0.65f, 0.7f);
            Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * LengthOfLaser, projectile.width * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.velocity == Vector2.Zero)
                return false;

			Texture2D beamStart = Main.projectileTexture[projectile.type];
            Texture2D beamMiddle = ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/ExoDestroyerBeamMiddle");
            Texture2D beamEnd = ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/ExoDestroyerBeamEnd");

            float drawLength = LengthOfLaser;
            Color color = new Color(250, 100, 100, 0);

			// Draw start of beam
            Vector2 vector = projectile.Center - Main.screenPosition;
            Rectangle? sourceRectangle = new Rectangle(0, 30 * (projectile.timeLeft / 3 % 4), beamStart.Width, 30);
			spriteBatch.Draw(beamStart, vector, sourceRectangle, color, projectile.rotation, beamStart.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);

			// Draw middle of beam
			drawLength -= (beamStart.Height / 2 + beamEnd.Height) * projectile.scale;
            Vector2 center = projectile.Center;
			center += projectile.velocity * projectile.scale * beamStart.Height / 2f;
            if (drawLength > 0f)
            {
                float i = 0f;
                Rectangle rectangle = new Rectangle(0, 30 * (projectile.timeLeft / 3 % 4), beamMiddle.Width, 30);
                while (i + 1f < drawLength)
                {
                    if (drawLength - i < rectangle.Height)
                        rectangle.Height = (int)(drawLength - i);

                    spriteBatch.Draw(beamMiddle, center - Main.screenPosition, rectangle, color, projectile.rotation, new Vector2(rectangle.Width / 2, 0f), projectile.scale, SpriteEffects.None, 0f);

					i += rectangle.Height * projectile.scale;
					center += projectile.velocity * rectangle.Height * projectile.scale;

                    rectangle.Y += 30;
                    if (rectangle.Y + rectangle.Height > beamMiddle.Height)
                        rectangle.Y = 0;
                }
            }

			// Draw end of beam
            Vector2 vector2 = center - Main.screenPosition;
            sourceRectangle = new Rectangle(0, 30 * (projectile.timeLeft / 3 % 4), beamEnd.Width, 30);
			spriteBatch.Draw(beamEnd, vector2, sourceRectangle, color, projectile.rotation, beamEnd.Frame(1, 1, 0, 0).Top(), projectile.scale, SpriteEffects.None, 0f);

            return false;
        }

        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = projectile.velocity;
            Utils.PlotTileLine(projectile.Center, projectile.Center + unit * LengthOfLaser, projectile.width * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CutTiles));
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
                return true;

            float collisionPoint = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity * LengthOfLaser, 30f * projectile.scale, ref collisionPoint))
                return true;

            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
			target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 360);
			target.AddBuff(BuffID.OnFire, 360);
		}

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)	
        {
			target.Calamity().lastProjectileHit = projectile;
		}
    }
}
