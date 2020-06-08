using CalamityMod.Items.Placeables.Furniture.DevPaintings;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Furniture.DevPaintings
{
    public class NincityPaintingTile : ModTile
    {
        public override void SetDefaults()
        {
            this.SetUp6x6Painting();
            dustType = 7;
            disableSmartCursor = true;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Nincity Painting");
            AddMapEntry(new Color(120, 85, 60), name);
			TileID.Sets.FramesOnKillWall[Type] = true;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 96, 96, ModContent.ItemType<NincityPainting>());
        }
    }
}
