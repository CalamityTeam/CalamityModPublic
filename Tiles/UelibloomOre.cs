using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
	public class UelibloomOre : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileValue[Type] = 805;
			dustType = mod.DustType("TCESparkle");
			drop = mod.ItemType("UelibloomOre");
			ModTranslation name = CreateMapEntryName();
 			name.SetDefault("Uelibloom Ore");
 			AddMapEntry(new Color(0, 255, 0), name);
			mineResist = 5f;
			minPick = 249;
			soundType = 21;
			Main.tileSpelunker[Type] = true;
		}
		
		public override bool CanExplode(int i, int j)
		{
			return NPC.downedMoonlord;
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
	}
}