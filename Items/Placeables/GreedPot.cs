using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Tiles;

namespace CalamityMod.Items.Placeables
{
    public class GreedPot : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GreedPotTile>());
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.buyPrice(gold: 5); // Sold by Shady Salesman
        }
    }
}
