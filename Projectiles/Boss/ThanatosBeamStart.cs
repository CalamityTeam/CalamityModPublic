using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.Projectiles.BaseProjectiles;
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
    public class ThanatosBeamStart : BaseLaserbeamProjectile
    {
        public int OwnerIndex
        {
            get => (int)projectile.ai[1];
            set => projectile.ai[1] = value;
        }

        public bool OwnerIsValid => Main.npc[OwnerIndex].active && Main.npc[OwnerIndex].type == ModContent.NPCType<ThanatosHead>();

        public override float MaxScale => 1f;
        public override float MaxLaserLength => 3600f;
        public override float Lifetime => 180;
        public override Color LaserOverlayColor => new Color(250, 250, 250, 100);
        public override Color LightCastColor => Color.White;
        public override Texture2D LaserBeginTexture => Main.projectileTexture[projectile.type];
        public override Texture2D LaserMiddleTexture => ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/ThanatosBeamMiddle");
        public override Texture2D LaserEndTexture => ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/ThanatosBeamEnd");

        public override void SetStaticDefaults()
        {
            // Thanatos' mouth laser
            DisplayName.SetDefault("T Hanos Beam");
            // This is its serious name
            // DisplayName.SetDefault("Gamma Disintegration Beam");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 40;
            projectile.hostile = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft = 600;
            projectile.Calamity().canBreakPlayerDefense = true;
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
            if (OwnerIsValid)
            {
                Vector2 fireFrom = Main.npc[OwnerIndex].Center + Vector2.UnitY * Main.npc[OwnerIndex].gfxOffY;
                fireFrom += projectile.velocity.SafeNormalize(Vector2.UnitY) * projectile.scale * 174f;
                projectile.Center = fireFrom;
            }

            // Die of the owner is invalid in some way.
            // This is not done client-side, as it's possible that they may not have recieved the proper owner index yet.
            else
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    projectile.Kill();
                return;
            }

            // Die if the owner is not performing Thanatos' deathray attack.
            if (Main.npc[OwnerIndex].Calamity().newAI[0] != (float)ThanatosHead.Phase.Deathray)
            {
                projectile.Kill();
                return;
            }
        }

        public override void UpdateLaserMotion()
        {
            projectile.velocity = Main.npc[OwnerIndex].velocity.SafeNormalize(Vector2.UnitY);
            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override void PostAI()
        {
            if (!OwnerIsValid)
                return;

            // Difficulty modes. Used during the firing of the perpendicular lasers.
            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

            // Spawn dust at the end of the beam.
            int dustType = (int)CalamityDusts.Brimstone;
            Vector2 dustCreationPosition = projectile.Center + projectile.velocity * (LaserLength - 14f);
            for (int i = 0; i < 2; i++)
            {
                float dustDirection = projectile.velocity.ToRotation() + Main.rand.NextBool().ToDirectionInt() * MathHelper.PiOver2;
                Vector2 dustVelocity = dustDirection.ToRotationVector2() * Main.rand.NextFloat(2f, 4f);

                Dust redFlame = Dust.NewDustDirect(dustCreationPosition, 0, 0, dustType, dustVelocity.X, dustVelocity.Y, 0, default, 1f);
                redFlame.noGravity = true;
                redFlame.scale = 1.7f;
            }

            if (Main.rand.NextBool(5))
            {
                Vector2 dustSpawnOffset = projectile.velocity.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloatDirection() * projectile.width * 0.5f;

                Dust redFlame = Dust.NewDustDirect(dustCreationPosition + dustSpawnOffset - Vector2.One * 4f, 8, 8, dustType, 0f, 0f, 100, default, 1.5f);
                redFlame.velocity *= 0.5f;

                // Ensure that the dust always moves up.
                redFlame.velocity.Y = -Math.Abs(redFlame.velocity.Y);
            }

            // Periodically fire beams along the laser perpendicular to its direction.
            float laserFireRate = expertMode ? 80f : 160f;
            if (Main.npc[OwnerIndex].Calamity().newAI[2] % laserFireRate == 0f)
            {
                // Play a laser sound to go with the beams.
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon"), Main.npc[OwnerIndex].Center);

                if (projectile.owner == Main.myPlayer)
                {
                    Vector2 beamDirection = projectile.velocity.SafeNormalize(Vector2.UnitY);
                    float distanceBetweenProjectiles = malice ? 160f : death ? 256f : revenge ? 288f : 320f;
                    Vector2 laserFirePosition = Main.npc[OwnerIndex].Center + beamDirection * distanceBetweenProjectiles;
                    int laserCount = (int)(LaserLength / distanceBetweenProjectiles);
                    int type = ModContent.ProjectileType<ThanatosLaser>();
                    int damage = projectile.GetProjectileDamage(Main.npc[OwnerIndex].type);
                    for (int i = 0; i < laserCount; i++)
                    {
                        int totalProjectiles = 2;
                        float radians = MathHelper.TwoPi / totalProjectiles;
                        for (int j = 0; j < totalProjectiles; j++)
                        {
                            Vector2 projVelocity = projectile.velocity.RotatedBy(radians * j + MathHelper.PiOver2) * 12f;
                            Projectile.NewProjectile(laserFirePosition, projVelocity, type, damage, 0f, Main.myPlayer, 0f, -1f);
                        }
                        laserFirePosition += beamDirection * distanceBetweenProjectiles;
                    }
                }
            }

            // Determine frames.
            projectile.frameCounter++;
            if (projectile.frameCounter % 5f == 0f)
                projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (!OwnerIsValid)
                return false;

            // This should never happen, but just in case.
            if (projectile.velocity == Vector2.Zero || projectile.localAI[0] < 2f)
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
            Vector2 centerOnLaser = projectile.Center + projectile.velocity * -5f;

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
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 360);
        }

        public override bool CanHitPlayer(Player target) => OwnerIsValid && projectile.scale >= 0.5f;

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)    
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
