using CalamityMod.Items.Placeables.Furniture.DevPaintings;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Furniture.DevPaintings
{
	public class NincityPaintingTile : ModTile
	{
        public override void SetStaticDefaults()
        {
			this.SetUp6x6Painting();
			DustType = 7;
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Developer Painting");
			AddMapEntry(new Color(120, 85, 60), name);
            TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.FramesOnKillWall[Type] = true;
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 96, 96, ModContent.ItemType<NincityPainting>());
		}
	}
}
