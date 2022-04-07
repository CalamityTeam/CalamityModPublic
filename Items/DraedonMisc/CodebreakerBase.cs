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
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.width = 42;
            Item.height = 32;
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Orange;
            Item.createTile = ModContent.TileType<CodebreakerTile>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ChargingStationItem>().
                AddIngredient<MysteriousCircuitry>(20).
                AddIngredient<DubiousPlating>(35).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
