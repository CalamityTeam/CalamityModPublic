using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Body)]
    public class AncientGodSlayerChestplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Ancient God Slayer Chestplate");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 20;
            Item.Calamity().customRarity = (CalamityRarity)14;
            Item.vanity = true;
            Item.value = Item.buyPrice(0, 60, 0, 0);
        }
    }
}
