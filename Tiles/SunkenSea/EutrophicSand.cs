using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.SunkenSea
{
	public class EutrophicSand : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;
			dustType = 108;
			drop = mod.ItemType("EutrophicSand");
			ModTranslation name = CreateMapEntryName();
 			name.SetDefault("Eutrophic Sand");
 			AddMapEntry(new Color(100, 100, 150), name);
			mineResist = 2f;
		}

		public override bool CanKillTile(int i, int j, ref bool blockDamaged)
		{
			return CalamityWorld.downedDesertScourge;
		}

		public override bool CanExplode(int i, int j)
		{
			return CalamityWorld.downedDesertScourge;
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
	}
}