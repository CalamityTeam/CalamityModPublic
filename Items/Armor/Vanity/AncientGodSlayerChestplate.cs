using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Body)]
    public class AncientGodSlayerChestplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient God Slayer Chestplate");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 20;
            item.Calamity().customRarity = (CalamityRarity)14;
            item.vanity = true;
            item.value = Item.buyPrice(0, 60, 0, 0);
        }
    }
}
