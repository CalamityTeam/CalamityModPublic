using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeSulphurSea : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sulphur Sea");
            Tooltip.SetDefault("I remember the serene waves and the clear breeze.\n" +
                "The bitterness of my youth has long since subsided, but it is far too late. I must never repeat a mistake like this again.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 5;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }
    }
}
