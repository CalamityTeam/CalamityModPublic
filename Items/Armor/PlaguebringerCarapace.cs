using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class PlaguebringerCarapace : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plaguebringer Carapace");
            Tooltip.SetDefault("Reduces the damage caused to you by the plague\n" +
                "12% increased minion damage\n" +
                "Friendly bees inflict the plague");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.defense = 17;
            item.value = CalamityGlobalItem.Rarity8BuyPrice;
            item.rare = ItemRarityID.Yellow;
            item.Calamity().donorItem = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().plaguebringerCarapace = true;
            player.minionDamage += 0.12f;
            player.Calamity().reducedPlagueDmg = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BeeBreastplate);
            recipe.AddIngredient(ModContent.ItemType<AlchemicalFlask>(), 2);
            recipe.AddIngredient(ModContent.ItemType<PlagueCellCluster>(), 7);
            recipe.AddIngredient(ModContent.ItemType<InfectedArmorPlating>(), 7);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
