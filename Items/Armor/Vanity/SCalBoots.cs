using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Legs)]
    public class SCalBoots : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Calamitous Boots");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            Item.vanity = true;
        }
    }
}
