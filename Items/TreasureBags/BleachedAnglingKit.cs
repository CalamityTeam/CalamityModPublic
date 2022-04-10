using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class BleachedAnglingKit : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bleached Angling Kit");
            Tooltip.SetDefault("Has a chance to contain various fishing gear\n" +
            "{$CommonItemTooltip.RightClickToOpen}");
        }

        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.Pink;
        }

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            // IEntitySource my beloathed
            var s = player.GetItemSource_OpenItem(Item.type);

            int anglerTackleBagChance = !Main.expertMode ? 18 : 15;
            int fishingAccChance = !Main.expertMode ? 12 : 10;
            int fishFindAccChance = !Main.expertMode ? 9 : 8;
            int anglerArmorChance = !Main.expertMode ? 4 : 2;
            int potionChance = !Main.expertMode ? 4 : 2;
            int bugNetChance = !Main.expertMode ? 15 : 12;
            // Fishing
            DropHelper.DropItemChance(s, player, ItemID.AnglerTackleBag, anglerTackleBagChance, 1, 1);
            DropHelper.DropItemChance(s, player, ItemID.HighTestFishingLine, fishingAccChance);
            DropHelper.DropItemChance(s, player, ItemID.TackleBox, fishingAccChance);
            DropHelper.DropItemChance(s, player, ItemID.AnglerEarring, fishingAccChance);
            DropHelper.DropItemChance(s, player, ItemID.FishermansGuide, fishFindAccChance);
            DropHelper.DropItemChance(s, player, ItemID.WeatherRadio, fishFindAccChance);
            DropHelper.DropItemChance(s, player, ItemID.Sextant, fishFindAccChance);
            DropHelper.DropItemChance(s, player, ItemID.AnglerHat, anglerArmorChance);
            DropHelper.DropItemChance(s, player, ItemID.AnglerVest, anglerArmorChance);
            DropHelper.DropItemChance(s, player, ItemID.AnglerPants, anglerArmorChance);
            DropHelper.DropItemChance(s, player, ItemID.FishingPotion, potionChance, 2, 3);
            DropHelper.DropItemChance(s, player, ItemID.SonarPotion, potionChance, 2, 3);
            DropHelper.DropItemChance(s, player, ItemID.CratePotion, potionChance, 2, 3);
            DropHelper.DropItemChance(s, player, ItemID.GoldenBugNet, bugNetChance, 1, 1);
        }
    }
}
