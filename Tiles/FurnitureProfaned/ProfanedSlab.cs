using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Utilities;

namespace CalamityMod.Tiles.FurnitureProfaned
{
	public class ProfanedSlab : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;

            TileMerge.MergeGeneralTiles(Type);
            TileMerge.MergeDecorativeTiles(Type);
            TileMerge.MergeSmoothTiles(Type);
            TileMerge.MergeTile(Type, mod.TileType("ProfanedRock"));

            soundType = 21;
            mineResist = 10f;
            minPick = 225;
            drop = mod.ItemType("ProfanedSlab");
			AddMapEntry(new Color(122, 66, 59));
            animationFrameHeight = 90;
        }
        int animationFrameWidth = 234;

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, mod.DustType("ProfanedTileRock"), 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameXOffset = i % 4 * animationFrameWidth;
            frameYOffset = j % 4 * animationFrameHeight;
        }
    }
}
