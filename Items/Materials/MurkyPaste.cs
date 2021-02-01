using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Materials
{
    public class MurkyPaste : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Murky Paste");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 2);
            item.rare = ItemRarityID.Blue;
        }
    }
}
