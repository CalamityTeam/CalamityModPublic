using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    [LegacyName("UeliaceBar")]
    public class UelibloomBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 25;
            DisplayName.SetDefault("Uelibloom Bar"); // Yoo-luh Bloom
			ItemID.Sets.SortingPriorityMaterials[Type] = 106;
        }

        public override void SetDefaults()
        {
            Item.createTile = ModContent.TileType<Tiles.UelibloomBar>();
            Item.width = 15;
            Item.height = 12;
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(gold: 5);
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
