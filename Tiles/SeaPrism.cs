using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
	public class SeaPrism : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;
			dustType = 33;
			//drop = mod.ItemType("CryonicOre");
			ModTranslation name = CreateMapEntryName();
 			name.SetDefault("Sea Prism");
 			AddMapEntry(new Color(0, 150, 200), name);
			mineResist = 3f;
			minPick = 199;
			soundType = 21;
			Main.tileSpelunker[Type] = true;
		}
		
		public override void RandomUpdate(int i, int j)
		{
			Main.tileValue[Type] = (short)(Main.hardMode ? 700 : 0);
		}
		
		public override bool CanExplode(int i, int j)
		{
			return CalamityWorld.downedLeviathan;
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
    }
}