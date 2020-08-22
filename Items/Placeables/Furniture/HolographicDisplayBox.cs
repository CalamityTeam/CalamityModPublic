using CalamityMod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture
{
    public class HolographicDisplayBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holographic Display Box");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 32;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.value = Item.buyPrice(0, 5, 0, 0);
            item.rare = 3;
            item.Calamity().customRarity = CalamityRarity.DraedonRust;
            item.createTile = ModContent.TileType<DraedonHologram>();
        }
    }
}
