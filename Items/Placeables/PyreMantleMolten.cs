using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader; // If you are using c# 6, you can use: "using static Terraria.Localization.GameCulture;" which would mean you could just write "DisplayName.AddTranslation(German, "");"

namespace CalamityMod.Items.Placeables
{
    public class PyreMantleMolten : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 100;
            DisplayName.SetDefault("Molten Pyre Mantle");
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.Abyss.PyreMantleMolten>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.LavaBucket).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
