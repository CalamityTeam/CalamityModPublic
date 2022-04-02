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
            Tooltip.SetDefault("12% increased jump speed and allows constant jumping\n" +
                "Grants immunity to fall damage\n" +
                "Can climb walls, dash, and dodge attacks\n" +
                "The dodge has a 90 second cooldown\n" +
                "This cooldown is shared with all other dodges and reflects\n" +
                "Dashes leave homing scythes in your wake");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(8, 3));
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 32;
            item.accessory = true;
            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            player.autoJump = true;
            player.jumpSpeedBoost += 0.6f;
            player.noFallDmg = true;
            player.blackBelt = true;
            modPlayer.dashMod = 7;
            player.dash = 0;
            player.spikedBoots = 2;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<StatisNinjaBelt>());
            recipe.AddIngredient(ModContent.ItemType<TwistingNether>(), 10);
            // This is not a mistake. Requiring only Nightmare Fuel is intentional for thematics.
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 20);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
