using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Projectiles.Rogue
{
    public class HoneycombFragment : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Honeycomb Fragment");
        }

        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.width = 14;
            projectile.height = 14;
            projectile.Calamity().rogue = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            //Changes the texture of the projectile
            if (projectile.ai[0] == 1f)
            {
                Texture2D texture = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/HoneycombFragment2");
                Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, 12, 14)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture.Width / 2f, 20 / 2f), projectile.scale, SpriteEffects.None, 0f);
                return false;
            }
            if (projectile.ai[0] == 2f)
            {
                Texture2D texture = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/HoneycombFragment3");
                Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, 16, 14)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture.Width / 2f, 20 / 2f), projectile.scale, SpriteEffects.None, 0f);
                return false;
            }
            return true;
        }

        public override void AI()
        {
            //Rotation and gravity
            projectile.rotation += 0.6f * projectile.direction;
            projectile.velocity.Y = projectile.velocity.Y + 0.27f;
            if (projectile.velocity.Y > 16f)
            {
                projectile.velocity.Y = 16f;
            }
        }

        public override void Kill(int timeLeft)
        {
            //Dust effect
            int splash = 0;
            while (splash < 4)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 9, -projectile.velocity.X * 0.15f, -projectile.velocity.Y * 0.10f, 159, default, 0.9f);
                splash += 1;
            }
        }
    }
}
