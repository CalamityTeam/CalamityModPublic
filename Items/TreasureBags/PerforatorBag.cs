using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.NPCs;
namespace CalamityMod.Items
{
    public class PerforatorBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<PerforatorHive>();

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
            DropHelper.DropItem(player, ModContent.ItemType<BloodSample>(), 30, 40);
            DropHelper.DropItemCondition(player, ItemID.Ichor, Main.hardMode, 15, 30);

            // Weapons
            DropHelper.DropItemChance(player, ModContent.ItemType<VeinBurster>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<BloodyRupture>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<SausageMaker>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<Aorta>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<Eviscerator>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<BloodBath>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<BloodClotStaff>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<ToothBall>(), 3, 50, 75);

            // Equipment
            DropHelper.DropItem(player, ModContent.ItemType<BloodyWormTooth>());

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<PerforatorMask>(), 7);
            DropHelper.DropItemChance(player, ModContent.ItemType<BloodyVein>(), 10);
        }
    }
}
