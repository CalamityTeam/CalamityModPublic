using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class HoneyDew : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        public static float NaturalRegenPower => 1.5f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs((NaturalRegenPower - 1f).ToPercent());
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.honeyDew = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BottledHoney, 10).
                AddIngredient(ItemID.BeeWax, 3).
                AddIngredient(ItemID.JungleSpores, 6).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
