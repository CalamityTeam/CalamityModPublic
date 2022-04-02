using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class UmbraphileRegalia : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Umbraphile Regalia");
            Tooltip.SetDefault("10% increased rogue damage and 10% increased rogue crit");
        }

        public override void SetDefaults()
        {
            item.width = 38;
            item.height = 24;
            item.value = Item.buyPrice(0, 24, 0, 0);
            item.rare = ItemRarityID.Lime;
            item.defense = 16;
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().throwingDamage += 0.1f;
            player.Calamity().throwingCrit += 10;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SolarVeil>(), 18);
            recipe.AddIngredient(ItemID.HallowedBar, 15);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
