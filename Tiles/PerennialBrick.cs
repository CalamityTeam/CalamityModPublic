using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
	public class PerennialBrick : ModTile
	{
		public override void SetDefaults()
		{
            Main.tileLighted[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlendAll[this.Type] = true;
            Main.tileBlockLight[Type] = true;
            dustType = mod.DustType("CESparkle");
			drop = mod.ItemType("PerennialBrick");
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Perennial Brick");
            AddMapEntry(new Color(200, 250, 100), name);
			soundType = 21;
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