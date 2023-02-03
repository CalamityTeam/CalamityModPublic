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
            SacrificeTotal = 1;
            DisplayName.SetDefault("Glove of Precision");
            Tooltip.SetDefault("Decreases rogue attack speed by 15% but increases damage by 12%, crit by 15% and velocity by 25%");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 40;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.accessory = true;
            Item.rare = ItemRarityID.Lime;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.gloveOfPrecision = true;
            player.GetDamage<RogueDamageClass>() += 0.12f;
            player.GetCritChance<RogueDamageClass>() += 15;
            modPlayer.rogueVelocity += 0.25f;
            player.GetAttackSpeed<RogueDamageClass>() -= 0.15f;
        }
    }
}
