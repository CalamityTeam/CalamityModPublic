using Terraria;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.Items.Bumblefuck
{
	public class BumblebirbBag : ModItem
	{
		public override int BossBagNPC => mod.NPCType("Bumblefuck");

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
			item.expert = true;
			item.rare = 9;
		}

		public override bool CanRightClick()
		{
			return true;
		}

		public override void OpenBossBag(Player player)
		{
			player.TryGettingDevArmor();

			// Materials
			DropHelper.DropItem(player, mod.ItemType("EffulgentFeather"), 9, 14);

			// Weapons
			DropHelper.DropItemFromSet(player, mod.ItemType("GildedProboscis"), mod.ItemType("GoldenEagle"), mod.ItemType("RougeSlash"));
			DropHelper.DropItemChance(player, mod.ItemType("Swordsplosion"), DropHelper.RareVariantDropRateInt);

			// Equipment
			DropHelper.DropItemCondition(player, mod.ItemType("RedLightningContainer"), CalamityWorld.revenge);
		}
	}
}
