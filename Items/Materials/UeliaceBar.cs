using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class UeliaceBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Uelibloom Bar");
        }

        public override void SetDefaults()
        {
            Item.createTile = ModContent.TileType<UelibloomBar>();
            Item.width = 15;
            Item.height = 12;
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(gold: 3);
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<UelibloomOre>(5).
                AddTile(TileID.AdamantiteForge).
                Register();
        }
    }
}
