using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.Items.SlimeGod
{
	public class SlimeGodBag : ModItem
	{
		public override int BossBagNPC => mod.NPCType("SlimeGodRun");

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
			DropHelper.DropItem(player, ItemID.Gel, 30, 60);
			DropHelper.DropItem(player, mod.ItemType("PurifiedGel"), 30, 50);

			// Weapons
			DropHelper.DropItemChance(player, mod.ItemType("OverloadedBlaster"), 3);
			DropHelper.DropItemChance(player, mod.ItemType("AbyssalTome"), 3);
			DropHelper.DropItemChance(player, mod.ItemType("EldritchTome"), 3);
			DropHelper.DropItemChance(player, mod.ItemType("CorroslimeStaff"), 3);
			DropHelper.DropItemChance(player, mod.ItemType("CrimslimeStaff"), 3);
			DropHelper.DropItemChance(player, mod.ItemType("GelDart"), 3, 100, 125);

			// Equipment
			DropHelper.DropItem(player, mod.ItemType("ManaOverloader"));
			DropHelper.DropItemCondition(player, mod.ItemType("ElectrolyteGelPack"), CalamityWorld.revenge);

			// Vanity
			DropHelper.DropItemFromSetChance(player, 7, mod.ItemType("SlimeGodMask"), mod.ItemType("SlimeGodMask2"));

			// Other
			DropHelper.DropItem(player, mod.ItemType("StaticRefiner"));
		}
	}
}
