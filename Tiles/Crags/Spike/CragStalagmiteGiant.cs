using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Tiles.Crags.Spike
{
	public class CragStalagmiteGiant1 : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileSolid[Type] = false;
			TileObjectData.newTile.Width = 4;
			TileObjectData.newTile.Height = 8;
			TileObjectData.newTile.Origin = new Point16(2, 7);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16, 16, 16 };
			TileObjectData.newTile.StyleWrapLimit = 36;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.DrawYOffset = 3;
			TileObjectData.addTile(Type);
            AddMapEntry(new Color(43, 23, 46));
            DustType = 155;
            HitSound = SoundID.Tink;
		}
	}

	public class CragStalagmiteGiant2 : CragStalagmiteGiant1
	{
	}

	public class CragStalagmiteGiant3 : CragStalagmiteGiant1
	{
	}
}