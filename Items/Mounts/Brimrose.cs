using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Mounts
{
    public class Brimrose : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimrose");
            Tooltip.SetDefault("Summons a brimrose mount");
        }

        public override void SetDefaults()
        {
            item.width = 64;
            item.height = 64;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.expert = true;
            item.UseSound = SoundID.Item3;
            item.noMelee = true;
            item.mountType = ModContent.MountType<PhuppersChair>();

            item.value = Item.buyPrice(platinum: 1, gold: 50);
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.Calamity().devItem = true;
        }
    }
}
