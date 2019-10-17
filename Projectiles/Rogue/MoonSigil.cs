using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles
{
    public class MoonSigil : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Moon Sigil");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.alpha = 0;
            projectile.penetrate = -1;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 250;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 2;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (projectile.timeLeft < 25)
            { projectile.alpha += 10; }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = mod.GetTexture("Projectiles/Rogue/MoonSigil");
            spriteBatch.Draw
            (
                texture,
                new Vector2
                (
                    projectile.position.X - Main.screenPosition.X + projectile.width * 0.5f,
                    projectile.position.Y - Main.screenPosition.Y + projectile.height - 20 * 0.5f
                ),
                new Rectangle(0, 0, 20, 20),
                Color.White,
                projectile.rotation,
                new Vector2(10, 10),
                projectile.scale,
                SpriteEffects.None,
                0f
            );
        }
    }
}
