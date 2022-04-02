using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class UmbraphileBoots : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Umbraphile Boots");
            Tooltip.SetDefault("9% increased rogue damage and 6% increased rogue crit\n" +
                               "20% increased movement speed");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 18;
            item.value = Item.buyPrice(0, 18, 0, 0);
            item.rare = ItemRarityID.Lime;
            item.defense = 12;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.2f;
            player.Calamity().throwingDamage += 0.09f;
            player.Calamity().throwingCrit += 6;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SolarVeil>(), 14);
            recipe.AddIngredient(ItemID.HallowedBar, 11);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
