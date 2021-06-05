using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class AncientGodSlayerHelm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient God Slayer Helm");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 20;
            item.Calamity().customRarity = (CalamityRarity)14;
            item.value = Item.buyPrice(0, 75, 0, 0);
            item.vanity = true;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<AncientGodSlayerChestplate>() && legs.type == ModContent.ItemType<AncientGodSlayerLeggings>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }
    }
}
