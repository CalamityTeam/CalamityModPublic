using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Accessories
{
    public class DraedonsHeart : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Draedon's Heart");
            Tooltip.SetDefault("Gives 10% increased damage while you have the absolute rage buff\n" +
                "Increases your chance of getting the heart attack debuff\n" +
				"Boosts your damage by 10% and max movement speed and acceleration by 5%\n" +
                "Rage mode does more damage\n" +
                "You gain rage over time\n" +
                "Gives immunity to the horror debuff\n" +
                "Standing still regenerates your life quickly and boosts your defense by 25");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 7));
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.accessory = true;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 15;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.draedonsHeart = true;
            player.buffImmune[mod.BuffType("Horror")] = true;
            modPlayer.draedonsStressGain = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "HeartofDarkness");
            recipe.AddIngredient(null, "StressPills");
            recipe.AddIngredient(null, "Laudanum");
			recipe.AddIngredient(null, "CosmiliteBar", 5);
			recipe.AddIngredient(null, "Phantoplasm", 5);
			recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
