using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Fishing.BrimstoneCragCatches
{
    public class CharredLasher : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Charred Lasher");
            Tooltip.SetDefault("This elusive fish is a prized commodity");
        }

        public override void SetDefaults()
        {
            item.width = 44;
            item.height = 36;
            item.maxStack = 999;
            item.value = Item.sellPrice(gold: 10);
            item.rare = ItemRarityID.Orange;
        }
    }
}
