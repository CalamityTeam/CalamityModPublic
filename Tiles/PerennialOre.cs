using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
	public class PerennialOre : ModTile
	{
		public override void SetDefaults()
		{
            Main.tileLighted[Type] = true;
            Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileValue[Type] = 690;
			dustType = mod.DustType("CESparkle");
			drop = mod.ItemType("PerennialOre");
			ModTranslation name = CreateMapEntryName();
 			name.SetDefault("Perennial Ore");
 			AddMapEntry(new Color(200, 250, 100), name);
			mineResist = 3f;
			minPick = 200;
			soundType = 21;
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
            r = 0.04f;
            g = 0.10f;
            b = 0.02f;
        }
    }
}
