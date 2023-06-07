using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Projectiles.BaseProjectiles;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using CalamityMod.Sounds;

namespace CalamityMod.Projectiles.Turret
{
    public class LaserShot : BaseLaserbeamProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public bool DoneHitting
        {
            get => Projectile.ai[1] == 1f;
            set => Projectile.ai[1] = value.ToInt();
        }
        public override string Texture => "CalamityMod/ExtraTextures/Lasers/TurretLaserStart";
        public override float MaxScale => 1f;
        public override float MaxLaserLength => 1030f;
        public override float Lifetime => 20f;
        public override Color LaserOverlayColor => Color.Transparent;
        public override Color LightCastColor => LaserOverlayColor;
        public override Texture2D LaserBeginTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/TurretLaserStart", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/TurretLaserMid", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserEndTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/TurretLaserEnd", AssetRequestMode.ImmediateLoad).Value;

        public NPC struckNPC = null;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 100;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 450;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override bool PreAI()
        {
            // If projectile knockback is set to 0 in the tile entity file, projectile hits players instead
            // This is used to check if the projectile came from the hostile version of the tile entity
            if (Projectile.knockBack == 0f)
                Projectile.hostile = true;
            else Projectile.friendly = true;

            Projectile.frameCounter++;
            if (Projectile.frameCounter % 5 == 4)
                Projectile.frame++;

            if (Projectile.localAI[0] == 0f)
            {
                // play a sound frame 1.
                SoundEngine.PlaySound(CommonCalamitySounds.LaserCannonSound with { Volume = 0.35f }, Projectile.Center);
                Projectile.localAI[0]++;
            }

            return true;
        }

        public override float DetermineLaserLength()
        {
            return DetermineLaserLength_CollideWithTiles(8);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // This is the drawcode from surge driver
            // Start texture drawing.
            Rectangle beginFrame = LaserBeginTexture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Main.EntitySpriteDraw(LaserBeginTexture,
                             Projectile.Center - Main.screenPosition,
                             beginFrame,
                             Color.White,
                             Projectile.rotation,
                             beginFrame.Size() / 2f,
                             Projectile.scale,
                             SpriteEffects.None,
                             0);

            // Prepare things for body drawing.
            float laserBodyLength = LaserLength;
            laserBodyLength -= (LaserBeginTexture.Height * 0.5f + LaserEndTexture.Height) * Projectile.scale / Main.projFrames[Projectile.type];
            Vector2 centerOnLaser = Projectile.Center;

            // Body drawing.
            Rectangle middleFrame = LaserMiddleTexture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            if (laserBodyLength > 30f)
            {
                float laserOffset = (LaserMiddleTexture.Height - 10f) * Projectile.scale / Main.projFrames[Projectile.type];
                float incrementalBodyLength = 0f;
                while (incrementalBodyLength + 1f < laserBodyLength)
                {
                    Main.EntitySpriteDraw(LaserMiddleTexture,
                                     centerOnLaser - Main.screenPosition,
                                     middleFrame,
                                     Color.White,
                                     Projectile.rotation,
                                     middleFrame.Width * 0.5f * Vector2.UnitX,
                                     Projectile.scale,
                                     SpriteEffects.None,
                                     0);
                    incrementalBodyLength += laserOffset;
                    centerOnLaser += Projectile.velocity * laserOffset;
                }
            }

            // End texture drawing.
            Rectangle endFrame = LaserEndTexture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Vector2 laserEndCenter = centerOnLaser - Main.screenPosition;
            Main.EntitySpriteDraw(LaserEndTexture,
                             laserEndCenter,
                             endFrame,
                             Color.White,
                             Projectile.rotation,
                             endFrame.Size() * new Vector2(0.5f, 0f),
                             Projectile.scale,
                             SpriteEffects.None,
                             0);
            return false;
        }
    }
}
