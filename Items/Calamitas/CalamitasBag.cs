using Terraria;
using Terraria.ModLoader;
using CalamityMod.World;
using CalamityMod.Utilities;

namespace CalamityMod.Items.Calamitas
{
	public class CalamitasBag : ModItem
	{
		public override int BossBagNPC => mod.NPCType("CalamitasRun3");

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
			player.TryGettingDevArmor();

			// Materials
			DropHelper.DropItem(player, mod.ItemType("CalamityDust"), 14, 18);
			DropHelper.DropItem(player, mod.ItemType("BlightedLens"), 1, 3);
			DropHelper.DropItem(player, mod.ItemType("EssenceofChaos"), 5, 9);
			DropHelper.DropItemCondition(player, mod.ItemType("Bloodstone"), CalamityWorld.downedProvidence, 35, 45);

			// Weapons
			DropHelper.DropItemChance(player, mod.ItemType("TheEyeofCalamitas"), 3);
			DropHelper.DropItemChance(player, mod.ItemType("Animosity"), 3);
			DropHelper.DropItemChance(player, mod.ItemType("CalamitasInferno"), 3);
			DropHelper.DropItemChance(player, mod.ItemType("BlightedEyeStaff"), 3);

			// Equipment
			DropHelper.DropItem(player, mod.ItemType("CalamityRing"));
			DropHelper.DropItemChance(player, mod.ItemType("ChaosStone"), 10);

			// Vanity
			DropHelper.DropItemChance(player, mod.ItemType("CalamitasMask"), 7);
		}
	}
}
