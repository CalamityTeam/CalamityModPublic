using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Materials
{
    public class MeldBlob : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Meld Blob");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 14;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 20);
            item.rare = ItemRarityID.Cyan;
        }
    }
}
