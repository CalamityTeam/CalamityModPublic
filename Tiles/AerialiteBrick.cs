using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
	public class AerialiteBrick : ModTile
	{
		public override void SetDefaults()
		{
            Main.tileLighted[Type] = true;
            Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            Main.tileBlendAll[this.Type] = true;
            dustType = mod.DustType("AHSparkle");
			drop = mod.ItemType("AerialiteBrick");
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Aerialite Brick");
            AddMapEntry(new Color(0, 255, 255), name);
			soundType = 21;
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
	}
}