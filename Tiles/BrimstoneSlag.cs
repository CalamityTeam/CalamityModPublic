using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
	public class BrimstoneSlag : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = false;
            drop = mod.ItemType("BrimstoneSlag");
            mineResist = 6f;
			minPick = 180;
			dustType = 53;
			soundType = 21;
			ModTranslation name = CreateMapEntryName();
 			name.SetDefault("Brimstone Slag");
 			AddMapEntry(new Color(20, 20, 20), name);
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
		
		public override bool CanExplode(int i, int j)
		{
			return false;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 0.50f;
			g = 0.00f;
			b = 0.00f;
		}
    }
}
