using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(new EquipType[] { EquipType.HandsOn, EquipType.HandsOff } )]
    public class GloveOfPrecision : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Glove of Precision");
            Tooltip.SetDefault("Decreases rogue attack speed by 20% but increases damage and crit by 12% and velocity by 25%");
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
            player.GetCritChance<RogueDamageClass>() += 12;
            modPlayer.rogueVelocity += 0.25f;
            player.GetAttackSpeed<RogueDamageClass>() -= 0.2f;
        }
    }
}
