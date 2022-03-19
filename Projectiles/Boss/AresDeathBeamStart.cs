using CalamityMod.Events;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class AresDeathBeamStart : BaseLaserbeamProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public int OwnerIndex
        {
            get => (int)projectile.ai[1];
            set => projectile.ai[1] = value;
        }
        public override float MaxScale => 1f;
        public override float MaxLaserLength => 2400f;
        public override float Lifetime => AresBody.deathrayDuration;
        public override Color LaserOverlayColor => new Color(250, 250, 250, 100);
        public override Color LightCastColor => Color.White;
        public override Texture2D LaserBeginTexture => ModContent.GetTexture("CalamityMod/Projectiles/Boss/AresDeathBeamStart");
        public override Texture2D LaserMiddleTexture => ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/AresDeathBeamMiddle");
        public override Texture2D LaserEndTexture => ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/AresDeathBeamEnd");

        public override void SetStaticDefaults()
        {
            // Ares' eight-pointed-star laser beams
            DisplayName.SetDefault("Exo Overload Beam");
            Main.projFrames[projectile.type] = 5;
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
            writer.Write(projectile.localAI[0]);
			writer.Write(projectile.localAI[1]);
		}

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
			projectile.localAI[1] = reader.ReadSingle();
		}

        public override void AttachToSomething()
        {
            if (Main.npc[OwnerIndex].active && Main.npc[OwnerIndex].type == ModContent.NPCType<AresBody>())
            {
                Vector2 fireFrom = new Vector2(Main.npc[OwnerIndex].Center.X - 1f, Main.npc[OwnerIndex].Center.Y + 23f);
                fireFrom += projectile.velocity.SafeNormalize(Vector2.UnitY) * MathHelper.Lerp(35f, 127f, projectile.scale * projectile.scale);
                projectile.Center = fireFrom;
            }

            // Die of the owner is invalid in some way.
            else
            {
                projectile.Kill();
                return;
            }

            // Die if the owner is not performing Ares' deathray attack.
            if (Main.npc[OwnerIndex].Calamity().newAI[0] != (float)AresBody.Phase.Deathrays)
            {
                projectile.Kill();
                return;
            }

			// Difficulty modes
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

			// Telegraph duration for deathray spiral
			float deathrayTelegraphDuration = malice ? AresBody.deathrayTelegraphDuration_Malice : death ? AresBody.deathrayTelegraphDuration_Death :
				revenge ? AresBody.deathrayTelegraphDuration_Rev : expertMode ? AresBody.deathrayTelegraphDuration_Expert : AresBody.deathrayTelegraphDuration_Normal;

			Time = Main.npc[OwnerIndex].Calamity().newAI[2] - deathrayTelegraphDuration;
        }

        public override void UpdateLaserMotion()
        {
            // Declare difficulty modes.
            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

            float angularSlowdownDivisor = malice ? 300f : death ? 320f : revenge ? 330f : expertMode ? 340f : 360f;
            float angularVelocity = MathHelper.TwoPi * Time / Lifetime / angularSlowdownDivisor;
			if (Main.npc[OwnerIndex].ai[3] % 2f == 0f)
				angularVelocity *= -1f;

            // Update the direction and rotation of the laser.
            projectile.velocity = projectile.velocity.RotatedBy(angularVelocity);
            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override float DetermineLaserLength()
        {
            float[] sampledLengths = new float[10];
            Collision.LaserScan(projectile.Center, projectile.velocity, projectile.width * projectile.scale, MaxLaserLength, sampledLengths);

            float newLaserLength = sampledLengths.Average();

            // Fire laser through walls at max length if target is behind tiles.
            if (!Collision.CanHitLine(Main.npc[OwnerIndex].Center, 1, 1, Main.player[Main.npc[OwnerIndex].target].Center, 1, 1))
                newLaserLength = MaxLaserLength;

            return newLaserLength;
        }

        public override void PostAI()
        {
            // Spawn dust at the end of the beam.
            int dustType = 107;
            Vector2 dustCreationPosition = projectile.Center + projectile.velocity * (LaserLength - 14f);
            for (int i = 0; i < 2; i++)
            {
                float dustDirection = projectile.velocity.ToRotation() + Main.rand.NextBool().ToDirectionInt() * MathHelper.PiOver2;
                Vector2 dustVelocity = dustDirection.ToRotationVector2() * Main.rand.NextFloat(2f, 4f);
                Dust exoEnergy = Dust.NewDustDirect(dustCreationPosition, 0, 0, dustType, dustVelocity.X, dustVelocity.Y, 0, new Color(0, 255, 255), 1f);
                exoEnergy.noGravity = true;
                exoEnergy.scale = 1.7f;
            }

            if (Main.rand.NextBool(5))
            {
                Vector2 dustSpawnOffset = projectile.velocity.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloatDirection() * projectile.width * 0.5f;
                Dust exoEnergy = Dust.NewDustDirect(dustCreationPosition + dustSpawnOffset - Vector2.One * 4f, 8, 8, dustType, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
                exoEnergy.velocity *= 0.5f;

                // Ensure that the dust always moves up.
                exoEnergy.velocity.Y = -Math.Abs(exoEnergy.velocity.Y);
            }

            // Determine frames.
            projectile.frameCounter++;
            if (projectile.frameCounter % 5f == 0f)
                projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            // This should never happen, but just in case-
            if (projectile.velocity == Vector2.Zero)
                return false;

            // Don't draw the laser if its scale is too low, as that could lead to an infinite loop and out of memory crash.
            // This has happened in multiplayer historically, so this check is important.
            if (projectile.scale < 0.001f)
                return false;

            Color beamColor = LaserOverlayColor;
            Rectangle startFrameArea = LaserBeginTexture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            Rectangle middleFrameArea = LaserMiddleTexture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            Rectangle endFrameArea = LaserEndTexture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);

            // Start texture drawing.
            spriteBatch.Draw(LaserBeginTexture,
                             projectile.Center - Main.screenPosition,
                             startFrameArea,
                             beamColor,
                             projectile.rotation,
                             LaserBeginTexture.Size() / 2f,
                             projectile.scale,
                             SpriteEffects.None,
                             0f);

            // Prepare things for body drawing.
            float laserBodyLength = LaserLength + middleFrameArea.Height;
            Vector2 centerOnLaser = projectile.Center;

            // Body drawing.
            if (laserBodyLength > 0f)
            {
                float laserOffset = middleFrameArea.Height * projectile.scale;
                float incrementalBodyLength = 0f;
                while (incrementalBodyLength + 1f < laserBodyLength)
                {
                    spriteBatch.Draw(LaserMiddleTexture,
                                     centerOnLaser - Main.screenPosition,
                                     middleFrameArea,
                                     beamColor,
                                     projectile.rotation,
                                     LaserMiddleTexture.Size() * 0.5f,
                                     projectile.scale,
                                     SpriteEffects.None,
                                     0f);
                    incrementalBodyLength += laserOffset;
                    centerOnLaser += projectile.velocity * laserOffset;
                    middleFrameArea.Y += LaserMiddleTexture.Height / Main.projFrames[projectile.type];
                    if (middleFrameArea.Y + middleFrameArea.Height > LaserMiddleTexture.Height)
                        middleFrameArea.Y = 0;
                }
            }

            Vector2 laserEndCenter = centerOnLaser - Main.screenPosition;
            spriteBatch.Draw(LaserEndTexture,
                             laserEndCenter,
                             endFrameArea,
                             beamColor,
                             projectile.rotation,
                             LaserEndTexture.Size() * 0.5f,
                             projectile.scale,
                             SpriteEffects.None,
                             0f);
            return false;
        }

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			target.AddBuff(BuffID.OnFire, 360);
			target.AddBuff(BuffID.Frostburn, 360);
		}

		public override bool CanHitPlayer(Player target) => projectile.scale >= 0.5f;

		public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)	
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
