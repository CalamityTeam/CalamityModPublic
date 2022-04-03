using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Furniture.CraftingStations
{
    public class StaticRefiner : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Static Refiner");
            AddMapEntry(new Color(191, 142, 111), name);
            animationFrameHeight = 54;
            //also counts as a Solidifier
            adjTiles = new int[] { TileID.Solidifier };
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 243, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter >= 6)
            {
                frame = (frame + 1) % 10;
                frameCounter = 0;
            }
        }

        // Below is an example completely manually drawing a tile. It shows some interesting concepts that may be useful for more advanced things.
        /*public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            // Flips the sprite
            SpriteEffects effects = SpriteEffects.None;
            if (i % 2 == 1)
            {
                effects = SpriteEffects.FlipHorizontally;
            }

            // Tweak the frame drawn by x position so tiles next to each other are off-sync and look much more interesting.
            int k = Main.tileFrame[Type] + i % 6;
            if (i % 2 == 0)
            {
                k += 3;
            }
            if (i % 3 == 0)
            {
                k += 3;
            }
            if (i % 4 == 0)
            {
                k += 3;
            }
            k = k % 6;

            Tile tile = Main.tile[i, j];
            Texture2D texture;
            if (Main.canDrawColorTile(i, j))
            {
                texture = Main.tileAltTexture[Type, (int)tile.color()];
            }
            else
            {
                texture = Main.tileTexture[Type];
            }
            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }
            int animate = k * animationFrameWidth;

            Main.spriteBatch.Draw(
                texture,
                new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
                new Rectangle(tile.frameX + animate, tile.frameY, 16, 16),
                Lighting.GetColor(i, j), 0f, default(Vector2), 1f, effects, 0f);

            return false; // return false to stop vanilla draw.
        }*/

        //      public override void AnimateTile(ref int frame, ref int frameCounter)
        //{
        //    /*frameCounter++;
        //    if (frameCounter > 8)
        //    {
        //        frameCounter = 0;
        //        frame++;
        //        if (frame > 5)
        //        {
        //            frame = 0;
        //        }
        //    }*/
        //    // Above code works, but since we are just mimicking another tile, we can just use the same value.
        //    frame = Main.tileFrame[TileID.FireflyinaBottle];
        //}

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 16, 32, ModContent.ItemType<Items.Placeables.Furniture.CraftingStations.StaticRefiner>());
        }
    }
}
