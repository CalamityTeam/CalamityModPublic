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
            Projectile.width = 752;
            Projectile.height = 752;
            Projectile.friendly = true;
            Projectile.alpha = 0;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 150;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.Calamity().rogue = true;
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
                Projectile.Kill();
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[Projectile.type];
            spriteBatch.Draw
            (
                texture,
                Projectile.Center - Main.screenPosition,
                new Rectangle(frameX * 752, frameY * 752, 752, 752),
                Color.White,
                Projectile.rotation,
                Projectile.Size / 2f,
                Projectile.scale,
                SpriteEffects.None,
                0f
            );
            return false;
        }
    }
}
