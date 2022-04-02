using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class MagneticOrb : ModProjectile
    {
        private const int Lifetime = 120;
        private const float FramesPerBeam = 12f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magnetic Orb");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 38;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 120;
            projectile.magic = true;
        }

        public override bool CanDamage() => false;

        public override void AI()
        {
            // Drift to a stop after being launched
            projectile.velocity *= 0.972f;

            // On frame 1, pick a random offset to use for the firing pattern.
            if (projectile.timeLeft == Lifetime)
                projectile.localAI[0] = Main.rand.NextFloat(0f, FramesPerBeam);

            // What in the hell does this code do
            if (projectile.velocity.X > 0f)
            {
                projectile.rotation += (Math.Abs(projectile.velocity.Y) + Math.Abs(projectile.velocity.X)) * 0.001f;
            }
            else
            {
                projectile.rotation -= (Math.Abs(projectile.velocity.Y) + Math.Abs(projectile.velocity.X)) * 0.001f;
            }

            // Update animation
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame > 4)
                {
                    projectile.frame = 0;
                }
            }

            CalamityGlobalProjectile.MagnetSphereHitscan(projectile, 300f, 8f, FramesPerBeam, 1, ModContent.ProjectileType<MagneticBeam>(), 1D, true);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.timeLeft < 30)
            {
                float num7 = (float)projectile.timeLeft / 30f;
                projectile.alpha = (int)(255f - 255f * num7);
            }
            return new Color(255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha, 0);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
