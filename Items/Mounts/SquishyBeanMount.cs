using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Mounts
{
    public class SquishyBeanMount : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Suspicious Looking Jelly Bean");
            Tooltip.SetDefault("JELLY BEAN");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item3;
            item.noMelee = true;
            item.mountType = ModContent.MountType<SquishyBean>();

            item.value = Item.buyPrice(platinum: 1);
            item.rare = ItemRarityID.Cyan;
            item.Calamity().devItem = true;
        }
    }
}
