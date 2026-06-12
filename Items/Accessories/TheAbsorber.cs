using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class TheAbsorber : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        public static float MoveSpeedBoost = 0.12f;
        public static float JumpSpeedBoost = 0.6f; // Both 12% so we only need just one in the tooltip
        public static int ThornsDamage => CalamityUtils.ScaleWithDifficulty(200);
        public static int AuraLifetime = 1800;
        public static int AuraRegenBoost = 4;
        public static float AuraDamageBoost = 0.08f;
        public static float AuraDamageReductionBoost = 0.05f;
        public static float DamageTakenHealedPercent = 0.05f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MoveSpeedBoost.ToPercent(), AuraLifetime.FramesToSeconds(),
        AuraRegenBoost.ToRegenPerSecond(), AuraDamageBoost.ToPercent(), AuraDamageReductionBoost.ToPercent(), DamageTakenHealedPercent.ToPercent());

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 24;
            Item.defense = 6;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            player.noKnockback = true; // Inherited from Giant Tortoise Shell
            modPlayer.absorber = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<GrandGelatin>().
                AddIngredient<Baroclaw>().
                AddIngredient<GiantTortoiseShell>().
                AddIngredient<MolluskHusk>(5).
                AddIngredient<MeldBlob>(14). //Consistent with Vanilla lunar wing costs
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
