using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Back)]
    public class GiantShell : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        public static int DefenseBoost = 4;
        public static float DashVelocityMult = 0.9f;
        public static int PostHitCancelDuration = CalamityUtils.SecondsToFrames(3);
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs((1f - DashVelocityMult).ToPercent(), PostHitCancelDuration.FramesToSeconds());

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.defense = DefenseBoost;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.gShell = true;
        }
    }
}
