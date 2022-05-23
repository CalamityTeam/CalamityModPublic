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
            SacrificeTotal = 3;
        }

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 36;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(gold: 10);
            Item.rare = ItemRarityID.Orange;
        }
    }
}
