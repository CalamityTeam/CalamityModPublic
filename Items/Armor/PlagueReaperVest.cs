using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class PlagueReaperVest : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plague Reaper Vest");
            Tooltip.SetDefault("Reduces the damage caused to you by the plague\n" +
                "15% increased ranged damage and 5% increased ranged critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 24, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.defense = 14;
        }

        public override void UpdateEquip(Player player)
        {
            player.rangedDamage += 0.15f;
            player.rangedCrit += 5;
            player.Calamity().reducedPlagueDmg = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.NecroBreastplate); //I will instead change the position of the necro armor get trolled
            recipe.AddIngredient(ModContent.ItemType<PlagueCellCluster>(), 29); //I like prime numbers =)
            recipe.AddIngredient(ItemID.Nanites, 19); //Change this to 30 and 20 and I will hunt you down
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
