using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using CalamityMod.CalPlayer.Dashes;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("StatisBeltOfCurses")]
    public class StatisVoidSash : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Statis' Void Sash");
            Tooltip.SetDefault("12% increased jump speed and allows constant jumping\n" +
                "Grants immunity to fall damage\n" +
                "Can climb walls, dash, and dodge attacks\n" +
                "The dodge has a 90 second cooldown\n" +
                "This cooldown is shared with all other dodges and reflects\n" +
                "Dashes leave homing scythes in your wake");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(8, 3));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            player.autoJump = true;
            player.jumpSpeedBoost += 0.6f;
            player.noFallDmg = true;
            player.blackBelt = true;
            modPlayer.DashID = StatisVoidSashDash.ID;
            player.dashType = 0;
            player.spikedBoots = 2;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<StatisNinjaBelt>().
                AddIngredient<TwistingNether>(10).
                AddIngredient<NightmareFuel>(20).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
