using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class EclipseMirrorBurst : ModProjectile
    {
        private int frameCounter = 0;
        private int frameX = 0;
        private int frameY = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eclipse Mirror Flash");
        }

        public override void SetDefaults()
        {
            projectile.width = 750;
            projectile.height = 750;
            projectile.friendly = true;
            projectile.alpha = 0;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 150;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            frameCounter++;
            if (frameCounter > 3)
            {
                frameCounter = 0;
                frameY++;
                if (frameY > 1)
                {
                    frameX++;
                    frameY = 0;
                }
            }
            if (frameX > 0 && frameY > 0)
            {
                projectile.Kill();
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            spriteBatch.Draw
            (
                texture,
                new Vector2
                (
                    projectile.position.X - Main.screenPosition.X + projectile.width * 0.5f - 50,
                    projectile.position.Y - Main.screenPosition.Y + projectile.height - 750 * 0.5f - 50
                ),
                new Rectangle(frameX * 752, frameY * 752, 750, 750),
                Color.White,
                projectile.rotation,
                new Vector2(325, 325),
                projectile.scale,
                SpriteEffects.None,
                0f
            );
        }
    }
}
