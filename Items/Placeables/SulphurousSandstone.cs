using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables
{
    public class SulphurousSandstone : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<SulphurousSand>();
        }

        public override void SetDefaults()
        {
            Item.createTile = ModContent.TileType<Tiles.Abyss.SulphurousSandstone>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.width = 13;
            Item.height = 10;
            Item.maxStack = 9999;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Walls.SulphurousSandstoneWall>(4).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
