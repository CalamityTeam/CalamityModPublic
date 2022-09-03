using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Tiles.FurnitureSacrilegious;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.FurnitureSacrilegious
{
    [LegacyName("SCalAltarItem")]
    public class AltarOfTheAccursedItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Altar of the Accursed");
            Tooltip.SetDefault("Offer Ashes of Calamity at this altar to summon the Witch\n" +
                "Doing so will create a square arena of blocks, with the altar at its center\n" +
                "During the battle, heart pickups only heal for half as much\n" +
                "The Witch enrages while you are outside of the arena");
        }

        public override void SetDefaults()
        {
            Item.createTile = ModContent.TileType<SCalAltar>();
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.width = 60;
            Item.height = 48;
            Item.maxStack = 999;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ModContent.RarityType<Violet>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).
				AddIngredient(ModContent.ItemType<BrimstoneSlag>(), 30).
				AddIngredient(ModContent.ItemType<AuricBar>(), 5).
				AddIngredient(ModContent.ItemType<CoreofCalamity>(), 1).
				AddTile(ModContent.TileType<CosmicAnvil>()).
				Register();
        }
    }
}
