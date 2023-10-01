using CalamityMod.Items.Placeables.FurnitureAcidwood;
using CalamityMod.Tiles.Abyss;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables.Walls;

namespace CalamityMod.Items.Placeables
{
    public class Acidwood : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.Wood;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 22;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<AcidwoodTile>();
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.Wood;
		}

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AcidwoodPlatform>(2).
                Register();
            CreateRecipe().
                AddIngredient<AcidwoodWallItem>(4).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
