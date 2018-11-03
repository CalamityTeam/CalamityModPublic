using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
	public class AbyssGravel : ModTile
	{
		public override void SetDefaults()
		{
            Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;
            drop = mod.ItemType("AbyssGravel");
            AddMapEntry(new Color(0, 0, 0));
            mineResist = 10f;
            minPick = 64;
            soundType = 21;
            dustType = 33;
		}

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (!closer && j < Main.maxTilesY - 205)
            {
                if (Main.tile[i, j].liquid <= 0)
                {
                    Main.tile[i, j].liquid = 255;
                    Main.tile[i, j].lava(false);
                }
            }
        }
    }
}