using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class SandCloak : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        public static int SandVeilDuration = CalamityUtils.SecondsToFrames(15);
        public static int SandVeilDefenseBoost = 3;
        public static float SandVeilAccelerationBoost = 0.75f;
        public static int SandVeilCooldown = CalamityUtils.SecondsToFrames(15);
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SandVeilDuration.FramesToSeconds(), SandVeilDefenseBoost, SandVeilAccelerationBoost.ToPercent(), SandVeilCooldown.FramesToSeconds());
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 44;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().sandCloak = true;
        }
    }
}
