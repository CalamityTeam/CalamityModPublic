using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class OmegaBlueChestplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Omega Blue Chestplate");
            Tooltip.SetDefault(@"12% increased damage and 8% increased critical strike chance
Your attacks inflict Crush Depth
No positive life regen");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.sellPrice(0, 38, 0, 0);
            item.rare = ItemRarityID.Red;
            item.defense = 28;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override void UpdateEquip(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            player.allDamage += 0.12f;
            modPlayer.AllCritBoost(8);
            modPlayer.omegaBlueChestplate = true;
            modPlayer.noLifeRegen = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ReaperTooth>(), 12);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 8);
            recipe.AddIngredient(ModContent.ItemType<Tenebris>(), 8);
            recipe.AddIngredient(ModContent.ItemType<RuinousSoul>(), 4);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
