using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class StatisNinjaBelt : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.autoJump = true;
            player.jumpSpeedBoost += 0.3f;
            player.extraFall += 35;
            player.blackBelt = true;
            player.dashType = 1;
            player.Calamity().DashID = string.Empty;
            player.spikedBoots = 2;
            player.accFlipper = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.MasterNinjaGear).
                AddIngredient(ItemID.FrogGear).
                AddIngredient<PurifiedGel>(50).
                AddIngredient<Polterplasm>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
