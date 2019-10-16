using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;
using Terraria.ObjectData;

namespace CalamityMod.Tiles
{
    class ProfanedCandle : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
            TileObjectData.newTile.CoordinateHeights = new int[] { 20 };
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Profaned Candle");
            AddMapEntry(new Color(191, 142, 111), name);
            disableSmartCursor = true;
            adjTiles = new int[] { TileID.Torches };
            drop = ModContent.ItemType<ProfanedCandle>();
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 246, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, ModContent.DustType<ProfanedTileRock>(), 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (Main.tile[i, j].frameX < 18)
            {
                r = 1f;
                g = 0.85f;
                b = 0.7f;
            }
            else
            {
                r = 0f;
                g = 0f;
                b = 0f;
            }
        }

        public override void HitWire(int i, int j)
        {
            int x = i - Main.tile[i, j].frameX / 18 % 1;
            int y = j - Main.tile[i, j].frameY / 18 % 1;
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
