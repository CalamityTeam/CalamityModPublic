using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
	public class CryonicBrick : ModTile
	{
		public override void SetDefaults()
		{
            Main.tileLighted[Type] = true;
            Main.tileSolid[Type] = true;
			Main.tileBlendAll[this.Type] = true;
			Main.tileBlockLight[Type] = true;
			dustType = mod.DustType("MSparkle");
			drop = mod.ItemType("CryonicBrick");
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Cryonic Brick");
            AddMapEntry(new Color(0, 0, 150), name);
			soundType = 21;
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