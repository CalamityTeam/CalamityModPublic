using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
	public class StatisBeltOfCurses : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Statis' Void Sash");
            Tooltip.SetDefault("24% increased jump speed and allows constant jumping\n" +
				"Increases fall damage resistance by 50 blocks\n" +
                "Can climb walls, dash, and dodge attacks\n" +
                "Dashes leave homing scythes in your wake\n" +
                "Toggle visibility of this accessory to enable/disable the dash");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(8, 3));
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 32;
            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.accessory = true;
            item.Calamity().postMoonLordRarity = 14;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            player.autoJump = true;
            player.jumpSpeedBoost += 1.2f;
            player.extraFall += 50;
            player.blackBelt = true;
            if (!hideVisual)
				modPlayer.dashMod = 7;
            player.spikedBoots = 2;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<StatisNinjaBelt>());
            recipe.AddIngredient(ModContent.ItemType<TwistingNether>(), 10);
			//This is not a mistake.  Only Nightmare Fuel is intentional for thematics.
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 10);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
