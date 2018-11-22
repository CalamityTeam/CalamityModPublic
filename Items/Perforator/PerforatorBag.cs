using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Perforator
{
	public class PerforatorBag : ModItem
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
			bossBagNPC = mod.NPCType("PerforatorHive");
		}

		public override bool CanRightClick()
		{
			return true;
		}

		public override void OpenBossBag(Player player)
		{
			if (Main.rand.Next(7) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("PerforatorMask"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("Aorta"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("BloodyRupture"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("SausageMaker"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("BloodBath"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("VeinBurster"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("Eviscerator"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("ToothBall"), Main.rand.Next(50, 76));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("BloodClotStaff"));
			}
			player.QuickSpawnItem(mod.ItemType("BloodyWormTooth"));
			player.QuickSpawnItem(ItemID.Vertebrae, Main.rand.Next(10, 21));
			player.QuickSpawnItem(ItemID.CrimtaneBar, Main.rand.Next(9, 15));
			player.QuickSpawnItem(mod.ItemType("BloodSample"), Main.rand.Next(30, 41));
            if (Main.hardMode)
            {
                player.QuickSpawnItem(ItemID.Ichor, Main.rand.Next(15, 31));
            }
		}
	}
}