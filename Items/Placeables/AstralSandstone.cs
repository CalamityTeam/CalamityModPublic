using CalamityMod.Items.Placeables.Walls;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace CalamityMod.Items.Placeables
{
    public class AstralSandstone : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
            DisplayName.SetDefault("Astral Sandstone");
        }

        public override void SetDefaults()
        {
            Item.createTile = ModContent.TileType<Tiles.AstralDesert.AstralSandstone>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AstralSandstoneWall>(4).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
