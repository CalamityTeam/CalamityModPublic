using CalamityMod.Items.Placeables.FurnitureAcidwood;
using CalamityMod.Tiles.Abyss;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Placeables
{
    public class Acidwood : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
            DisplayName.SetDefault("Acidwood");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 22;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<AcidwoodTile>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AcidwoodPlatform>(2).
                Register();
        }
    }
}
