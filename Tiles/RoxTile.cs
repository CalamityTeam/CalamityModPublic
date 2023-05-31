using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles
{
    public class RoxTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileSpelunker[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };

            TileObjectData.addTile(Type);

            AddMapEntry(new Color(240, 77, 7), CalamityUtils.GetItemName<Roxcalibur>());
            TileID.Sets.DisableSmartCursor[Type] = true;
            RegisterItemDrop(ModContent.ItemType<Roxcalibur>());
            FlexibleTileWand.RubblePlacementLarge.AddVariations(ModContent.ItemType<Roxcalibur>(), Type, 0);
        }
    }
}
