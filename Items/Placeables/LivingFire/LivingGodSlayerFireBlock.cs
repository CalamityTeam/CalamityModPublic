using CalamityMod.Items.Materials;
using CalamityMod.Tiles.LivingFire;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.LivingFire
{
    public class LivingGodSlayerFireBlock : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Living God Slayer Fire Block");
        }

        public override void SetDefaults()
        {
            item.width = 10;
            item.height = 12;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.createTile = ModContent.TileType<LivingGodSlayerFireBlockTile>();
        }

        public override void PostUpdate()
        {
            Lighting.AddLight((int)((item.position.X + item.width / 2) / 16f), (int)((item.position.Y + item.height / 2) / 16f), 1f, 0f, 1f);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.LivingFireBlock, 20);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>());
            recipe.SetResult(this, 20);
            recipe.AddRecipe();
        }
    }
}
