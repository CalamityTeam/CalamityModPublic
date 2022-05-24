using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Legs)]
    public class AncientGodSlayerLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Ancient God Slayer Leggings");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 20;
            Item.Calamity().customRarity = (CalamityRarity)14;
            Item.value = Item.buyPrice(0, 45, 0, 0);
            Item.vanity = true;
        }
    }
}
