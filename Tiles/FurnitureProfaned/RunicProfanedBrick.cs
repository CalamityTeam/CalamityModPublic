using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureProfaned
{
	public class RunicProfanedBrick : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = false;
			Main.tileBlockLight[Type] = true;
            soundType = 21;
            mineResist = 10f;
            minPick = 225;
            drop = mod.ItemType("RunicProfanedBrick");
			AddMapEntry(new Color(122, 66, 59));
            animationFrameHeight = 90;
        }
        int animationFrameWidth = 234;

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 246, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, mod.DustType("ProfanedTileRock"), 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameXOffset = (i % 2) * animationFrameWidth;
            frameYOffset = (j % 2) * animationFrameHeight;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            int xPos = Main.tile[i, j].frameX;
            int yPos = Main.tile[i, j].frameY;
            int xOffset = i % 2;
            int yOffset = j % 2;
            xOffset *= animationFrameWidth;
            yOffset *= animationFrameHeight;
            xPos += xOffset;
            yPos += yOffset;
            Texture2D glowmask = mod.GetTexture("Tiles/FurnitureProfaned/RunicProfanedBrick_Glowmask");
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + zero;
            Color drawColour = new Color(128, 128, 128, 128);
            Tile trackTile = Main.tile[i, j];
            double num6 = Main.time * 0.08;
            if (!trackTile.halfBrick() && trackTile.slope() == 0)
            {
                Main.spriteBatch.Draw(glowmask, drawOffset, new Rectangle?(new Rectangle(xPos, yPos, 18, 18)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }
            else if (trackTile.halfBrick())
            {
                Main.spriteBatch.Draw(glowmask, drawOffset + new Vector2(0f, 8f), new Rectangle?(new Rectangle(xPos, yPos, 18, 8)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }
        }
    }
}
