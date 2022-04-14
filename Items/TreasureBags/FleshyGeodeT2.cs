using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.TreasureBags
{
    public class FleshyGeodeT2 : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Necromantic Geode");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 10;
        }

        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 24;
            Item.height = 24;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            // IEntitySource my beloathed
            var s = player.GetItemSource_OpenItem(Item.type);

            // Materials
            int barMin = !Main.expertMode ? 5 : 7;
            int barMax = !Main.expertMode ? 10 : 12;
            int coreMin = !Main.expertMode ? 1 : 2;
            int coreMax = !Main.expertMode ? 3 : 4;
            int bloodstoneMin = !Main.expertMode ? 50 : 60;
            int bloodstoneMax = !Main.expertMode ? 60 : 70;
            int lifeAlloyChance = !Main.expertMode ? 2 : 1;
            int coreofCalChance = !Main.expertMode ? 3 : 2;
            DropHelper.DropItem(s, player, ModContent.ItemType<VerstaltiteBar>(), barMin, barMax);
            DropHelper.DropItem(s, player, ModContent.ItemType<DraedonBar>(), barMin, barMax);
            DropHelper.DropItem(s, player, ModContent.ItemType<CruptixBar>(), barMin, barMax);
            DropHelper.DropItem(s, player, ModContent.ItemType<CoreofCinder>(), coreMin, coreMax);
            DropHelper.DropItem(s, player, ModContent.ItemType<CoreofEleum>(), coreMin, coreMax);
            DropHelper.DropItem(s, player, ModContent.ItemType<CoreofChaos>(), coreMin, coreMax);
            DropHelper.DropItem(s, player, ModContent.ItemType<Bloodstone>(), bloodstoneMin, bloodstoneMax);
            DropHelper.DropItemChance(s, player, ModContent.ItemType<BarofLife>(), lifeAlloyChance, 1, 1);
            DropHelper.DropItemChance(s, player, ModContent.ItemType<CoreofCalamity>(), coreofCalChance, 1, 1);
        }
    }
}
