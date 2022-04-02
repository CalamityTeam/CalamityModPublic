using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class PlagueReaperStriders : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plague Reaper Striders");
            Tooltip.SetDefault("3% increased ranged critical strike chance\n" +
                "15% increased movement speed");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 18, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.defense = 11;
        }

        public override void UpdateEquip(Player player)
        {
            player.rangedCrit += 3;
            player.moveSpeed += 0.15f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.NecroGreaves);
            recipe.AddIngredient(ModContent.ItemType<PlagueCellCluster>(), 21);
            recipe.AddIngredient(ItemID.Nanites, 17);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
