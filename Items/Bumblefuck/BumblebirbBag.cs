using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Bumblefuck
{
	public class BumblebirbBag : ModItem
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
			bossBagNPC = mod.NPCType("Bumblefuck");
		}

		public override bool CanRightClick()
		{
			return true;
		}

		public override void OpenBossBag(Player player)
		{
			if (CalamityWorld.revenge)
			{
				player.QuickSpawnItem(mod.ItemType("RedLightningContainer"));
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
			if (Main.rand.Next(40) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("Swordsplosion"));
			}
			switch (Main.rand.Next(3))
			{
				case 0:
					player.QuickSpawnItem(mod.ItemType("RougeSlash"));
					break;
				case 1:
					player.QuickSpawnItem(mod.ItemType("GildedProboscis"));
					break;
				case 2:
					player.QuickSpawnItem(mod.ItemType("GoldenEagle"));
					break;
			}
			player.QuickSpawnItem(mod.ItemType("EffulgentFeather"), Main.rand.Next(9, 15));
		}
	}
}