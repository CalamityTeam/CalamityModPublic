using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.PlaguebringerGoliath
{
	public class PlaguebringerGoliathBag : ModItem
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
			bossBagNPC = mod.NPCType("PlaguebringerGoliath");
		}

		public override bool CanRightClick()
		{
			return true;
		}

		public override void OpenBossBag(Player player)
		{
			player.TryGettingDevArmor();
			if(Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("PestilentDefiler"));
			}
			if(Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("ThePlaguebringer"));
			}
			if(Main.rand.Next(7) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("PlaguebringerGoliathMask"));
			}
			if(Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("TheHive"));
			}
			if(Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("Malevolence"));
			}
			if(Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("DiseasedPike"));
			}
			if(Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("VirulentKatana"));
			}
			if(Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("MepheticSprayer"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("PlagueStaff"));
			}
			player.QuickSpawnItem(mod.ItemType("ToxicHeart"));
			player.QuickSpawnItem(mod.ItemType("PlagueCellCluster"), Main.rand.Next(13, 18));
		}
	}
}