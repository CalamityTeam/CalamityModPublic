using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class VoidVortexProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Void Vortex");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 38;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 180;
            projectile.magic = true;
        }

        public override void AI()
        {
            // Spin chaotically given a pre-defined spin direction. Choose one initially at random.
            float spinTheta = 0.11f;
            if (projectile.localAI[1] == 0f)
                projectile.localAI[1] = Main.rand.NextBool() ? -spinTheta : spinTheta;
            projectile.rotation += projectile.localAI[1];

            // Animate the lightning orb.
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

            // Spiral outwards in an increasingly chaotic fashion.
            float revolutionTheta = 0.12f * projectile.ai[1];
            projectile.velocity = projectile.velocity.RotatedBy(revolutionTheta) * 1.0092f;

            // Initial stagger in frames needs to be skipped over before it starts shooting,
            // but once it's past that then it can fire constantly
            --projectile.ai[0];
            if (projectile.ai[0] < 0f)
                CalamityGlobalProjectile.MagnetSphereHitscan(projectile, 400f, 8f, VoidVortex.OrbFireRate, 2, ModContent.ProjectileType<ClimaxBeam>(), 1D, true);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.timeLeft < 30)
            {
                float num7 = projectile.timeLeft / 30f;
                projectile.alpha = (int)(255f - 255f * num7);
            }
            return new Color(255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha, 0);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture2D13.Width / 2f, num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool CanDamage() => false;
    }
}
