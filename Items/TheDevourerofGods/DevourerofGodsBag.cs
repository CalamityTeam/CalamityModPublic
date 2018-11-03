using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.TheDevourerofGods
{
	public class DevourerofGodsBag : ModItem
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
			bossBagNPC = mod.NPCType("DevourerofGodsHeadS");
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
				player.QuickSpawnItem(mod.ItemType("DeathhailStaff"));
			}
			if (Main.rand.Next(7) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("DevourerofGodsMask"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("Eradicator"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("Excelsus"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("TheObliterator"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("EradicatorMelee"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("Deathwind"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("StaffoftheMechworm"));
			}
            if (CalamityWorld.revenge)
            {
                if (player.GetModPlayer<CalamityPlayer>(mod).fabsolVodka)
                {
                    player.QuickSpawnItem(mod.ItemType("Fabsol"));
                }
                if (CalamityWorld.death && player.difficulty == 2)
                {
                    player.QuickSpawnItem(mod.ItemType("CosmicPlushie"));
                }
            }
            player.QuickSpawnItem(mod.ItemType("CosmiliteBar"), Main.rand.Next(30, 40));
			player.QuickSpawnItem(mod.ItemType("NebulousCore"));
		}
	}
}