using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class ProcyonidPrawn : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Procyonid Prawn");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 26;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 5);
            item.rare = 1;
        }
    }
}
