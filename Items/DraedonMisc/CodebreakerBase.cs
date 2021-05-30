using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.DraedonSummoner;
using CalamityMod.Items.Placeables.DraedonStructures;

namespace CalamityMod.Items.DraedonMisc
{
    public class CodebreakerBase : ModItem
    {
        public override void SetStaticDefaults() => DisplayName.SetDefault("Codebreaker Base");

        public override void SetDefaults()
        {
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 42;
            item.height = 32;
            item.maxStack = 999;
            item.rare = ItemRarityID.Orange;
            item.createTile = ModContent.TileType<CodebreakerTile>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ChargingStationItem>());
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 20);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 35);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}
