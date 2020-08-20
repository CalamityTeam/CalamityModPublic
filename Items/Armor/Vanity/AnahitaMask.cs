using Terraria.ModLoader;
namespace CalamityMod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class AnahitaMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Anahita Mask");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 20;
            item.rare = 1;
            item.vanity = true;
        }

        public override bool DrawHead() => false;
    }
}
