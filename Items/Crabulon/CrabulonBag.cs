using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Crabulon
{
	public class CrabulonBag : ModItem
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
			bossBagNPC = mod.NPCType("CrabulonIdle");
		}

		public override bool CanRightClick()
		{
			return true;
		}

		public override void OpenBossBag(Player player)
		{
            if (Main.rand.Next(7) == 0)
            {
                player.QuickSpawnItem(mod.ItemType("CrabulonMask"));
            }
            if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("HyphaeRod"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("MycelialClaws"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("Mycoroot"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("Fungicide"));
			}
			player.QuickSpawnItem(ItemID.GlowingMushroom, Main.rand.Next(25, 36));
			player.QuickSpawnItem(ItemID.MushroomGrassSeeds, Main.rand.Next(5, 11));
			player.QuickSpawnItem(mod.ItemType("FungalClump"));
            if (CalamityWorld.revenge)
            {
                player.QuickSpawnItem(mod.ItemType("MushroomPlasmaRoot"));
            }
		}
	}
}