using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class MolluskShellplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mollusk Shellplate");
            Tooltip.SetDefault("10% increased damage and 6% increased critical strike chance\n" +
                               "15% decreased movement speed");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 22;
            item.value = Item.buyPrice(0, 20, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.defense = 22;
        }

        public override void UpdateEquip(Player player)
        {
            player.allDamage += 0.1f;
            player.Calamity().AllCritBoost(6);
            player.moveSpeed -= 0.15f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<MolluskHusk>(), 15);
            recipe.AddIngredient(ModContent.ItemType<SeaPrism>(), 25);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
