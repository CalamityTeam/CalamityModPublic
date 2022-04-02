using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class PhantasmalRuinGhost : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantasmal Afterimage");
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 240;
            projectile.extraUpdates = 1;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            // Set the projectile's direction correctly
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4;

            // The projectile rapidly fades in as it starts existing
            if (projectile.timeLeft >= 207)
                projectile.alpha += 6;

            CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 300f, 12f, 40f);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(100, 200, 255, 100);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.timeLeft > 239)
                return false;

            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
