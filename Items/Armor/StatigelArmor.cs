using Terraria;
using Terraria.ModLoader; using CalamityMod.Items.Materials;
using Terraria.ID;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class StatigelArmor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Statigel Armor");
            Tooltip.SetDefault("5% increased critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = 4;
            item.defense = 10;
        }

        public override void UpdateEquip(Player player)
        {
            player.meleeCrit += 5;
            player.magicCrit += 5;
            player.rangedCrit += 5;
            player.Calamity().throwingCrit += 5;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<PurifiedGel>(), 8);
            recipe.AddIngredient(ItemID.HellstoneBar, 13);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
