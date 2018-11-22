using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles
{
    class StatigelCandle : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Statigel Candle");
            AddMapEntry(new Color(238, 145, 105), name);
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            disableSmartCursor = true;
            adjTiles = new int[] { TileID.Torches };
            drop = mod.ItemType("StatigelCandle");
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (Main.tile[i, j].frameX < 18)
            {
                r = 0.8f;
                g = 0.9f;
                b = 1f;
            }
            else
            {
                r = 0f;
                g = 0f;
                b = 0f;
            }
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 8, 8, mod.ItemType("StatigelCandle"));
        }

        public override void HitWire(int i, int j)
        {
            int x = i - (Main.tile[i, j].frameX / 18) % 1;
            int y = j - (Main.tile[i, j].frameY / 18) % 1;
            for (int l = x; l < x + 1; l++)
            {
                for (int m = y; m < y + 1; m++)
                {
                    if (Main.tile[l, m] == null)
                    {
                        Main.tile[l, m] = new Tile();
                    }
                    if (Main.tile[l, m].active() && Main.tile[l, m].type == Type)
                    {
                        if (Main.tile[l, m].frameX < 18)
                        {
                            Main.tile[l, m].frameX += 18;
                        }
                        else
                        {
                            Main.tile[l, m].frameX -= 18;
                        }
                    }
                }
            }
            if (Wiring.running)
            {
                Wiring.SkipWire(x, y);
            }
            //NetMessage.SendTileSquare(-1, x, y + 1, 3);
        }
    }
}
