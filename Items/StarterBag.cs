using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items
{
	public class StarterBag : ModItem
	{
		public override void SetStaticDefaults()
 		{
 			DisplayName.SetDefault("Starter Bag");
 			Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
 		}
		
		public override void SetDefaults()
		{
			item.consumable = true;
			item.width = 24;
			item.height = 24;
			item.rare = 1;
		}

		public override bool CanRightClick()
		{
			return true;
		}

		public override void RightClick(Player player)
		{
            player.QuickSpawnItem(ItemID.CopperBroadsword); //melee needs
            player.QuickSpawnItem(ItemID.CopperBow); //ranged needs
            player.QuickSpawnItem(ItemID.WoodenArrow, 100);
            player.QuickSpawnItem(ItemID.AmethystStaff); //mage needs
			player.QuickSpawnItem(ItemID.ManaCrystal);
			player.QuickSpawnItem(ItemID.Shuriken, 75); //throwing needs
            player.QuickSpawnItem(ItemID.ThrowingKnife, 75);
            player.QuickSpawnItem(ItemID.SlimeStaff); //summoner needs
            player.QuickSpawnItem(ItemID.CopperHammer); //tool needs
            player.QuickSpawnItem(ItemID.MiningPotion); //mining needs
            player.QuickSpawnItem(ItemID.SpelunkerPotion, 2); //mining/treasure needs
            player.QuickSpawnItem(ItemID.SwiftnessPotion, 3); //movement needs
            player.QuickSpawnItem(ItemID.GillsPotion, 2); //abyss needs
            player.QuickSpawnItem(ItemID.ShinePotion); //mining needs
            player.QuickSpawnItem(ItemID.SlimeCrown); //speedruns lul needs
            player.QuickSpawnItem(ItemID.Chest, 3); //storage needs
            player.QuickSpawnItem(ItemID.Torch, 25); //speedruns lul needs
            player.QuickSpawnItem(ItemID.Bomb, 10); //speedruns lul needs
            player.QuickSpawnItem(mod.ItemType("Death"));
            player.QuickSpawnItem(mod.ItemType("DefiledRune"));
        }
	}
}