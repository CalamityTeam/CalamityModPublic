using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class HallowedRune : ModItem, ILocalizedModType, IHoldShiftTooltipItem
    {
        public new string LocalizationCategory => "Items.Accessories";

        public static int RegenBoost = 2;
        public static int DefenseBoost = 5;
        public static float SummonDamageBoost = 0.1f;
        public LocalizedText TooltipExtensionText => this.GetLocalization("HoldShiftTooltip").WithFormatArgs(SummonDamageBoost.ToPercent(), DefenseBoost, RegenBoost.ToRegenPerSecond());
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().hallowedRune = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SpiritGlyph>().
                AddIngredient(ItemID.HallowedBar, 18).
                AddIngredient(ItemID.SoulofFright, 5).
                AddIngredient(ItemID.SoulofMight, 5).
                AddIngredient(ItemID.SoulofSight, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
