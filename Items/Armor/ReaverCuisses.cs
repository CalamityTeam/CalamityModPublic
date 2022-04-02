using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class ReaverCuisses : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reaver Cuisses");
            Tooltip.SetDefault("5% increased critical strike chance\n" +
                "12% increased movement speed");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 18;
            item.value = Item.buyPrice(0, 18, 0, 0);
            item.rare = ItemRarityID.Lime;
            item.defense = 14;
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().AllCritBoost(5);
            player.moveSpeed += 0.12f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<DraedonBar>(), 10);
            recipe.AddIngredient(ItemID.JungleSpores, 8);
            recipe.AddIngredient(ModContent.ItemType<EssenceofCinder>(), 2);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
