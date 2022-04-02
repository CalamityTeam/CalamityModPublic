using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class ReaperToothNecklace : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reaper Tooth Necklace");
            Tooltip.SetDefault("A grisly trophy from the ultimate predator\n" + "15% increased damage\n" + "Increases armor penetration by 15");
        }

        public override void SetDefaults()
        {
            item.width = 44;
            item.height = 50;
            item.accessory = true;
            item.value = CalamityGlobalItem.Rarity13BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.allDamage += 0.15f;
            player.armorPenetration += 15;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SandSharkToothNecklace>());
            recipe.AddIngredient(ModContent.ItemType<ReaperTooth>(), 6);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 15);
            recipe.AddIngredient(ModContent.ItemType<DepthCells>(), 15);
            recipe.AddIngredient(ModContent.ItemType<Tenebris>(), 5);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
