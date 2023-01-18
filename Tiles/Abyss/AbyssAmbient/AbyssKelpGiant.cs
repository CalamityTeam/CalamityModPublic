using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Tiles.Abyss.AbyssAmbient
{
	public class AbyssKelpGiant : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileSolid[Type] = false;
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 6;
			TileObjectData.newTile.Origin = new Point16(1, 5);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16 };
			TileObjectData.newTile.StyleWrapLimit = 36;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
            AddMapEntry(new Color(0, 80, 0));
            DustType = 2;
            HitSound = SoundID.Grass;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.42f;
            g = 0.35f;
            b = 0f;
        }
	}
}