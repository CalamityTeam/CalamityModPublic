using CalamityMod.Items.Materials;
using CalamityMod.Tiles;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    public class SCalAltarItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Altar of the Accursed");
            Tooltip.SetDefault("Offer Ashes of Calamity at this altar to summon the Witch\n" +
                "Doing so will create a square arena of blocks, with the altar at its center\n" +
                "During the battle, heart pickups only heal for half as much");
        }

        public override void SetDefaults()
        {
            item.createTile = ModContent.TileType<SCalAltar>();
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 60;
            item.height = 48;
            item.maxStack = 999;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BrimstoneSlag>(), 30);
            recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<CoreofCalamity>(), 1);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
