using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.World;

namespace CalamityMod.Items.Astrageldon
{
	public class AstrageldonBag : ModItem
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
			bossBagNPC = mod.NPCType("Astrageldon");
		}

		public override bool CanRightClick()
		{
			return true;
		}

        public override void OpenBossBag(Player player)
        {
			if (CalamityWorld.revenge)
			{
				player.QuickSpawnItem(mod.ItemType("StarlightFuelCell"));
				if (NPC.downedMoonlord)
				{
					player.QuickSpawnItem(mod.ItemType("SquishyBeanMount"));
				}
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
            player.QuickSpawnItem(mod.ItemType("AstralJelly"), Main.rand.Next(12, 17));
            player.QuickSpawnItem(mod.ItemType("Stardust"), Main.rand.Next(30, 41));
            player.QuickSpawnItem(ItemID.FallenStar, Main.rand.Next(30, 51));
			if (Main.rand.Next(5) == 0)
			{
				player.QuickSpawnItem(ItemID.HallowedKey);
			}
			if (Main.rand.Next(4) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("Nebulash"));
			}
			if (Main.rand.Next(7) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("AureusMask"));
			}
        }
	}
}