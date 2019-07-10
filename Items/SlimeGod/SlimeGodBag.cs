using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.Items.SlimeGod
{
	public class SlimeGodBag : ModItem
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
			bossBagNPC = mod.NPCType("SlimeGodRun");
		}

		public override bool CanRightClick()
		{
			return true;
		}

		public override void OpenBossBag(Player player)
		{
			if (CalamityWorld.revenge)
			{
				player.QuickSpawnItem(mod.ItemType("ElectrolyteGelPack"));
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
			int maskChoice = Main.rand.Next(2);
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("OverloadedBlaster"));
			}
			if (Main.rand.Next(7) == 0)
			{
				if (maskChoice == 0)
				{
					player.QuickSpawnItem(mod.ItemType("SlimeGodMask"));
				}
				else
				{
					player.QuickSpawnItem(mod.ItemType("SlimeGodMask2"));
				}
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("AbyssalTome"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("EldritchTome"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("CrimslimeStaff"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("CorroslimeStaff"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("GelDart"), Main.rand.Next(100, 126));
			}
			player.QuickSpawnItem(mod.ItemType("StaticRefiner"));
			player.QuickSpawnItem(mod.ItemType("ManaOverloader"));
			player.QuickSpawnItem(ItemID.Gel, Main.rand.Next(150, 201));
			player.QuickSpawnItem(mod.ItemType("PurifiedGel"), Main.rand.Next(30, 51));
		}
	}
}
