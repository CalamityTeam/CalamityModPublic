using CalamityMod.Items.Materials;
using CalamityMod.Items.Potions;
using CalamityMod.Tiles.Astral;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.Furniture
{
    public class AstralBeaconItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Beacon");
            Tooltip.SetDefault("Summons Astrum Deus in exchange for specific offerings");
        }
        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 26;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 14;
            item.rare = ItemRarityID.Cyan;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.createTile = ModContent.TileType<AstralBeacon>();
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AstralJelly>(), 5);
            recipe.AddIngredient(ModContent.ItemType<Stardust>(), 20);
            recipe.AddIngredient(ModContent.ItemType<AstralStone>(), 30);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
