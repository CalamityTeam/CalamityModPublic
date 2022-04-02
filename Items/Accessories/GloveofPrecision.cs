using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(new EquipType[] { EquipType.HandsOn, EquipType.HandsOff } )]
    public class GloveOfPrecision : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Glove of Precision");
            Tooltip.SetDefault("Decreases rogue attack speed by 20% but increases damage and crit by 12% and velocity by 25%");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 40;
            item.value = CalamityGlobalItem.Rarity7BuyPrice;
            item.accessory = true;
            item.rare = ItemRarityID.Lime;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.gloveOfPrecision = true;
            modPlayer.throwingDamage += 0.12f;
            modPlayer.throwingCrit += 12;
            modPlayer.throwingVelocity += 0.25f;
            modPlayer.rogueUseSpeedFactor -= 0.2f;
        }
    }
}
