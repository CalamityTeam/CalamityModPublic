using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Materials
{
    public class SolarVeil : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
            DisplayName.SetDefault("Solar Veil");
            Tooltip.SetDefault("Sunlight cannot penetrate the fabric of this cloth");
        }

        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 32;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(silver: 80);
            Item.rare = ItemRarityID.Lime;
        }    }
}
