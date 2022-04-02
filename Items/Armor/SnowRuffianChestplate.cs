using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class SnowRuffianChestplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Snow Ruffian Chestplate");
            Tooltip.SetDefault("3% increased rogue critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 0, 75, 0);
            item.rare = ItemRarityID.Blue;
            item.defense = 2; //4
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().throwingCrit += 3;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("AnySnowBlock", 30);
            recipe.AddRecipeGroup("AnyIceBlock", 15);
            recipe.AddIngredient(ItemID.BorealWood, 45);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
