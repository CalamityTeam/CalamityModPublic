using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Calamitas
{
	public class CalamitasBag : ModItem
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
			bossBagNPC = mod.NPCType("CalamitasRun3");
		}

		public override bool CanRightClick()
		{
			return true;
		}

		public override void OpenBossBag(Player player)
		{
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
                player.QuickSpawnItem(mod.ItemType("Bloodstone"), Main.rand.Next(30, 41));
            }
            if (Main.rand.Next(10) == 0)
            {
                player.QuickSpawnItem(mod.ItemType("ChaosStone"));
            }
            if (CalamityWorld.revenge)
            {
                player.QuickSpawnItem(mod.ItemType("Animosity"));
            }
            player.QuickSpawnItem(mod.ItemType("CalamityDust"), Main.rand.Next(14, 19));
			player.QuickSpawnItem(mod.ItemType("EssenceofChaos"), Main.rand.Next(4, 6));
			player.QuickSpawnItem(mod.ItemType("BlightedLens"), Main.rand.Next(1, 4));
			player.QuickSpawnItem(mod.ItemType("CalamityRing"));
		}
	}
}