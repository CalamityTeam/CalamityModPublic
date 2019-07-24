using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Tiles
{
	public class AerialiteBrick : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = false;
			Main.tileBlockLight[Type] = true;
            TileID.Sets.NeedsGrassFraming[Type] = true;
            soundType = 21;
            minPick = 50;
            drop = mod.ItemType("AerialiteBrick");
			AddMapEntry(new Color(68, 58, 145));
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 1, 0f, 0f, 1, new Color(119, 102, 255), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 10, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }
    }
}