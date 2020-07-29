using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class FleshyGeodeT1 : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fleshy Geode");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
        }

        public override void SetDefaults()
        {
            item.maxStack = 999;
            item.consumable = true;
            item.width = 24;
            item.height = 24;
            item.rare = 8;
        }

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            // Materials
            int barMin = !Main.expertMode ? 1 : 2;
            int barMax = 3;
            int coreMin = 1;
            int coreMax = !Main.expertMode ? 2 : 3;
            DropHelper.DropItem(player, ModContent.ItemType<VerstaltiteBar>(), barMin, barMax);
            DropHelper.DropItem(player, ModContent.ItemType<DraedonBar>(), barMin, barMax);
            DropHelper.DropItem(player, ModContent.ItemType<CruptixBar>(), barMin, barMax);
            DropHelper.DropItem(player, ModContent.ItemType<CoreofCinder>(), coreMin, coreMax);
            DropHelper.DropItem(player, ModContent.ItemType<CoreofEleum>(), coreMin, coreMax);
            DropHelper.DropItem(player, ModContent.ItemType<CoreofChaos>(), coreMin, coreMax);
        }
    }
}
