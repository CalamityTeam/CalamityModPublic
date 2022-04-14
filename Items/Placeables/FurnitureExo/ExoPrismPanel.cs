using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Tiles.FurnitureExo;
using CalamityMod.Items.Placeables.Walls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
namespace CalamityMod.Items.Placeables.FurnitureExo
{
    public class ExoPrismPanel : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
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
            Item.createTile = ModContent.TileType<ExoPrismPanelTile>();
            Item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Placeables/FurnitureExo/ExoPrismPanel_Glow").Value);
        }

        public override void AddRecipes()
        {
            CreateRecipe(200).AddIngredient(ItemID.Glass, 200).AddIngredient(ModContent.ItemType<ExoPrism>()).AddTile(ModContent.TileType<DraedonsForge>()).Register();
            CreateRecipe(1).AddIngredient(ModContent.ItemType<ExoPrismPlatform>(), 2).Register();
            CreateRecipe(1).AddIngredient(ModContent.ItemType<ExoPrismPanelWallItem>(), 4).AddTile(TileID.WorkBenches).Register();
        }
    }
}
