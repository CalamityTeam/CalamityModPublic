using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
	public class LumenylCrystals : ModTile
	{
		public override void SetDefaults()
		{
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileNoFail[Type] = true;
            ModTranslation name = CreateMapEntryName();
 			name.SetDefault("Lumenyl");
 			AddMapEntry(new Color(0, 150, 200), name);
            soundType = 2;
            soundStyle = 27;
            dustType = 67;
            drop = mod.ItemType("Lumenite");
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.00f;
            g = 0.3f;
            b = 0.4f;
        }
    }
}