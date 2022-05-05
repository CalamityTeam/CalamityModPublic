using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Placeables
{
    public class BrimstoneSlag : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
            DisplayName.SetDefault("Brimstone Slag");
        }

        public override void SetDefaults()
        {
            Item.createTile = ModContent.TileType<Tiles.Crags.BrimstoneSlag>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.width = 13;
            Item.height = 10;
            Item.maxStack = 999;
            Item.value = 1000;
            Item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BrimstoneSlagWall>(4).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
