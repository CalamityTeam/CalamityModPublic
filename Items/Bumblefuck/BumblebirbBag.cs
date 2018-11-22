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
			player.TryGettingDevArmor();
            player.QuickSpawnItem(mod.ItemType("EffulgentFeather"), Main.rand.Next(9, 15));
            int choice = Main.rand.Next(3);
            if (choice == 0)
            {
                player.QuickSpawnItem(mod.ItemType("RougeSlash"));
            }
            else if (choice == 1)
            {
                player.QuickSpawnItem(mod.ItemType("GildedProboscis"));
            }
            else
            {
                player.QuickSpawnItem(mod.ItemType("GoldenEagle"));
            }
		}
	}
}