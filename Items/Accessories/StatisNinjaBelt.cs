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
            Tooltip.SetDefault("6% increased jump speed and allows constant jumping\n" +
                "Increased fall damage resistance by 35 blocks\n" +
                "Can climb walls, dash, and dodge attacks\n" +
                "The dodge has a 90 second cooldown\n" +
                "This cooldown is shared with all other dodges and reflects");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 32;
            item.value = CalamityGlobalItem.Rarity13BuyPrice;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
            item.rare = ItemRarityID.Purple;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.autoJump = true;
            player.jumpSpeedBoost += 0.3f;
            player.extraFall += 35;
            player.blackBelt = true;
            player.dash = 1;
            player.Calamity().dashMod = 0;
            player.spikedBoots = 2;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.MasterNinjaGear);
            recipe.AddIngredient(ItemID.FrogLeg);
            recipe.AddIngredient(ModContent.ItemType<PurifiedGel>(), 50);
            recipe.AddIngredient(ModContent.ItemType<Phantoplasm>(), 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
