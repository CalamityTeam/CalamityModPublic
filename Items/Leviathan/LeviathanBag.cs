using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.World;

namespace CalamityMod.Items.Leviathan
{
	public class LeviathanBag : ModItem
	{
        public override int BossBagNPC => mod.NPCType("Siren");

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

            DropHelper.DropRevBagAccessories(player);

            // Weapons
            DropHelper.DropItemCondition(player, mod.ItemType("Greentide"), Main.hardMode, 3, 1, 1);
            DropHelper.DropItemCondition(player, mod.ItemType("Leviatitan"), Main.hardMode, 3, 1, 1);
            DropHelper.DropItemCondition(player, mod.ItemType("SirensSong"), Main.hardMode, 3, 1, 1);
            DropHelper.DropItemCondition(player, mod.ItemType("Atlantis"), Main.hardMode, 3, 1, 1);
            DropHelper.DropItemCondition(player, mod.ItemType("BrackishFlask"), Main.hardMode, 3, 1, 1);

            // Equipment
            DropHelper.DropItemCondition(player, mod.ItemType("LeviathanAmbergris"), Main.hardMode);
            DropHelper.DropItemCondition(player, mod.ItemType("LureofEnthrallment"), Main.hardMode, 3, 1, 1);
            float communityChance = CalamityWorld.defiled ? DropHelper.DefiledDropRateFloat : DropHelper.LegendaryChanceFloat;
            DropHelper.DropItemCondition(player, mod.ItemType("TheCommunity"), CalamityWorld.revenge, communityChance);

            // Vanity
            DropHelper.DropItemChance(player, mod.ItemType("LeviathanMask"), 7);

            // Fishing
            DropHelper.DropItem(player, mod.ItemType("EnchantedPearl"));
            DropHelper.DropItemChance(player, ItemID.HotlineFishingHook, 10);
            DropHelper.DropItemChance(player, ItemID.BottomlessBucket, 10);
            DropHelper.DropItemChance(player, ItemID.SuperAbsorbantSponge, 10);
            DropHelper.DropItemChance(player, ItemID.FishingPotion, 5, 5, 8);
            DropHelper.DropItemChance(player, ItemID.SonarPotion, 5, 5, 8);
            DropHelper.DropItemChance(player, ItemID.CratePotion, 5, 5, 8);

            // Other
            DropHelper.DropItemCondition(player, mod.ItemType("IOU"), !Main.hardMode);
		}
	}
}
