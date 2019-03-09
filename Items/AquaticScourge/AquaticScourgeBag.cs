using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.AquaticScourge
{
	public class AquaticScourgeBag : ModItem
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
			bossBagNPC = mod.NPCType("AquaticScourgeHead");
		}

		public override bool CanRightClick()
		{
			return true;
		}

		public override void OpenBossBag(Player player)
		{
			if (CalamityWorld.revenge)
			{
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
				player.QuickSpawnItem(mod.ItemType("DeepseaStaff"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("Downpour"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("Barinautical"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("SubmarineShocker"));
			}
            if (Main.rand.Next(15) == 0)
            {
                player.QuickSpawnItem(ItemID.HighTestFishingLine);
            }
            if (Main.rand.Next(30) == 0)
            {
                player.QuickSpawnItem(ItemID.AnglerTackleBag);
            }
            if (Main.rand.Next(15) == 0)
            {
                player.QuickSpawnItem(ItemID.TackleBox);
            }
            if (Main.rand.Next(10) == 0)
            {
                player.QuickSpawnItem(ItemID.AnglerEarring);
            }
            if (Main.rand.Next(10) == 0)
            {
                player.QuickSpawnItem(ItemID.FishermansGuide);
            }
            if (Main.rand.Next(10) == 0)
            {
                player.QuickSpawnItem(ItemID.WeatherRadio);
            }
            if (Main.rand.Next(10) == 0)
            {
                player.QuickSpawnItem(ItemID.Sextant);
            }
            if (Main.rand.Next(5) == 0)
            {
                player.QuickSpawnItem(ItemID.AnglerHat);
            }
            if (Main.rand.Next(5) == 0)
            {
                player.QuickSpawnItem(ItemID.AnglerVest);
            }
            if (Main.rand.Next(5) == 0)
            {
                player.QuickSpawnItem(ItemID.AnglerPants);
            }
            if (Main.rand.Next(5) == 0)
            {
                player.QuickSpawnItem(ItemID.CratePotion, Main.rand.Next(2, 4));
            }
            if (Main.rand.Next(5) == 0)
            {
                player.QuickSpawnItem(ItemID.FishingPotion, Main.rand.Next(2, 4));
            }
            if (Main.rand.Next(5) == 0)
            {
                player.QuickSpawnItem(ItemID.SonarPotion, Main.rand.Next(2, 4));
            }
            if (Main.rand.Next(10) == 0)
            {
                player.QuickSpawnItem(mod.ItemType("AeroStone"));
            }
            if (NPC.downedBoss3)
            {
                if (Main.rand.Next(20) == 0)
                {
                    player.QuickSpawnItem(ItemID.GoldenBugNet);
                }
            }
            player.QuickSpawnItem(mod.ItemType("AquaticEmblem"));
			player.QuickSpawnItem(mod.ItemType("VictoryShard"), Main.rand.Next(15, 26));
			player.QuickSpawnItem(ItemID.Coral, Main.rand.Next(7, 12));
			player.QuickSpawnItem(ItemID.Seashell, Main.rand.Next(7, 12));
			player.QuickSpawnItem(ItemID.Starfish, Main.rand.Next(7, 12));
            player.QuickSpawnItem(ItemID.SoulofSight, Main.rand.Next(25, 41));
        }
	}
}