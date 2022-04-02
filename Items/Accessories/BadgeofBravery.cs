using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class BadgeofBravery : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Badge of Bravery");
            Tooltip.SetDefault("15% increased melee speed\n" +
                               "Increases melee damage and melee crit by 5%\n" +
                               "+5 armor penetration");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.rare = ItemRarityID.Purple;
            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.accessory = true;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.badgeOfBravery = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.FeralClaws);
            recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 2);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
