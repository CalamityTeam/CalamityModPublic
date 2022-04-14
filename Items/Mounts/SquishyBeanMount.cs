using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Mounts
{
    public class SquishyBeanMount : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Suspicious Looking Jelly Bean");
            Tooltip.SetDefault("JELLY BEAN");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item3;
            Item.noMelee = true;
            Item.mountType = ModContent.MountType<SquishyBean>();

            Item.value = Item.buyPrice(platinum: 1);
            Item.rare = ItemRarityID.Cyan;
            Item.Calamity().devItem = true;
        }
    }
}
