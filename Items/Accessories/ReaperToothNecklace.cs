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
            Item.width = 44;
            Item.height = 50;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.allDamage += 0.15f;
            player.armorPenetration += 15;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<SandSharkToothNecklace>()).AddIngredient(ModContent.ItemType<ReaperTooth>(), 6).AddIngredient(ModContent.ItemType<Lumenite>(), 15).AddIngredient(ModContent.ItemType<DepthCells>(), 15).AddIngredient(ModContent.ItemType<Tenebris>(), 5).AddTile(TileID.TinkerersWorkbench).Register();
        }
    }
}
