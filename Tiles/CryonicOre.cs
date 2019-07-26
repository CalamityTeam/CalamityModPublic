using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.Tiles
{
	public class CryonicOre : ModTile
	{
		public override void SetDefaults()
		{
            Main.tileLighted[Type] = true;
            Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileValue[Type] = 675;
			dustType = mod.DustType("MSparkle");
			drop = mod.ItemType("CryonicOre");
			ModTranslation name = CreateMapEntryName();
 			name.SetDefault("Cryonic Ore");
 			AddMapEntry(new Color(0, 0, 150), name);
			mineResist = 3f;
			minPick = 180;
			soundType = 21;
			Main.tileSpelunker[Type] = true;
		}

		public override bool CanExplode(int i, int j)
		{
			return CalamityWorld.downedCryogen;
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.02f;
            g = 0.02f;
            b = 0.06f;
        }
    }
}
