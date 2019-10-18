using Terraria.ModLoader;
namespace CalamityMod.Items
{
    [AutoloadEquip(EquipType.Head)]
    public class DesertScourgeMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Desert Scourge Mask");
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
