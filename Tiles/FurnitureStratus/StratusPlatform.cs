using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureStratus
{
    public class StratusPlatform : ModTile
    {
        public override void SetStaticDefaults() => this.SetUpPlatform(ModContent.ItemType<Items.Placeables.FurnitureStratus.StratusPlatform>(), true);

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 1, 0f, 0f, 1, new Color(100, 130, 150), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 132, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void PostSetDefaults()
        {
            Main.tileNoSunLight[Type] = false;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            int xPos = Main.tile[i, j].TileFrameX;
            int yPos = Main.tile[i, j].TileFrameY;
            Texture2D glowmask = ModContent.Request<Texture2D>("CalamityMod/Tiles/FurnitureStratus/StratusPlatformGlow").Value;
            Vector2 drawPosition = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + (Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange));
            Tile trackTile = Main.tile[i, j];
            if (!(trackTile.IsHalfBlock && trackTile.Slope == 0))
                spriteBatch.Draw(glowmask, drawPosition, new Rectangle?(new Rectangle(xPos, yPos, 18, 18)), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            else if (trackTile.IsHalfBlock)
                spriteBatch.Draw(glowmask, drawPosition + new Vector2(0f, 8f), new Rectangle?(new Rectangle(xPos, yPos, 18, 8)), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
