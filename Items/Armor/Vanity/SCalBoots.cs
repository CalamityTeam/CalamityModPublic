using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Legs)]
    public class SCalBoots : ModItem
    {
        public override void SetStaticDefaults() => DisplayName.SetDefault("Calamitous Boots");

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 20;
            item.rare = ItemRarityID.Blue;
            item.vanity = true;
        }
    }
}
