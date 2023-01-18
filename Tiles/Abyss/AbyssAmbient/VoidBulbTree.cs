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
	public class VoidBulbTree : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileSolid[Type] = false;
			TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.Height = 4;
			TileObjectData.newTile.Origin = new Point16(2, 3);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
			TileObjectData.newTile.StyleWrapLimit = 36;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
            AddMapEntry(new Color(59, 72, 168));
            DustType = 33;
            HitSound = SoundID.Dig;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.15f;
            g = 0.3f;
            b = 0.5f;
        }
	}
}