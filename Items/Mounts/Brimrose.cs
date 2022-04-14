using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Mounts
{
    public class Brimrose : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimrose");
            Tooltip.SetDefault("Summons a brimrose mount");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 64;
            Item.height = 64;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item3;
            Item.noMelee = true;
            Item.mountType = ModContent.MountType<PhuppersChair>();

            Item.value = Item.buyPrice(platinum: 1, gold: 50);
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.Calamity().devItem = true;
        }
    }
}
