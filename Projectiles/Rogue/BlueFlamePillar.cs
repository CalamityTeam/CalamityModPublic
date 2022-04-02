using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class BlueFlamePillar : ModProjectile
    {
        public int frameX = 0;
        public int frameY = 0;
        public int currentFrame => frameY + frameX * 6;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flame Pillar");
        }

        public override void SetDefaults()
        {
            projectile.width = 80;
            projectile.height = 322;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 180;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.Calamity().rogue = true;
        }
        public override void AI()
        {
            //2-6
            projectile.frameCounter += 1;
            if (projectile.frameCounter % 7 == 6)
            {
                frameY += 1;
                if (frameY >= 6)
                {
                    frameX += 1;
                    frameY = 0;
                }
                if (frameX >= 3)
                {
                    projectile.Kill();
                }
            }
            if (projectile.localAI[0] == 0f)
            {
                projectile.position.Y -= projectile.height / 2; //position adjustments
                projectile.localAI[0] = 1f;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            Rectangle frame = new Rectangle(frameX * 80, frameY * 322, 80, 322);
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, frame, Color.White, projectile.rotation, projectile.Size / 2, 1f, SpriteEffects.None, 0f);
            return false;
        }
    }
}
