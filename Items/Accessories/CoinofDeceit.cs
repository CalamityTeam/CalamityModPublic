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
            item.width = 20;
            item.height = 22;
            item.value = CalamityGlobalItem.Rarity1BuyPrice;
            item.accessory = true;
            item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().throwingCrit += 6;
            player.Calamity().stealthStrike75Cost = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("AnyGoldBar", 4);
            recipe.AddRecipeGroup("AnyCopperBar", 8);
            // So you make fake coins out of wood, nobody will judge you
            recipe.AddIngredient(ModContent.ItemType<Acidwood>(), 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
