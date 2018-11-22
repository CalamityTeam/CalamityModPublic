using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
	public class EutrophicSand : ModTile
	{
		public override void SetDefaults()
		{
            Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;
			ModTranslation name = CreateMapEntryName();
 			name.SetDefault("Eutrophic Sand");
 			AddMapEntry(new Color(0, 255, 150), name);
            soundType = 0;
            dustType = 185;
		}

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            return false;
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}