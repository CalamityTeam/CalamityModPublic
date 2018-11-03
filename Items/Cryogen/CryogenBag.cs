using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Cryogen
{
	public class CryogenBag : ModItem
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
			bossBagNPC = mod.NPCType("Cryogen");
		}

		public override bool CanRightClick()
		{
			return true;
		}

		public override void OpenBossBag(Player player)
		{
			player.TryGettingDevArmor();
			if (Main.rand.Next(7) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("CryogenMask"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("GlacialCrusher"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("Permafrost"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("BittercoldStaff"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("IceStar"), Main.rand.Next(150, 201));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("EffluviumBow"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("Icebreaker"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("SnowstormStaff"));
			}
            if (Main.rand.Next(5) == 0)
            {
                player.QuickSpawnItem(ItemID.FrozenKey);
            }
            if (Main.rand.Next(10) == 0)
            {
                player.QuickSpawnItem(mod.ItemType("CryoStone"));
            }
            if (CalamityWorld.revenge)
            {
                player.QuickSpawnItem(mod.ItemType("FrostFlare"));
            }
            player.QuickSpawnItem(ItemID.SoulofMight, Main.rand.Next(25, 41));
            player.QuickSpawnItem(mod.ItemType("CryoBar"), Main.rand.Next(20, 41));
			player.QuickSpawnItem(mod.ItemType("EssenceofEleum"), Main.rand.Next(4, 6));
			player.QuickSpawnItem(mod.ItemType("SoulofCryogen"));
			player.QuickSpawnItem(ItemID.FrostCore);
		}
	}
}