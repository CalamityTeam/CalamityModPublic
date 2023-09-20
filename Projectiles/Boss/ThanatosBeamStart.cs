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
using Terraria.Audio;
using ReLogic.Content;
using CalamityMod.Sounds;

namespace CalamityMod.Projectiles.Boss
{
    public class ThanatosBeamStart : BaseLaserbeamProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public int OwnerIndex
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public bool OwnerIsValid => Main.npc[OwnerIndex].active && Main.npc[OwnerIndex].type == ModContent.NPCType<ThanatosHead>();

        public override float MaxScale => 1f;
        public override float MaxLaserLength => 3600f;
        public override float Lifetime => 180;
        public override Color LaserOverlayColor => new(250, 250, 250, 100);
        public override Color LightCastColor => Color.White;
        public override Texture2D LaserBeginTexture => ModContent.Request<Texture2D>(Texture).Value;
        public override Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/ThanatosBeamMiddle", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserEndTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/ThanatosBeamEnd", AssetRequestMode.ImmediateLoad).Value;

        public override void SetStaticDefaults()
        {
            // Thanatos' mouth laser
            // This is its serious name
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.Calamity().DealsDefenseDamage = true;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AttachToSomething()
        {
            if (OwnerIsValid)
            {
                Vector2 fireFrom = Main.npc[OwnerIndex].Center + Vector2.UnitY * Main.npc[OwnerIndex].gfxOffY;
                fireFrom += Projectile.velocity.SafeNormalize(Vector2.UnitY) * Projectile.scale * 174f;
                Projectile.Center = fireFrom;
            }

            // Die of the owner is invalid in some way.
            // This is not done client-side, as it's possible that they may not have recieved the proper owner index yet.
            else
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.Kill();
                return;
            }

            // Die if the owner is not performing Thanatos' deathray attack.
            if (Main.npc[OwnerIndex].Calamity().newAI[0] != (float)ThanatosHead.Phase.Deathray)
            {
                Projectile.Kill();
                return;
            }
        }

        public override void UpdateLaserMotion()
        {
            Projectile.velocity = Main.npc[OwnerIndex].velocity.SafeNormalize(Vector2.UnitY);
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override void PostAI()
        {
            if (!OwnerIsValid)
                return;

            // Difficulty modes. Used during the firing of the perpendicular lasers.
            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool expertMode = Main.expertMode || bossRush;

            // Spawn dust at the end of the beam.
            int dustType = (int)CalamityDusts.Brimstone;
            Vector2 dustCreationPosition = Projectile.Center + Projectile.velocity * (LaserLength - 14f);
            for (int i = 0; i < 2; i++)
            {
                float dustDirection = Projectile.velocity.ToRotation() + Main.rand.NextBool().ToDirectionInt() * MathHelper.PiOver2;
                Vector2 dustVelocity = dustDirection.ToRotationVector2() * Main.rand.NextFloat(2f, 4f);

                Dust redFlame = Dust.NewDustDirect(dustCreationPosition, 0, 0, dustType, dustVelocity.X, dustVelocity.Y, 0, default, 1f);
                redFlame.noGravity = true;
                redFlame.scale = 1.7f;
            }

            if (Main.rand.NextBool(5))
            {
                Vector2 dustSpawnOffset = Projectile.velocity.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloatDirection() * Projectile.width * 0.5f;

                Dust redFlame = Dust.NewDustDirect(dustCreationPosition + dustSpawnOffset - Vector2.One * 4f, 8, 8, dustType, 0f, 0f, 100, default, 1.5f);
                redFlame.velocity *= 0.5f;

                // Ensure that the dust always moves up.
                redFlame.velocity.Y = -Math.Abs(redFlame.velocity.Y);
            }

            // Periodically fire beams along the laser perpendicular to its direction.
            float laserFireRate = (CalamityWorld.LegendaryMode && revenge) ? 60f : expertMode ? 80f : 160f;
            if (Main.npc[OwnerIndex].Calamity().newAI[2] % laserFireRate == 0f)
            {
                // Play a laser sound to go with the beams.
                SoundEngine.PlaySound(CommonCalamitySounds.LaserCannonSound, Main.npc[OwnerIndex].Center);

                if (Projectile.owner == Main.myPlayer)
                {
                    Vector2 beamDirection = Projectile.velocity.SafeNormalize(Vector2.UnitY);
                    float distanceBetweenProjectiles = bossRush ? 160f : death ? 256f : revenge ? 288f : 320f;
                    Vector2 laserFirePosition = Main.npc[OwnerIndex].Center + beamDirection * distanceBetweenProjectiles;
                    int laserCount = (int)(LaserLength / distanceBetweenProjectiles);
                    int type = ModContent.ProjectileType<ThanatosLaser>();
                    int damage = Projectile.GetProjectileDamage(Main.npc[OwnerIndex].type);
                    for (int i = 0; i < laserCount; i++)
                    {
                        int totalProjectiles = 2;
                        float radians = MathHelper.TwoPi / totalProjectiles;
                        for (int j = 0; j < totalProjectiles; j++)
                        {
                            Vector2 projVelocity = (CalamityWorld.LegendaryMode && revenge) ? new Vector2(Main.rand.Next(-12, 12), Main.rand.Next(-12, 12)) : Projectile.velocity.RotatedBy(radians * j + MathHelper.PiOver2) * 12f;
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), laserFirePosition, projVelocity, type, damage, 0f, Main.myPlayer, 0f, -1f);
                        }
                        laserFirePosition += beamDirection * distanceBetweenProjectiles;
                    }
                }
            }

            // Determine frames.
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 5f == 0f)
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!OwnerIsValid)
                return false;

            // This should never happen, but just in case.
            if (Projectile.velocity == Vector2.Zero || Projectile.localAI[0] < 2f)
                return false;

            Color beamColor = LaserOverlayColor;
            Rectangle startFrameArea = LaserBeginTexture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Rectangle middleFrameArea = LaserMiddleTexture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Rectangle endFrameArea = LaserEndTexture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);

            // Start texture drawing.
            Main.EntitySpriteDraw(LaserBeginTexture,
                             Projectile.Center - Main.screenPosition,
                             startFrameArea,
                             beamColor,
                             Projectile.rotation,
                             LaserBeginTexture.Size() / 2f,
                             Projectile.scale,
                             SpriteEffects.None,
                             0);

            // Prepare things for body drawing.
            float laserBodyLength = LaserLength + middleFrameArea.Height;
            Vector2 centerOnLaser = Projectile.Center + Projectile.velocity * -5f;

            // Body drawing.
            if (laserBodyLength > 0f)
            {
                float laserOffset = middleFrameArea.Height * Projectile.scale;
                float incrementalBodyLength = 0f;
                while (incrementalBodyLength + 1f < laserBodyLength)
                {
                    Main.EntitySpriteDraw(LaserMiddleTexture,
                                     centerOnLaser - Main.screenPosition,
                                     middleFrameArea,
                                     beamColor,
                                     Projectile.rotation,
                                     LaserMiddleTexture.Size() * 0.5f,
                                     Projectile.scale,
                                     SpriteEffects.None,
                                     0);
                    incrementalBodyLength += laserOffset;
                    centerOnLaser += Projectile.velocity * laserOffset;
                    middleFrameArea.Y += LaserMiddleTexture.Height / Main.projFrames[Projectile.type];
                    if (middleFrameArea.Y + middleFrameArea.Height > LaserMiddleTexture.Height)
                        middleFrameArea.Y = 0;
                }
            }

            Vector2 laserEndCenter = centerOnLaser - Main.screenPosition;
            Main.EntitySpriteDraw(LaserEndTexture,
                             laserEndCenter,
                             endFrameArea,
                             beamColor,
                             Projectile.rotation,
                             LaserEndTexture.Size() * 0.5f,
                             Projectile.scale,
                             SpriteEffects.None,
                             0);
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);
        }

        public override bool CanHitPlayer(Player target) => OwnerIsValid && Projectile.scale >= 0.5f;
    }
}
