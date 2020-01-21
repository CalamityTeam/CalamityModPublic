using CalamityMod.Items.Materials;
using CalamityMod.Items.Potions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture
{
    public class ChaosCandle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chaos Candle");
            Tooltip.SetDefault("The mere presence of this candle enrages surrounding enemies drastically");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 20;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.consumable = true;
            item.value = 500;
            item.createTile = ModContent.TileType<Tiles.Furniture.ChaosCandle>();
			item.holdStyle = 1;
        }

        public override void HoldItem(Player player)
        {
            player.Calamity().chaosCandle = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.WaterCandle, 3);
            recipe.AddIngredient(ItemID.SoulofNight, 3);
            recipe.AddIngredient(ModContent.ItemType<CoreofChaos>(), 2);
            recipe.AddIngredient(ModContent.ItemType<ZergPotion>());
            recipe.SetResult(this);
            recipe.AddTile(18);
            recipe.AddRecipe();
        }
    }
}
