using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon.SmallAresArms
{
    public class CannonLaserbeam : BaseLaserbeamProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public Player Owner => Main.player[Projectile.owner];

        public Projectile OwnerProjectile => CalamityUtils.FindProjectileByIdentity((int)Projectile.ai[1], Projectile.owner);

        public const int LifetimeConst = 75;
        public override float MaxScale => 0.5f + (float)Math.Cos(Main.GlobalTimeWrappedHourly * 10f) * 0.07f;
        public override float MaxLaserLength => 1560f;
        public override float Lifetime => LifetimeConst;
        public override Color LaserOverlayColor => Color.White;
        public override Color LightCastColor => LaserOverlayColor;
        public override Texture2D LaserBeginTexture => ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/ThanatosBeamStart", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/ThanatosBeamMiddle", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserEndTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/ThanatosBeamEnd", AssetRequestMode.ImmediateLoad).Value;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 38;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.localNPCHitCooldown = 10;
            Projectile.scale = 0.5f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.hide = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.Calamity().UpdatePriority = 1f;
        }
        
        public override float DetermineLaserLength()
        {
            float[] samples = new float[4];
            Collision.LaserScan(Projectile.Center, Projectile.velocity, Projectile.width * Projectile.scale, MaxLaserLength, samples);
            return samples.Average();
        }

        public override void UpdateLaserMotion()
        {
            if (OwnerProjectile is null)
            {
                Projectile.Kill();
                return;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override void AttachToSomething()
        {
            Projectile ownerProjectile = OwnerProjectile;
            if (ownerProjectile is null)
            {
                Projectile.Kill();
                return;
            }

            float attachmentOffset = ownerProjectile.width * ownerProjectile.scale * 0.75f;
            Projectile.Center = ownerProjectile.Center + ownerProjectile.rotation.ToRotationVector2() * attachmentOffset - Owner.velocity;
            Projectile.rotation = ownerProjectile.rotation;
            Projectile.velocity = Projectile.rotation.ToRotationVector2();

            // Update frames.
            Projectile.frame = Projectile.frameCounter++ / 5 % Main.projFrames[Type];
        }

        public override bool ShouldUpdatePosition() => false;

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
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
                             startFrameArea.Size() * new Vector2(0.5f, 1f),
                             Projectile.scale,
                             SpriteEffects.None,
                             0);

            // Prepare things for body drawing.
            float laserBodyLength = LaserLength + middleFrameArea.Height;
            Vector2 centerOnLaser = Projectile.Center;

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
                                     middleFrameArea.Size() * 0.5f,
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
                             endFrameArea.Size() * 0.5f,
                             Projectile.scale,
                             SpriteEffects.None,
                             0);
            return false;
        }
    }
}
