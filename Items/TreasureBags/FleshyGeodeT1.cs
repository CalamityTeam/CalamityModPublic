using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.TreasureBags
{
    public class FleshyGeodeT1 : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fleshy Geode");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
            SacrificeTotal = 10;
        }

        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.Yellow;
        }

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            // IEntitySource my beloathed
            var s = player.GetSource_OpenItem(Item.type);

            // Materials
            int barMin = !Main.expertMode ? 1 : 2;
            int barMax = 3;
            int coreMin = 1;
            int coreMax = !Main.expertMode ? 2 : 3;
            DropHelper.DropItem(s, player, ModContent.ItemType<VerstaltiteBar>(), barMin, barMax);
            DropHelper.DropItem(s, player, ModContent.ItemType<DraedonBar>(), barMin, barMax);
            DropHelper.DropItem(s, player, ModContent.ItemType<CruptixBar>(), barMin, barMax);
            DropHelper.DropItem(s, player, ModContent.ItemType<CoreofCinder>(), coreMin, coreMax);
            DropHelper.DropItem(s, player, ModContent.ItemType<CoreofEleum>(), coreMin, coreMax);
            DropHelper.DropItem(s, player, ModContent.ItemType<CoreofChaos>(), coreMin, coreMax);
        }
    }
}
