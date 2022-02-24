using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Materials
{
    public class VictoryShard : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Victory Shard");
        }

        public override void SetDefaults()
        {
            item.width = 14;
            item.height = 14;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 3);
            item.rare = ItemRarityID.Green;
        }
    }
}
