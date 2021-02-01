using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Fishing.AstralCatches
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
            item.rare = ItemRarityID.Blue;
        }
    }
}
