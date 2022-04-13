using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Fishing.SunkenSeaCatches
{
    public class GreenwaveLoach : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Greenwave Loach");
            Tooltip.SetDefault("An endangered fish that is highly prized in the market");
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 38;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(gold: 10);
            Item.rare = ItemRarityID.Orange;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }
    }
}
