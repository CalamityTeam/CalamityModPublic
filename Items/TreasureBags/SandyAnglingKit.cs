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
            item.maxStack = 999;
            item.consumable = true;
            item.width = 24;
            item.height = 24;
            item.rare = ItemRarityID.Blue;
        }

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            int fishingAccChance = !Main.expertMode ? 15 : 12;
            int fishFindAccChance = !Main.expertMode ? 10 : 9;
            int anglerArmorChance = !Main.expertMode ? 5 : 4;
            int potionChance = !Main.expertMode ? 5 : 4;
            int bugNetChance = !Main.expertMode ? 20 : 18;
            // Fishing
            DropHelper.DropItemChance(player, ItemID.HighTestFishingLine, fishingAccChance);
            DropHelper.DropItemChance(player, ItemID.TackleBox, fishingAccChance);
            DropHelper.DropItemChance(player, ItemID.AnglerEarring, fishingAccChance);
            DropHelper.DropItemChance(player, ItemID.FishermansGuide, fishFindAccChance);
            DropHelper.DropItemChance(player, ItemID.WeatherRadio, fishFindAccChance);
            DropHelper.DropItemChance(player, ItemID.Sextant, fishFindAccChance);
            DropHelper.DropItemChance(player, ItemID.AnglerHat, anglerArmorChance);
            DropHelper.DropItemChance(player, ItemID.AnglerVest, anglerArmorChance);
            DropHelper.DropItemChance(player, ItemID.AnglerPants, anglerArmorChance);
            DropHelper.DropItemChance(player, ItemID.FishingPotion, potionChance, 2, 3);
            DropHelper.DropItemChance(player, ItemID.SonarPotion, potionChance, 2, 3);
            DropHelper.DropItemChance(player, ItemID.CratePotion, potionChance, 2, 3);
            DropHelper.DropItemCondition(player, ItemID.GoldenBugNet, NPC.downedBoss3, bugNetChance, 1, 1);
        }
    }
}
