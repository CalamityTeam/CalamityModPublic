using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.World;

namespace CalamityMod.Items.BrimstoneWaifu
{
	public class BrimstoneWaifuBag : ModItem
	{
        public override int BossBagNPC => mod.NPCType("BrimstoneElemental");

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
			if (CalamityWorld.revenge)
			{
				player.QuickSpawnItem(mod.ItemType("CharredRelic"));
				if (CalamityWorld.downedProvidence)
				{
					player.QuickSpawnItem(mod.ItemType("Brimrose"));
				}
				if (Main.rand.Next(20) == 0)
				{
					switch (Main.rand.Next(3))
					{
						case 0:
							player.QuickSpawnItem(mod.ItemType("StressPills"));
							break;
						case 1:
							player.QuickSpawnItem(mod.ItemType("Laudanum"));
							break;
						case 2:
							player.QuickSpawnItem(mod.ItemType("HeartofDarkness"));
							break;
					}
				}
			}
			player.TryGettingDevArmor();
			switch (Main.rand.Next(3))
			{
				case 0:
					player.QuickSpawnItem(mod.ItemType("Abaddon"));
					break;
				case 1:
					player.QuickSpawnItem(mod.ItemType("Abaddon"));
					player.QuickSpawnItem(mod.ItemType("Brimlance"));
					break;
				case 2:
					player.QuickSpawnItem(mod.ItemType("Abaddon"));
					player.QuickSpawnItem(mod.ItemType("SeethingDischarge"));
					break;
			}
			if (CalamityWorld.downedProvidence)
			{
				player.QuickSpawnItem(mod.ItemType("Bloodstone"), Main.rand.Next(25, 36));
			}
			if (Main.rand.Next(10) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("RoseStone"));
			}
			player.QuickSpawnItem(ItemID.SoulofFright, Main.rand.Next(25, 41));
			player.QuickSpawnItem(mod.ItemType("EssenceofChaos"), Main.rand.Next(5, 10));
			player.QuickSpawnItem(mod.ItemType("Gehenna"));
		}
	}
}
