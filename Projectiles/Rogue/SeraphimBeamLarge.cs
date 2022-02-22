using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class SeraphimBeamLarge : BaseLaserbeamProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public int OwnerIndex
        {
            get => (int)projectile.ai[1];
            set => projectile.ai[1] = value;
        }
        public override float MaxScale => 1f;
        public override float MaxLaserLength => 1000f;
        public override float Lifetime => 50;
        public override Color LaserOverlayColor
        {
            get
            {
                Color c1 = Color.Goldenrod;
                Color c2 = Color.Orange;
                Color color = Color.Lerp(c1, c2, projectile.identity % 5f / 5f) * 1.1f;
                color.A = 25;
                return color;
            }
        }
        public override Color LightCastColor => Color.Transparent;
        public override Texture2D LaserBeginTexture => ModContent.GetTexture("CalamityMod/Projectiles/Rogue/SeraphimBeamLarge");
        public override Texture2D LaserMiddleTexture => ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/SeraphimBeamLargeMiddle");
        public override Texture2D LaserEndTexture => ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/SeraphimBeamLargeEnd");

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Beam");
            Main.projFrames[projectile.type] = 10;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft = 600;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
            projectile.Calamity().rogue = true;
            cooldownSlot = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(projectile.localAI[1]);
            writer.Write(projectile.scale);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            projectile.localAI[1] = reader.ReadSingle();
            projectile.scale = reader.ReadSingle();
        }

        public override void AttachToSomething() { }

        public override void UpdateLaserMotion()
        {
            // Update the direction and rotation of the laser.
            projectile.velocity = projectile.velocity.SafeNormalize(Vector2.UnitY);
            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override float DetermineLaserLength()
        {
            float[] sampledLengths = new float[10];
            Collision.LaserScan(projectile.Center, projectile.velocity, projectile.width * projectile.scale, MaxLaserLength, sampledLengths);
            float newLaserLength = sampledLengths.Average();

            if (Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                return MaxLaserLength;

            return newLaserLength;
        }

        public override void PostAI()
        {
            if (projectile.frameCounter == 0)
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon"), projectile.Center);

            // Determine frames.
            projectile.frameCounter++;
            if (projectile.frameCounter % 5f == 4f)
                projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            // This should never happen, but just in case-
            if (projectile.velocity == Vector2.Zero)
                return false;

            Color beamColor = LaserOverlayColor;
            Rectangle startFrameArea = LaserBeginTexture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            Rectangle middleFrameArea = LaserMiddleTexture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            Rectangle endFrameArea = LaserEndTexture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);

            // Start texture drawing.
            Vector2 centerOnLaser = projectile.Center + projectile.velocity * projectile.scale * 116f;
            spriteBatch.Draw(LaserBeginTexture,
                             centerOnLaser - Main.screenPosition,
                             startFrameArea,
                             beamColor,
                             projectile.rotation,
                             LaserBeginTexture.Size() / 2f,
                             projectile.scale,
                             SpriteEffects.None,
                             0f);

            // Prepare things for body drawing.
            float laserBodyLength = LaserLength + middleFrameArea.Height;

            // Body drawing.
            if (laserBodyLength > 0f)
            {
                float laserOffset = middleFrameArea.Height * projectile.scale;
                float incrementalBodyLength = 0f;
                while (incrementalBodyLength + 1f < laserBodyLength)
                {
                    centerOnLaser += projectile.velocity * laserOffset;
                    incrementalBodyLength += laserOffset;
                    spriteBatch.Draw(LaserMiddleTexture,
                                     centerOnLaser - Main.screenPosition,
                                     middleFrameArea,
                                     beamColor,
                                     projectile.rotation,
                                     LaserMiddleTexture.Size() * 0.5f,
                                     projectile.scale,
                                     SpriteEffects.None,
                                     0f);
                }
            }

            Vector2 laserEndCenter = centerOnLaser + projectile.velocity * endFrameArea.Height - Main.screenPosition;
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
    }
}
