using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Capture;

namespace CalamityMod.Tiles
{
	public class AstralMonolith : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            drop = mod.ItemType("AstralMonolith");
            AddMapEntry(new Color(45, 36, 63));
            animationFrameHeight = 270;
        }
        int animationFrameWidth = 288;

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, mod.DustType("AstralBasic"), 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            int xPos = i % 4;
            int yPos = j % 4;
            frameXOffset = xPos * animationFrameWidth;
            frameYOffset = yPos * animationFrameHeight;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            int xPos = Main.tile[i, j].frameX;
            int yPos = Main.tile[i, j].frameY;
            int xOffset = i % 4;
            int yOffset = j % 4;
            xOffset = xOffset * 288;
            yOffset = yOffset * 270;
            xPos += xOffset;
            yPos += yOffset;
            Texture2D glowmask = mod.GetTexture("Tiles/AstralMonolith_Glowmask");
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + zero;
            Color drawColour = new Color(50, 50, 50, 50);
            Tile trackTile = Main.tile[i, j];
            Texture2D texture3 = glowmask;
            double num6 = Main.time * 0.08;
            if (!trackTile.halfBrick() && trackTile.slope() == 0)
            {
                Main.spriteBatch.Draw(texture3, drawOffset, new Rectangle?(new Rectangle(xPos, yPos, 18, 18)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }
            else if (trackTile.halfBrick())
            {
                Main.spriteBatch.Draw(texture3, drawOffset + new Vector2(0f, 8f), new Rectangle?(new Rectangle(xPos, yPos, 18, 8)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }
        }
    }
}
