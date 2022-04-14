using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Materials
{
    public class ReaperTooth : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
            DisplayName.SetDefault("Reaper Tooth");
            Tooltip.SetDefault("Sharp enough to cut diamonds");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.maxStack = 999;
            Item.value = Item.buyPrice(0, 7, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
        }
    }
}
