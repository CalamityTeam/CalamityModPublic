using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Fishing.BrimstoneCragCatches
{
    public class Shadowfish : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadowfish");
            Tooltip.SetDefault("Darkness spreads");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(silver: 8);
            Item.rare = ItemRarityID.Blue;
        }
    }
}
