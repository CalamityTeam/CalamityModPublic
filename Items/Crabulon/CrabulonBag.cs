using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.World;

namespace CalamityMod.Items.Crabulon
{
	public class CrabulonBag : ModItem
	{
		public override int BossBagNPC => mod.NPCType("CrabulonIdle");

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
			DropHelper.DropItem(player, ItemID.GlowingMushroom, 25, 35);
			DropHelper.DropItem(player, ItemID.MushroomGrassSeeds, 5, 10);

			// Weapons
			DropHelper.DropItemChance(player, mod.ItemType("MycelialClaws"), 3);
			DropHelper.DropItemChance(player, mod.ItemType("Fungicide"), 3);
			DropHelper.DropItemChance(player, mod.ItemType("HyphaeRod"), 3);
			DropHelper.DropItemChance(player, mod.ItemType("Mycoroot"), 3);

			// Equipment
			DropHelper.DropItem(player, mod.ItemType("FungalClump"));
			DropHelper.DropItemCondition(player, mod.ItemType("MushroomPlasmaRoot"), CalamityWorld.revenge);

			// Vanity
			DropHelper.DropItemChance(player, mod.ItemType("CrabulonMask"), 7);
		}
	}
}
