using CalamityMod.Tiles.Furniture;
using CalamityMod.Items.Critters;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Placeables.Furniture
{
    public class BabyFlakCrabCage : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Baby Flak Crab Cage");
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 32;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<BabyFlakCrabCageTile>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Terrarium).
                AddIngredient<BabyFlakCrabItem>().
                Register();
        }
    }
}
