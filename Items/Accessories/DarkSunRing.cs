using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using CalamityMod.Tiles.Furniture.CraftingStations;

namespace CalamityMod.Items.Accessories
{
	public class DarkSunRing : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Sun Ring");
            Tooltip.SetDefault("Contains the power of the dark sun\n" +
				"12% increase to damage and melee speed\n" +
                "+1 life regen, 15% increased pick speed, and +2 max minions\n" +
                "Increased minion knockback\n" +
                "During the day the player has +3 life regen\n" +
                "During the night the player has +30 defense\n" +
				"Both of these bonuses are granted during an eclipse");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 6));
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = CalamityGlobalItem.Rarity15BuyPrice;
            item.defense = 10;
            item.lifeRegen = 1;
            item.accessory = true;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.darkSunRing = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 10);
            recipe.AddIngredient(ModContent.ItemType<DarksunFragment>(), 100);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
