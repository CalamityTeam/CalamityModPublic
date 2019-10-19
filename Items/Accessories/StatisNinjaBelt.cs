using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class StatisNinjaBelt : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Statis' Ninja Belt");
            Tooltip.SetDefault("Increases jump speed and allows constant jumping\n" +
                "Can climb walls, dash, and dodge attacks\n" +
                "5% increased rogue damage and velocity\n" +
                "5% increased rogue crit chance");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 32;
            item.value = Item.buyPrice(0, 45, 0, 0);
            item.rare = 9;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            player.autoJump = true;
            player.jumpSpeedBoost += 0.4f;
            player.extraFall += 35;
            player.blackBelt = true;
            player.dash = 1;
            player.spikedBoots = 2;
            modPlayer.throwingDamage += 0.05f;
            modPlayer.throwingCrit += 5;
            modPlayer.throwingVelocity += 0.05f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.FrogLeg);
            recipe.AddIngredient(ModContent.ItemType<PurifiedGel>(), 50);
            recipe.AddIngredient(ModContent.ItemType<CoreofEleum>());
            recipe.AddIngredient(ItemID.MasterNinjaGear);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
