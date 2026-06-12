using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("SpiritGenerator")]
    public class SpiritGlyph : ModItem, ILocalizedModType, IHoldShiftTooltipItem
    {
        public new string LocalizationCategory => "Items.Accessories";

        public static int RegenBoost = 1;
        public static int DefenseBoost = 2;
        public static float SummonDamageBoost = 0.1f;
        public LocalizedText TooltipExtensionText => this.GetLocalization("HoldShiftTooltip").WithFormatArgs(SummonDamageBoost.ToPercent(), DefenseBoost, RegenBoost.ToRegenPerSecond());
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 30;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.sGlyph = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Diamond, 5).
                AddIngredient(ItemID.Obsidian, 15).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
