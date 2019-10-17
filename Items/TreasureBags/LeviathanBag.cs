using CalamityMod.World;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.NPCs;
namespace CalamityMod.Items
{
    public class LeviathanBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<Siren>();

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
            // siren & levi are available PHM, so this check is necessary to keep vanilla consistency
            if (Main.hardMode)
                player.TryGettingDevArmor();

            // Weapons
            DropHelper.DropItemCondition(player, ModContent.ItemType<Greentide>(), Main.hardMode, 3, 1, 1);
            DropHelper.DropItemCondition(player, ModContent.ItemType<Leviatitan>(), Main.hardMode, 3, 1, 1);
            DropHelper.DropItemCondition(player, ModContent.ItemType<SirensSong>(), Main.hardMode, 3, 1, 1);
            DropHelper.DropItemCondition(player, ModContent.ItemType<Atlantis>(), Main.hardMode, 3, 1, 1);
            DropHelper.DropItemCondition(player, ModContent.ItemType<BrackishFlask>(), Main.hardMode, 3, 1, 1);

            // Equipment
            DropHelper.DropItemCondition(player, ModContent.ItemType<LeviathanAmbergris>(), Main.hardMode);
            DropHelper.DropItemCondition(player, ModContent.ItemType<LureofEnthrallment>(), Main.hardMode, 3, 1, 1);
            float communityChance = CalamityWorld.defiled ? DropHelper.DefiledDropRateFloat : DropHelper.LegendaryDropRateFloat;
            DropHelper.DropItemCondition(player, ModContent.ItemType<TheCommunity>(), CalamityWorld.revenge, communityChance);

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<LeviathanMask>(), 7);

            // Fishing
            DropHelper.DropItem(player, ModContent.ItemType<EnchantedPearl>());
            DropHelper.DropItemChance(player, ItemID.HotlineFishingHook, 10);
            DropHelper.DropItemChance(player, ItemID.BottomlessBucket, 10);
            DropHelper.DropItemChance(player, ItemID.SuperAbsorbantSponge, 10);
            DropHelper.DropItemChance(player, ItemID.FishingPotion, 5, 5, 8);
            DropHelper.DropItemChance(player, ItemID.SonarPotion, 5, 5, 8);
            DropHelper.DropItemChance(player, ItemID.CratePotion, 5, 5, 8);

            // Other
            DropHelper.DropItemCondition(player, ModContent.ItemType<IOU>(), !Main.hardMode);
        }
    }
}
