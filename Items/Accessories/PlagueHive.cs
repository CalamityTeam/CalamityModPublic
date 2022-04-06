using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class PlagueHive : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plague Hive");
            Tooltip.SetDefault("All attacks inflict the Plague debuff\n" +
                   "Releases bees when damaged that inflict the Plague\n" +
                   "Projectiles spawn plague seekers on enemy hits");
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 48;
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.rare = ItemRarityID.Cyan;
            Item.accessory = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AlchemicalFlask>().
                AddIngredient(ItemID.HoneyComb).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            player.bee = true;
            modPlayer.uberBees = true;
            modPlayer.alchFlask = true;
        }
    }
}
