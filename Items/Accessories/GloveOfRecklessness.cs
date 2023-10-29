using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(new EquipType[] { EquipType.HandsOn, EquipType.HandsOff } )]
    public class GloveOfRecklessness : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 36;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.accessory = true;
            Item.rare = ItemRarityID.Lime;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.gloveOfRecklessness = true;
            modPlayer.stealthGenStandstill += 0.15f;
            modPlayer.stealthGenMoving += 0.15f;
            player.GetDamage<RogueDamageClass>() -= 0.10f;
            player.GetCritChance<RogueDamageClass>() -= 5;
            player.GetAttackSpeed<RogueDamageClass>() += 0.15f;
        }
    }
}
