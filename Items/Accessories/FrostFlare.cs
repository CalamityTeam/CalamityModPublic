using CalamityMod.CalPlayer;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class FrostFlare : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        public static float HealthRatioThreshold = 0.5f;
        public static float DamageBoost = 0.1f;
        public static int DefenseBoost = 10;
        public static float AccelerationBoost = 0.15f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(HealthRatioThreshold.ToPercent(), DamageBoost.ToPercent(), DefenseBoost, AccelerationBoost.ToPercent());

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 4));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 24;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.frostFlare = true;
        }
    }
}
