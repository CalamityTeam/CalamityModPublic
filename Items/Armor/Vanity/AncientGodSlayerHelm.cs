using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class AncientGodSlayerHelm : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Ancient God Slayer Helm");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 20;
            Item.Calamity().customRarity = (CalamityRarity)14;
            Item.value = Item.buyPrice(0, 75, 0, 0);
            Item.vanity = true;
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
