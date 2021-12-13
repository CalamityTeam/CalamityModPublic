using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Body)]
    public class CalamityRobes : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Robes of Calamity");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 20;
            item.rare = ItemRarityID.Lime;
            item.vanity = true;
            item.Calamity().donorItem = true;
            item.value = Item.sellPrice(gold: 2);
        }
    }
}
