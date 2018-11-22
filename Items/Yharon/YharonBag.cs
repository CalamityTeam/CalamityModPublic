using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Yharon
{
	public class YharonBag : ModItem
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
			bossBagNPC = mod.NPCType("Yharon");
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
				player.QuickSpawnItem(mod.ItemType("AngryChickenStaff"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("DragonsBreath"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("PhoenixFlameBarrage"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("DragonRage"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("ProfanedTrident"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("TheBurningSky"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("ChickenCannon"));
			}
			if (Main.rand.Next(7) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("YharonMask"));
			}
            if (CalamityWorld.revenge)
            {
                player.QuickSpawnItem(mod.ItemType("DrewsWings"));
                player.QuickSpawnItem(mod.ItemType("FoxDrive"));
            }
			player.QuickSpawnItem(mod.ItemType("YharimsGift"));
		}
	}
}