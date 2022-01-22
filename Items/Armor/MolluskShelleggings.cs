using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class MolluskShelleggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mollusk Shelleggings");
            Tooltip.SetDefault("12% increased damage and 4% increased critical strike chance\n" +
                               "7% decreased movement speed");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 18;
            item.value = Item.buyPrice(0, 15, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.defense = 15;
        }

        public override void UpdateEquip(Player player)
        {
            player.allDamage += 0.12f;
            player.Calamity().AllCritBoost(4);
            player.moveSpeed -= 0.07f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<MolluskHusk>(), 10);
            recipe.AddIngredient(ModContent.ItemType<SeaPrism>(), 20);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
