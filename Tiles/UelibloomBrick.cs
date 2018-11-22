using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
	public class UelibloomBrick : ModTile
	{
		public override void SetDefaults()
		{
            Main.tileLighted[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlendAll[this.Type] = true;
            Main.tileBlockLight[Type] = true;
            dustType = mod.DustType("TCESparkle");
			drop = mod.ItemType("UelibloomBrick");
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Uelibloom Brick");
            AddMapEntry(new Color(0, 255, 0), name);
			soundType = 21;
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
	}
}