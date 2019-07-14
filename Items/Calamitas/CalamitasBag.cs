using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.World;

namespace CalamityMod.Items.Calamitas
{
	public class CalamitasBag : ModItem
	{
        public override int BossBagNPC => mod.NPCType("CalamitasRun3");

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
			if (CalamityWorld.revenge)
			{
				player.QuickSpawnItem(mod.ItemType("Animosity"));
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
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("CalamitasInferno"));
			}
			if (Main.rand.Next(7) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("CalamitasMask"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("TheEyeofCalamitas"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("BlightedEyeStaff"));
			}
            if (CalamityWorld.downedProvidence)
            {
                player.QuickSpawnItem(mod.ItemType("Bloodstone"), Main.rand.Next(35, 46));
            }
            if (Main.rand.Next(10) == 0)
            {
                player.QuickSpawnItem(mod.ItemType("ChaosStone"));
            }
            player.QuickSpawnItem(mod.ItemType("CalamityDust"), Main.rand.Next(14, 19));
			player.QuickSpawnItem(mod.ItemType("EssenceofChaos"), Main.rand.Next(5, 10));
			player.QuickSpawnItem(mod.ItemType("BlightedLens"), Main.rand.Next(1, 4));
			player.QuickSpawnItem(mod.ItemType("CalamityRing"));
		}
	}
}
