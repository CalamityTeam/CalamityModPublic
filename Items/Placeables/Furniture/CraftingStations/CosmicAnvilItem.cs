using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture.CraftingStations
{
    public class CosmicAnvilItem : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<CosmicAnvil>());
            Item.value = Item.sellPrice(gold: 50);
            Item.rare = ModContent.RarityType<CosmicPurple>();
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.CraftingObjects;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("HardmodeAnvil").
                AddIngredient<CosmiliteBar>(10).
                AddIngredient(ItemID.LunarBar, 10).
                AddIngredient(ItemID.FragmentSolar, 12).
                AddIngredient(ItemID.FragmentVortex, 12).
                AddIngredient(ItemID.FragmentNebula, 12).
                AddIngredient(ItemID.FragmentStardust, 12).
                AddIngredient<MeldBlob>(12).
                AddIngredient<ExodiumCluster>(20).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
