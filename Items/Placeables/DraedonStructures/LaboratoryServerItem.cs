using CalamityMod.Items.DraedonMisc;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.DraedonStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.DraedonStructures
{
    public class LaboratoryServerItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Laboratory Server");
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
            Item.createTile = ModContent.TileType<LaboratoryServer>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<Items.Placeables.DraedonStructures.LaboratoryPlating>(), 12).AddIngredient(ModContent.ItemType<DubiousPlating>()).AddIngredient(ModContent.ItemType<PowerCell>(), 15).AddTile(TileID.Anvils).Register();
        }
    }
}
