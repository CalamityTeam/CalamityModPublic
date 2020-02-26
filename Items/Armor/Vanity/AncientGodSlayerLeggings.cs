using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Legs)]
    public class AncientGodSlayerLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient God Slayer Leggings");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 20;
            item.Calamity().customRarity = (CalamityRarity)14;
            item.value = Item.buyPrice(0, 45, 0, 0);
            item.vanity = true;
        }
    }
}
