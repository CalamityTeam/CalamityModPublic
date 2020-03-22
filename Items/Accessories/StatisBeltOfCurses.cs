using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class StatisBeltOfCurses : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Statis' Belt of Curses");
            Tooltip.SetDefault("Increases jump speed and allows constant jumping\n" +
                "Can climb walls, dash, and dodge attacks\n" +
                "Toggle visibility of this accessory to enable/disable the dash");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 32;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.accessory = true;
            item.Calamity().postMoonLordRarity = 13;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            player.autoJump = true;
            player.jumpSpeedBoost += 1.2f;
            player.extraFall += 50;
            player.blackBelt = true;
            if (!hideVisual)
				player.dash = 1;
            player.spikedBoots = 2;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<StatisNinjaBelt>());
            recipe.AddIngredient(ModContent.ItemType<TwistingNether>(), 10);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
