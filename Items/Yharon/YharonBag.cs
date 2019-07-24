using Terraria;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.Items.Yharon
{
	public class YharonBag : ModItem
	{
		public override int BossBagNPC => mod.NPCType("Yharon");

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

			// Weapons
			DropHelper.DropItemChance(player, mod.ItemType("DragonRage"), 3);
			DropHelper.DropItemChance(player, mod.ItemType("TheBurningSky"), 3);
			DropHelper.DropItemChance(player, mod.ItemType("DragonsBreath"), 3);
			DropHelper.DropItemChance(player, mod.ItemType("ChickenCannon"), 3);
			DropHelper.DropItemChance(player, mod.ItemType("PhoenixFlameBarrage"), 3);
			DropHelper.DropItemChance(player, mod.ItemType("AngryChickenStaff"), 3);
			DropHelper.DropItemChance(player, mod.ItemType("ProfanedTrident"), 3); // Infernal Spear

			// Equipment
			DropHelper.DropItem(player, mod.ItemType("YharimsGift"));
			DropHelper.DropItemCondition(player, mod.ItemType("DrewsWings"), CalamityWorld.revenge);

			// Vanity
			DropHelper.DropItemChance(player, mod.ItemType("YharonMask"), 7);
			DropHelper.DropItemChance(player, mod.ItemType("ForgottenDragonEgg"), 10);
			DropHelper.DropItemCondition(player, mod.ItemType("FoxDrive"), CalamityWorld.revenge);
		}
	}
}