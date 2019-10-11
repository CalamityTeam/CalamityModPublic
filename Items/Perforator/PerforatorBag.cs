using CalamityMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Perforator
{
    public class PerforatorBag : ModItem
    {
        public override int BossBagNPC => mod.NPCType("PerforatorHive");

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Treasure Bag");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
        }

        public override void SetDefaults()
        {
            item.maxStack = 999;
            item.consumable = true;
            item.width = 24;
            item.height = 24;
            item.rare = 9;
            item.expert = true;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void OpenBossBag(Player player)
        {
            // Materials
            DropHelper.DropItem(player, ItemID.Vertebrae, 10, 20);
            DropHelper.DropItem(player, ItemID.CrimtaneBar, 9, 14);
            DropHelper.DropItem(player, mod.ItemType("BloodSample"), 30, 40);
            DropHelper.DropItemCondition(player, ItemID.Ichor, Main.hardMode, 15, 30);

            // Weapons
            DropHelper.DropItemChance(player, mod.ItemType("VeinBurster"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("BloodyRupture"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("SausageMaker"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("Aorta"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("Eviscerator"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("BloodBath"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("BloodClotStaff"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("ToothBall"), 3, 50, 75);

            // Equipment
            DropHelper.DropItem(player, mod.ItemType("BloodyWormTooth"));

            // Vanity
            DropHelper.DropItemChance(player, mod.ItemType("PerforatorMask"), 7);
            DropHelper.DropItemChance(player, mod.ItemType("BloodyVein"), 10);
        }
    }
}
