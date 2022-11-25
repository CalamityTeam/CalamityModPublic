using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture.CraftingStations
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
                "The Witch enrages while you are outside of the arena\n" +
				"Used for special crafting");
        }

        public override void SetDefaults()
        {
            Item.createTile = ModContent.TileType<SCalAltar>();
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.width = 38;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ModContent.RarityType<Violet>();
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.CraftingObjects;
		}

        public override void AddRecipes()
        {
            CreateRecipe().
				AddIngredient<BrimstoneSlag>(30).
				AddIngredient<AuricBar>(5).
				AddIngredient<CoreofCalamity>().
				AddTile<CosmicAnvil>().
				Register();
        }
    }
}
