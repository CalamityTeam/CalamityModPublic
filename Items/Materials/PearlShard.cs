using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Materials
{
    [LegacyName("VictoryShard")]
    public class PearlShard : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 25;
            DisplayName.SetDefault("Pearl Shard");
			ItemID.Sets.SortingPriorityMaterials[Type] = 60; // Meteorite
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(silver: 3);
            Item.rare = ItemRarityID.Green;
        }    }
}
