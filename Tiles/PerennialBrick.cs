using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Capture;

namespace CalamityMod.Tiles
{
	public class PerennialBrick : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = false;
			Main.tileBlockLight[Type] = true;
			drop = mod.ItemType("PerennialBrick");
			soundType = 21;
			minPick = 150;
			AddMapEntry(new Color(17, 133, 46));
			animationFrameHeight = 90;
		}

		public override bool CreateDust(int i, int j, ref int type)
		{
			Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 128, 0f, 0f, 1, new Color(255, 255, 255), 1f);
			Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 60, 0f, 0f, 1, new Color(255, 255, 255), 1f);
			return false;
		}

		public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
		{
			int xPos = i % 4;
			int yPos = j % 4;
			frameXOffset = xPos * 234;
			frameYOffset = yPos * animationFrameHeight;
		}
	}
}