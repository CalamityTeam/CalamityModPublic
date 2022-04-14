using CalamityMod.Items.Placeables.Walls;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Placeables.Plates
{
    public class Navyplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Navyplate");
            Tooltip.SetDefault("It resonates with otherworldly energy.");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }

        public override void SetDefaults()
        {
            Item.createTile = ModContent.TileType<Tiles.Plates.Navyplate>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.width = 13;
            Item.height = 10;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(silver: 3);
            Item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            CreateRecipe(3).AddIngredient(ModContent.ItemType<SeaPrism>(), 1).AddIngredient(ItemID.Obsidian, 3).AddTile(TileID.Hellforge).Register();
            CreateRecipe(1).AddIngredient(ModContent.ItemType<NavyplateWall>(), 4).AddTile(TileID.WorkBenches).Register();
        }
    }
}
