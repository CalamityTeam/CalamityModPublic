using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class CoinofDeceit : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Coin of Deceit");
            Tooltip.SetDefault("Stealth strikes only expend 75% of your max stealth\n" +
            "6% increased rogue crit chance");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 22;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().throwingCrit += 6;
            player.Calamity().stealthStrike75Cost = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("AnyGoldBar", 4)
                .AddRecipeGroup("AnyCopperBar", 8)
                .AddIngredient<Acidwood>(5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
