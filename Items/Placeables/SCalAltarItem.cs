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
            DisplayName.SetDefault("Calamitous Altar");
            Tooltip.SetDefault("Death\n" +
                "Allows you to summon Supreme Calamitas with Ashes of Calamity\n" +
                "She creates a large square arena of blocks upon being summoned\n" +
                "When she is summoned the arena will spawn around her with her at the center\n" +
                "During the battle, heart pickups will heal half as much HP");
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
            recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
