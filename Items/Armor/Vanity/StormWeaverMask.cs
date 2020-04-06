using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class StormWeaverMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Weaver Mask");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 20;
            item.rare = 1;
            item.vanity = true;
        }

        public override bool DrawHead()
        {
            return false;
        }
    }
}
