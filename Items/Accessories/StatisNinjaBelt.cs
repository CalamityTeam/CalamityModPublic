using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class StatisNinjaBelt : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Statis' Ninja Belt");
            Tooltip.SetDefault("12% increased jump speed and allows constant jumping\n" +
				"Increased fall damage resistance by 35 blocks\n" +
                "Can climb walls, dash, and dodge attacks");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 32;
            item.value = CalamityGlobalItem.Rarity7BuyPrice;
            item.rare = 7;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            player.autoJump = true;
            player.jumpSpeedBoost += 0.6f;
            player.extraFall += 35;
            player.blackBelt = true;
			player.dash = 1;
            player.spikedBoots = 2;
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
