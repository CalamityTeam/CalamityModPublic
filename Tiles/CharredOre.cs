using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
	public class CharredOre : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
			Main.tileValue[Type] = 710;
			drop = mod.ItemType("CharredOre");
			ModTranslation name = CreateMapEntryName();
 			name.SetDefault("Charred Ore");
 			AddMapEntry(new Color(128, 0, 0), name);
			mineResist = 6f;
			minPick = 200;
            soundType = 21;
            dustType = 235;
            Main.tileSpelunker[Type] = true;
		}
		
		public override bool CanExplode(int i, int j)
		{
			return NPC.downedPlantBoss;
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 1.00f;
			g = 0.00f;
			b = 0.00f;
		}
    }
}