using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.CalPlayer.Dashes;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Neck)]
    public class CounterScarf : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 38;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override bool CanEquipAccessory(Player player, int slot, bool modded) => !player.Calamity().dodgeScarf;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<TrueMeleeDamageClass>() += 0.1f;
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.dodgeScarf = true;
            modPlayer.DashID = CounterScarfDash.ID;
            player.dashType = 0;
        }
    }
}
