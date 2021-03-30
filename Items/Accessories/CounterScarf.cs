using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Neck)]
    public class CounterScarf : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Counter Scarf");
            Tooltip.SetDefault("True melee strikes deal 15% more damage\n" +
                "Grants the ability to dash; dashing into an attack will cause you to dodge it\n" +
                "After a dodge you will be granted a buff to all damage, melee speed, and all crit chance for a short time\n" +
                "After a successful dodge you must wait 15 seconds before you can dodge again\n" +
                "This cooldown will be twice as long if you have Chaos State\n" +
                "While on cooldown, Chaos State will last twice as long\n" +
				"Rare Item Variant");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = CalamityGlobalItem.Rarity2BuyPrice;
            item.rare = ItemRarityID.Green;
            item.accessory = true;
			item.Calamity().customRarity = CalamityRarity.RareVariant;
		}

        public override bool CanEquipAccessory(Player player, int slot) => !player.Calamity().dodgeScarf;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.dodgeScarf = true;
            modPlayer.dashMod = 1;
        }
    }
}
