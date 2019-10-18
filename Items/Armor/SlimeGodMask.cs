using Terraria.ModLoader;
namespace CalamityMod.Items
{
    [AutoloadEquip(EquipType.Head)]
    public class SlimeGodMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slime God Mask");
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
