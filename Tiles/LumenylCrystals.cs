using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ObjectData;

namespace CalamityMod.Tiles
{
    public class LumenylCrystals : ModTile
	{
		public override void SetDefaults()
		{
            Main.tileLighted[Type] = true;
			Main.tileNoFail[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileObsidianKill[Type] = true;
			ModTranslation name = CreateMapEntryName();
 			name.SetDefault("Lumenyl");
            		AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
 			AddMapEntry(new Color(0, 150, 200), name);
            soundType = 2;
            soundStyle = 27;
            dustType = 67;
            drop = mod.ItemType("Lumenite");
            Main.tileSpelunker[Type] = true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.00f;
            g = 0.3f;
            b = 0.4f;
        }

		public override bool CanPlace(int i, int j)
		{
			if (Main.tile[i, j + 1].slope() == 0 && !Main.tile[i, j + 1].halfBrick())
				return true;
			if (Main.tile[i, j - 1].slope() == 0 && !Main.tile[i, j - 1].halfBrick())
				return true;
			if (Main.tile[i + 1, j].slope() == 0 && !Main.tile[i + 1, j].halfBrick())
				return true;
			if (Main.tile[i - 1, j].slope() == 0 && !Main.tile[i - 1, j].halfBrick())
				return true;
			return false;
		}

		public override void PlaceInWorld(int i, int j, Item item)
		{
			if (Main.tile[i, j + 1].active() && Main.tileSolid[Main.tile[i, j + 1].type] && Main.tile[i, j + 1].slope() == 0 && !Main.tile[i, j + 1].halfBrick())
			{
				Main.tile[i, j].frameY = (short)(0 * 18);
			}
			else if (Main.tile[i, j - 1].active() && Main.tileSolid[Main.tile[i, j - 1].type] && Main.tile[i, j - 1].slope() == 0 && !Main.tile[i, j - 1].halfBrick())
			{
				Main.tile[i, j].frameY = (short)(1 * 18);
			}
			else if (Main.tile[i + 1, j].active() && Main.tileSolid[Main.tile[i + 1, j].type] && Main.tile[i + 1, j].slope() == 0 && !Main.tile[i + 1, j].halfBrick())
			{
				Main.tile[i, j].frameY = (short)(2 * 18);
			}
			else if (Main.tile[i - 1, j].active() && Main.tileSolid[Main.tile[i - 1, j].type] && Main.tile[i - 1, j].slope() == 0 && !Main.tile[i - 1, j].halfBrick())
			{
				Main.tile[i, j].frameY = (short)(3 * 18);
			}
			Main.tile[i, j].frameX = (short)(WorldGen.genRand.Next(18) * 18);
		}
	}
}
