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
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 240;
            Projectile.extraUpdates = 1;
            Projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            // Set the projectile's direction correctly
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            // The projectile rapidly fades in as it starts existing
            if (Projectile.timeLeft >= 207)
                Projectile.alpha += 6;

            CalamityGlobalProjectile.HomeInOnNPC(Projectile, true, 300f, 12f, 40f);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(100, 200, 255, 100);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (Projectile.timeLeft > 239)
                return false;

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
