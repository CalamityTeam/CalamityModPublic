using CalamityMod.Items.Materials;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class StatisNinjaBelt : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 6));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 26;
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
