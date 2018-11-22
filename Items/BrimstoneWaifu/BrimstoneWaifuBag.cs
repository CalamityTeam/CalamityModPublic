using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.BrimstoneWaifu
{
	public class BrimstoneWaifuBag : ModItem
	{
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
			bossBagNPC = mod.NPCType("BrimstoneElemental");
		}

		public override bool CanRightClick()
		{
			return true;
		}

		public override void OpenBossBag(Player player)
		{
			player.TryGettingDevArmor();
			int choice = Main.rand.Next(3);
			if (choice == 0)
			{
				player.QuickSpawnItem(mod.ItemType("Abaddon"));
			}
			else if (choice == 1)
			{
				player.QuickSpawnItem(mod.ItemType("Brimlance"));
			}
			else
			{
				player.QuickSpawnItem(mod.ItemType("SeethingDischarge"));
			}
            player.QuickSpawnItem(ItemID.SoulofFright, Main.rand.Next(25, 41));
            player.QuickSpawnItem(mod.ItemType("EssenceofChaos"), Main.rand.Next(3, 5));
			player.QuickSpawnItem(mod.ItemType("Gehenna"));
            if (CalamityWorld.revenge)
            {
                player.QuickSpawnItem(mod.ItemType("CharredRelic"));
                if (CalamityWorld.downedProvidence)
                {
                    player.QuickSpawnItem(mod.ItemType("Brimrose"));
                }
            }
		}
	}
}