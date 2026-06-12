using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class GiantTortoiseShell : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        public static int DefenseBoost = 6;
        public static float DamageReductionBoost = 0.1f;
        public static float DashVelocityMult = 0.9f;
        public static int PostHitCancelDuration = CalamityUtils.SecondsToFrames(3);
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageReductionBoost.ToPercent(), (1f - DashVelocityMult).ToPercent(), PostHitCancelDuration.FramesToSeconds());

        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 30;
            Item.defense = DefenseBoost;
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            player.noKnockback = true;
            modPlayer.tortShell = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<GiantShell>().
                AddIngredient(ItemID.TurtleShell).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
