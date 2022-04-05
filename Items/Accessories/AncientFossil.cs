using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class AncientFossil : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Fossil");
            Tooltip.SetDefault("Increases pick speed by 15% while underground");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight)
            {
                player.pickSpeed -= 0.15f;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("SiltGroup", 100)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}
