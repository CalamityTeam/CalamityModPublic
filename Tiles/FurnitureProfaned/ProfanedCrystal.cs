using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureProfaned
{
	public class ProfanedCrystal : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;
            soundType = 13;
            mineResist = 1f;
            minPick = 225;
            drop = mod.ItemType("ProfanedCrystal");
			AddMapEntry(new Color(181, 136, 177));
            animationFrameHeight = 270;
        }
        int animationFrameWidth = 288;

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 205, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameXOffset = (i % 2) * animationFrameWidth;
            frameYOffset = (j % 3) * animationFrameHeight;
        }
    }
}