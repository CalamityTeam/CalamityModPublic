using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles
{
    class AshenLantern : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Ashen Lantern");
            AddMapEntry(new Color(238, 145, 105), name);
            animationFrameHeight = 36;

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            disableSmartCursor = true;
            adjTiles = new int[] { TileID.Torches };
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter >= 6)
            {
                frame = (frame + 1) % 11;
                frameCounter = 0;
            }
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 16, 32, mod.ItemType("AshenLantern"));
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (Main.tile[i, j].frameX < 18)
            {
                r = 0.9f;
                g = 0.3f;
                b = 0.3f;
            } else
            {
                r = 0f;
                g = 0f;
                b = 0f;
            }
        }

        public override void HitWire(int i, int j)
        {
            int x = i - (Main.tile[i, j].frameX / 18) % 1;
            int y = j - (Main.tile[i, j].frameY / 18) % 2;
            for (int l = x; l < x + 1; l++)
            {
                for (int m = y; m < y + 2; m++)
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
                Wiring.SkipWire(x, y + 1);
            }
            //NetMessage.SendTileSquare(-1, x, y + 1, 3);
        }
    }
}
