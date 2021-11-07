using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class ClimaxProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Voltaic Orb");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 38;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 48;
            projectile.magic = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            // Rapidly screech to a halt once spawned.
            projectile.velocity *= 0.86f;

            // Spin chaotically given a pre-defined spin direction. Choose one initially at random.
            float theta = 0.03f;
            if (projectile.localAI[0] == 0f)
                projectile.localAI[0] = Main.rand.NextBool() ? -theta : theta;
            projectile.rotation += projectile.localAI[0];

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

            ++projectile.ai[1];
            // Initial stagger in frames needs to be skipped over before it starts shooting,
            // but once it's past that then it can fire constantly
            if (projectile.ai[1] > projectile.ai[0])
                CalamityGlobalProjectile.MagnetSphereHitscan(projectile, 400f, 8f, Climax.OrbFireRate, 2, ModContent.ProjectileType<ClimaxBeam>(), 1D, true);
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
