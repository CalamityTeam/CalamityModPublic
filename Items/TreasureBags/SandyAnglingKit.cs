using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class SandyAnglingKit : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sandy Angling Kit");
            Tooltip.SetDefault("Has a chance to contain various fishing gear\n" +
            "{$CommonItemTooltip.RightClickToOpen}");
        }

        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.Blue;
        }

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            // IEntitySource my beloathed
            var s = player.GetItemSource_OpenItem(Item.type);

            int fishingAccChance = !Main.expertMode ? 15 : 12;
            int fishFindAccChance = !Main.expertMode ? 10 : 9;
            int anglerArmorChance = !Main.expertMode ? 5 : 4;
            int potionChance = !Main.expertMode ? 5 : 4;
            int bugNetChance = !Main.expertMode ? 20 : 18;
            // Fishing
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
            DropHelper.DropItemCondition(s, player, ItemID.GoldenBugNet, NPC.downedBoss3, bugNetChance, 1, 1);
        }
    }
}
