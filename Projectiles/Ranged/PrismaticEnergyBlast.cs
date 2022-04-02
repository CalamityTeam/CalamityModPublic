using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class PrismaticEnergyBlast : BaseLaserbeamProjectile
    {
        public bool ExplodedYet
        {
            get => projectile.ai[1] == 1f;
            set => projectile.ai[1] = value.ToInt();
        }
        public override string Texture => "CalamityMod/ExtraTextures/Lasers/PrismLaserStart";
        public override float MaxScale => 1f;
        public override float MaxLaserLength => 2700f;
        public override float Lifetime => 50f;
        public override Color LaserOverlayColor => Color.White;
        public override Color LightCastColor => LaserOverlayColor;
        public override Texture2D LaserBeginTexture => ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/PrismLaserStart");
        public override Texture2D LaserMiddleTexture => ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/PrismLaserMid");
        public override Texture2D LaserEndTexture => ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/PrismLaserEnd");

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Energy Blast");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 14;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.tileCollide = false;
            projectile.penetrate = 100;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 450;
        }

        public override bool PreAI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter % 5 == 4)
                projectile.frame++;

            Vector2 laserEnd = projectile.position + projectile.velocity * LaserLength;
            if (Collision.SolidCollision(laserEnd, 84, 84))
                CreateExplosion(laserEnd + projectile.Size * 0.5f);
            return true;
        }

        public void CreateExplosion(Vector2 laserEnd)
        {
            if (Main.myPlayer != projectile.owner || ExplodedYet || LaserLength <= 60f)
                return;

            Projectile.NewProjectile(laserEnd, Vector2.Zero, ModContent.ProjectileType<PrismExplosionLarge>(), projectile.damage / 2, 0f, projectile.owner);

            if (!ExplodedYet)
            {
                ExplodedYet = true;
                projectile.netUpdate = true;
            }
        }

        public override float DetermineLaserLength()
        {
            if (projectile.penetrate == 100)
                return DetermineLaserLength_CollideWithTiles(8);
            return LaserLength;
        }

        public override bool CanDamage() => projectile.penetrate == 100;

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            LaserLength = projectile.Distance(target.Center);
            CreateExplosion(target.Center);
        }

        public override void Kill(int timeLeft)
        {
            Vector2 laserEnd = projectile.Center + projectile.velocity * LaserLength;
            CreateExplosion(laserEnd);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            // Start texture drawing.
            Rectangle beginFrame = LaserBeginTexture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            spriteBatch.Draw(LaserBeginTexture,
                             projectile.Center - Main.screenPosition,
                             beginFrame,
                             Color.White,
                             projectile.rotation,
                             beginFrame.Size() / 2f,
                             projectile.scale,
                             SpriteEffects.None,
                             0f);

            // Prepare things for body drawing.
            float laserBodyLength = LaserLength;
            laserBodyLength -= (LaserBeginTexture.Height * 0.5f + LaserEndTexture.Height) * projectile.scale / Main.projFrames[projectile.type];
            Vector2 centerOnLaser = projectile.Center;

            // Body drawing.
            Rectangle middleFrame = LaserMiddleTexture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            if (laserBodyLength > 30f)
            {
                float laserOffset = (LaserMiddleTexture.Height - 10f) * projectile.scale / Main.projFrames[projectile.type];
                float incrementalBodyLength = 0f;
                while (incrementalBodyLength + 1f < laserBodyLength)
                {
                    spriteBatch.Draw(LaserMiddleTexture,
                                     centerOnLaser - Main.screenPosition,
                                     middleFrame,
                                     Color.White,
                                     projectile.rotation,
                                     middleFrame.Width * 0.5f * Vector2.UnitX,
                                     projectile.scale,
                                     SpriteEffects.None,
                                     0f);
                    incrementalBodyLength += laserOffset;
                    centerOnLaser += projectile.velocity * laserOffset;
                }
            }

            // End texture drawing.
            Rectangle endFrame = LaserEndTexture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            Vector2 laserEndCenter = centerOnLaser - Main.screenPosition;
            spriteBatch.Draw(LaserEndTexture,
                             laserEndCenter,
                             endFrame,
                             Color.White,
                             projectile.rotation,
                             endFrame.Size() * new Vector2(0.5f, 0f),
                             projectile.scale,
                             SpriteEffects.None,
                             0f);
            return false;
        }
    }
}
