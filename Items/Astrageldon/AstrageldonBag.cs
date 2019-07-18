using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.World;

namespace CalamityMod.Items.Astrageldon
{
	public class AstrageldonBag : ModItem
	{
        public override int BossBagNPC => mod.NPCType("Astrageldon");

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
		}

		public override bool CanRightClick()
		{
			return true;
		}

        public override void OpenBossBag(Player player)
        {
            player.TryGettingDevArmor();
            DropHelper.DropRevBagAccessories(player);

            // Materials
            DropHelper.DropItem(player, mod.ItemType("AstralJelly"), 12, 16);
            DropHelper.DropItem(player, mod.ItemType("Stardust"), 30, 40);
            DropHelper.DropItem(player, ItemID.FallenStar, 30, 50);

            // Weapons
            DropHelper.DropItemChance(player, mod.ItemType("Nebulash"), 4);

            // Equipment
            DropHelper.DropItemCondition(player, mod.ItemType("SquishyBeanMount"), CalamityWorld.revenge && NPC.downedMoonlord);

            // Vanity
            DropHelper.DropItemChance(player, mod.ItemType("AureusMask"), 7);

            // Other
            DropHelper.DropItemCondition(player, mod.ItemType("StarlightFuelCell"), CalamityWorld.revenge);
            DropHelper.DropItemChance(player, ItemID.HallowedKey, 5);
        }
	}
}
