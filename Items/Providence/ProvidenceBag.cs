using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Providence
{
	public class ProvidenceBag : ModItem
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
			bossBagNPC = mod.NPCType("Providence");
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
				player.QuickSpawnItem(mod.ItemType("BlissfulBombardier"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("HolyCollider"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("MoltenAmputator"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("PurgeGuzzler"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("SolarFlare"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("TelluricGlare"));
			}
			if (Main.rand.Next(7) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("ProvidenceMask"));
			}
			player.QuickSpawnItem(mod.ItemType("UnholyEssence"), Main.rand.Next(25, 36));
            player.QuickSpawnItem(mod.ItemType("DivineGeode"), Main.rand.Next(15, 26));
            player.QuickSpawnItem(mod.ItemType("RuneofCos"));
		}
	}
}