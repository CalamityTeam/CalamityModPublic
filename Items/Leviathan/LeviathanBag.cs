using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.World;

namespace CalamityMod.Items.Leviathan
{
	public class LeviathanBag : ModItem
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
			item.rare = 9;
			item.expert = true;
			bossBagNPC = mod.NPCType("Siren");
		}

		public override bool CanRightClick()
		{
			return true;
		}

		public override void OpenBossBag(Player player)
		{
			bool hardMode = Main.hardMode;
			if (CalamityWorld.revenge)
			{
				if (Main.rand.Next(100) == 0)
				{
					player.QuickSpawnItem(mod.ItemType("TheCommunity"));
				}
				else if (CalamityWorld.defiled)
				{
					if (Main.rand.Next(20) == 0)
					{
						player.QuickSpawnItem(mod.ItemType("TheCommunity"));
					}
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
			player.QuickSpawnItem(mod.ItemType("EnchantedPearl"));
			if (Main.rand.Next(10) == 0)
			{
				player.QuickSpawnItem(ItemID.HotlineFishingHook);
			}
			if (Main.rand.Next(10) == 0)
			{
				player.QuickSpawnItem(ItemID.BottomlessBucket);
			}
			if (Main.rand.Next(10) == 0)
			{
				player.QuickSpawnItem(ItemID.SuperAbsorbantSponge);
			}
			if (Main.rand.Next(5) == 0)
			{
				player.QuickSpawnItem(ItemID.CratePotion, Main.rand.Next(5, 9));
			}
			if (Main.rand.Next(5) == 0)
			{
				player.QuickSpawnItem(ItemID.FishingPotion, Main.rand.Next(5, 9));
			}
			if (Main.rand.Next(5) == 0)
			{
				player.QuickSpawnItem(ItemID.SonarPotion, Main.rand.Next(5, 9));
			}
			if (!hardMode)
			{
				player.QuickSpawnItem(mod.ItemType("IOU"));
			}
			if (Main.rand.Next(7) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("LeviathanMask"));
			}
			if (hardMode)
			{
				player.TryGettingDevArmor();
				if (Main.rand.Next(3) == 0)
				{
					player.QuickSpawnItem(mod.ItemType("Atlantis")); //done
				}
				if (Main.rand.Next(3) == 0)
				{
					player.QuickSpawnItem(mod.ItemType("BrackishFlask")); //done
				}
				if (Main.rand.Next(3) == 0)
				{
					player.QuickSpawnItem(mod.ItemType("Leviatitan")); //done
				}
				if (Main.rand.Next(3) == 0)
				{
					player.QuickSpawnItem(mod.ItemType("LureofEnthrallment"));
				}
				if (Main.rand.Next(3) == 0)
				{
					player.QuickSpawnItem(mod.ItemType("SirensSong")); //done
				}
				if (Main.rand.Next(3) == 0)
				{
					player.QuickSpawnItem(mod.ItemType("Greentide")); //done
				}
				player.QuickSpawnItem(mod.ItemType("LeviathanAmbergris")); //done
			}
		}
	}
}
