using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.World;

namespace CalamityMod.Items.Scavenger
{
	public class RavagerBag : ModItem
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
			bossBagNPC = mod.NPCType("ScavengerBody");
		}

		public override bool CanRightClick()
		{
			return true;
		}

		public override void OpenBossBag(Player player)
		{
			if (CalamityWorld.revenge)
			{
				player.QuickSpawnItem(mod.ItemType("InfernalBlood"));
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
			if (CalamityWorld.downedProvidence)
			{
				player.QuickSpawnItem(mod.ItemType("Bloodstone"), Main.rand.Next(60, 71));
				player.QuickSpawnItem(mod.ItemType("VerstaltiteBar"), Main.rand.Next(7, 13));
				player.QuickSpawnItem(mod.ItemType("DraedonBar"), Main.rand.Next(7, 13));
				player.QuickSpawnItem(mod.ItemType("CruptixBar"), Main.rand.Next(7, 13));
				player.QuickSpawnItem(mod.ItemType("CoreofCinder"), Main.rand.Next(2, 5));
				player.QuickSpawnItem(mod.ItemType("CoreofEleum"), Main.rand.Next(2, 5));
				player.QuickSpawnItem(mod.ItemType("CoreofChaos"), Main.rand.Next(2, 5));
				player.QuickSpawnItem(mod.ItemType("BarofLife"));
				if (Main.rand.Next(2) == 0)
				{
					player.QuickSpawnItem(mod.ItemType("CoreofCalamity"));
				}
				player.QuickSpawnItem(mod.ItemType("BloodflareCore"));
			}
			else
			{
				player.QuickSpawnItem(mod.ItemType("VerstaltiteBar"), Main.rand.Next(2, 4));
				player.QuickSpawnItem(mod.ItemType("DraedonBar"), Main.rand.Next(2, 4));
				player.QuickSpawnItem(mod.ItemType("CruptixBar"), Main.rand.Next(2, 4));
				player.QuickSpawnItem(mod.ItemType("CoreofCinder"), Main.rand.Next(1, 4));
				player.QuickSpawnItem(mod.ItemType("CoreofEleum"), Main.rand.Next(1, 4));
				player.QuickSpawnItem(mod.ItemType("CoreofChaos"), Main.rand.Next(1, 4));
			}
			if (Main.rand.Next(2) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("BloodPact"));
			}
			if (Main.rand.Next(2) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("FleshTotem"));
			}
			switch (Main.rand.Next(5))
			{
				case 0:
					player.QuickSpawnItem(mod.ItemType("Hematemesis"));
					break;
				case 1:
					player.QuickSpawnItem(mod.ItemType("RealmRavager"));
					break;
				case 2:
					player.QuickSpawnItem(mod.ItemType("SpikecragStaff"));
					break;
				case 3:
					player.QuickSpawnItem(mod.ItemType("UltimusCleaver"));
					break;
				case 4:
					player.QuickSpawnItem(mod.ItemType("CraniumSmasher"));
					break;
			}
		}
	}
}
