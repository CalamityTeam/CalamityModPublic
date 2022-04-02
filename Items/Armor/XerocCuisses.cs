using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class XerocCuisses : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Empyrean Cuisses");
            Tooltip.SetDefault("Speed of the cosmos\n" +
                    "5% increased rogue damage and critical strike chance\n" +
                    "15% increased movement speed");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 24, 0, 0);
            item.rare = ItemRarityID.Red;
            item.defense = 24;
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().throwingCrit += 5;
            player.Calamity().throwingDamage += 0.05f;
            player.moveSpeed += 0.15f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<MeldiateBar>(), 18);
            recipe.AddIngredient(ItemID.LunarBar, 12);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
