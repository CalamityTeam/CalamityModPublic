using CalamityMod.Items.Materials;
using CalamityMod.Tiles.DraedonStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.DraedonStructures
{
    public class ReinforcedCrateItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reinforced Crate");
        }

        public override void SetDefaults()
        {
            item.width = 12;
            item.height = 12;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.createTile = ModContent.TileType<ReinforcedCrate>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Items.Placeables.DraedonStructures.LaboratoryPlating>(), 7);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>());
            recipe.SetResult(this);
            recipe.AddTile(TileID.Anvils);
            recipe.AddRecipe();
        }
    }
}
