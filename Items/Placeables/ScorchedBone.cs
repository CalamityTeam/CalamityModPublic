using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Walls;
using CalamityMod.Tiles.Crags;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    public class ScorchedBone : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 100;
            DisplayName.SetDefault("Scorched Bone");
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
            Item.rare = ItemRarityID.Orange;
            Item.createTile = ModContent.TileType<Tiles.Crags.ScorchedBone>();
        }

        /*public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.Wood;
        }*/

        /*public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AcidwoodPlatform>(2).
                Register();

            CreateRecipe(1).
            AddIngredient(ModContent.ItemType<AcidwoodWallItem>(), 4).
            AddTile(TileID.WorkBenches).Register();
        }*/
    }
}
