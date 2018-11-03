using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
	public class SmoothBrimstoneSlag : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;
            drop = mod.ItemType("SmoothBrimstoneSlag");
            mineResist = 3f;
			minPick = 79;
			dustType = 53;
			soundType = 21;
			ModTranslation name = CreateMapEntryName();
 			name.SetDefault("Smooth Brimstone Slag");
 			AddMapEntry(new Color(20, 20, 20), name);
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 0.50f;
			g = 0.00f;
			b = 0.00f;
		}
    }
}