using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Fishing.FishingRods;

namespace CalamityMod.Projectiles.Typeless
{
    public class DevourerofCodsBobber : ModProjectile
    {
        private bool initialized = false;
        private Color fishingLineColor;
        public Color[] PossibleLineColors = new Color[]
        {
            new Color(252, 109, 202, 100), //a pink color
            new Color(39, 151, 171, 100) // a blue color
        };

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Devourer of Cods Bobber");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.aiStyle = 61;
            projectile.bobber = true;
            projectile.penetrate = -1;
        }

        //What if we want to randomize the line color
        public override void AI()
        {
            if (!initialized)
            {
                //Decide color of the pole by randomizing the array
                fishingLineColor = Main.rand.Next(PossibleLineColors);
                initialized = true;
            }
        }

        //fuck glowmasks btw
        //i second this notion -Dominic
        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D glowmask = ModContent.GetTexture("CalamityMod/Projectiles/Typeless/DevourerofCodsGlow");
            float xOffset = (glowmask.Width - projectile.width) * 0.5f + projectile.width * 0.5f;
            Vector2 drawPos = projectile.position - Main.screenPosition;
            drawPos.X += xOffset;
            drawPos.Y += projectile.height / 2f + projectile.gfxOffY;
            Rectangle frame = new Microsoft.Xna.Framework.Rectangle(0, 0, glowmask.Width, glowmask.Height);
            Vector2 origin = new Vector2(xOffset, projectile.height / 2f);
            SpriteEffects spriteEffects = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            if (projectile.ai[0] <= 1f)
            {
                Main.spriteBatch.Draw(glowmask, drawPos, frame, Color.White, projectile.rotation, origin, projectile.scale, spriteEffects, 0f);
            }
        }

        public override bool PreDrawExtras(SpriteBatch spriteBatch)
        {
            Lighting.AddLight(projectile.Center, 0.35f, 0f, 0.25f);
            return projectile.DrawFishingLine(ModContent.ItemType<TheDevourerofCods>(), fishingLineColor);
        }
    }
}
