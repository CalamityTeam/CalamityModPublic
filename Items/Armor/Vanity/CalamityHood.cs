using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class CalamityHood : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hood of Calamity");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 24;
            item.rare = ItemRarityID.Lime;
            item.vanity = true;
            item.Calamity().donorItem = true;
            item.value = Item.sellPrice(gold: 2);
        }

        public override bool DrawHead() => false;
    }
}
