using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

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
            player.TryGettingDevArmor();
            player.QuickSpawnItem(mod.ItemType("AstralJelly"), Main.rand.Next(12, 17));
            player.QuickSpawnItem(mod.ItemType("Stardust"), Main.rand.Next(30, 41));
            player.QuickSpawnItem(ItemID.FallenStar, Main.rand.Next(30, 51));
            if (NPC.downedMoonlord)
            {
                if (CalamityWorld.revenge)
                {
                    player.QuickSpawnItem(mod.ItemType("SquishyBeanMount"));
                }
            }
        }
	}
}