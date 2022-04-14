using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Placeables.Furniture.CraftingStations
{
    public class CosmicAnvilItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Cosmic Anvil");
            Tooltip.SetDefault("An otherworldly anvil capable of withstanding the pressures of stellar collapse\n" +
                "Also functions as every previous tier of anvil");
        }

        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 32;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<CosmicAnvil>();

            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
            Item.value = Item.sellPrice(platinum: 2, gold: 50);
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddRecipeGroup("HardmodeAnvil").AddIngredient(ModContent.ItemType<CosmiliteBar>(), 10).AddIngredient(ItemID.LunarBar, 10).AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 12).AddIngredient(ModContent.ItemType<ExodiumClusterOre>(), 20).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
