using CalamityMod.Items.Materials;
using CalamityMod.Tiles.DraedonStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.DraedonStructures
{
    public class LaboratoryDoorItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laboratory Door");
        }

        public override void SetDefaults()
        {
            item.width = 14;
            item.height = 28;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.DraedonRust;
            item.consumable = true;
            item.createTile = ModContent.TileType<LaboratoryDoorClosed>();
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
