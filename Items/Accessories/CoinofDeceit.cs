using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class CoinofDeceit : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Coin of Deceit");
            Tooltip.SetDefault("Stealth strikes only expend 90% of your max stealth");
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
            player.Calamity().stealthStrike90Cost = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyGoldBar", 4).
                AddRecipeGroup("AnyEvilBar", 8).
                AddIngredient<WulfrumMetalScrap>(5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
