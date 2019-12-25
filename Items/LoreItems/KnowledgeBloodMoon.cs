using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeBloodMoon : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Red Moon");
            Tooltip.SetDefault("We long ago feared the light of the red moon.\n" +
                "Many went mad, others died, but a scant few became blessed with a wealth of cosmic understanding.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 9;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }
    }
}
