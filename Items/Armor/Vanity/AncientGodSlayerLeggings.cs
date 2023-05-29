using CalamityMod.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Legs)]
    public class AncientGodSlayerLeggings : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Armor";
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 20;
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.value = Item.buyPrice(0, 45, 0, 0);
            Item.vanity = true;
        }
    }
}
