using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
	public class DarkSunRing : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Sun Ring");
            Tooltip.SetDefault("Contains the power of the dark sun\n" +
				"12% increase to damage and melee speed\n" +
                "+1 life regen, 15% increased pick speed and +2 max minions\n" +
                "Increased minion knockback\n" +
                "During the day the player has +3 life regen\n" +
                "During the night the player has +15 defense\n" +
				"Both of these bonuses are granted during an eclipse");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 5));
        }

        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 28;
            item.value = CalamityGlobalItem.Rarity14BuyPrice;
			item.rare = ItemRarityID.Purple;
            item.defense = 10;
            item.lifeRegen = 1;
            item.accessory = true;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
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
            recipe.AddIngredient(ModContent.ItemType<DarksunFragment>(), 50);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
