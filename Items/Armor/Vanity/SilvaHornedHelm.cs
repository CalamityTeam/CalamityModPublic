using CalamityMod.Items.Armor.Silva;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class SilvaHornedHelm : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Silva Horned Helm");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 24;
            Item.value = Item.buyPrice(0, 8, 0, 0);
            Item.vanity = true;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<SilvaArmor>() && legs.type == ModContent.ItemType<SilvaLeggings>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }
    }
}
