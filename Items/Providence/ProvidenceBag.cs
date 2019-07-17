using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.World;

namespace CalamityMod.Items.Providence
{
	public class ProvidenceBag : ModItem
	{
        public override int BossBagNPC => mod.NPCType("Providence");

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
		}

		public override bool CanRightClick()
		{
			return true;
		}

		public override void OpenBossBag(Player player)
		{
            player.TryGettingDevArmor();
            DropHelper.DropRevBagAccessories(player);

            DropHelper.DropItemChance(player, mod.ItemType("HolyCollider"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("SolarFlare"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("TelluricGlare"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("BlissfulBombardier"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("PurgeGuzzler"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("MoltenAmputator"), 3);

            DropHelper.DropItemChance(player, mod.ItemType("SamuraiBadge"), 40);

            DropHelper.DropItemChance(player, mod.ItemType("ProvidenceMask"), 7);

            DropHelper.DropItem(player, mod.ItemType("UnholyEssence"), 25, 35);
            DropHelper.DropItem(player, mod.ItemType("DivineGeode"), 15, 25);

            DropHelper.DropItem(player, mod.ItemType("RuneofCos"));
		}
	}
}
