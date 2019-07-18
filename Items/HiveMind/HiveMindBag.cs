using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.World;

namespace CalamityMod.Items.HiveMind
{
	public class HiveMindBag : ModItem
	{
        public override int BossBagNPC => mod.NPCType("HiveMindP2");

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
            DropHelper.DropRevBagAccessories(player);

            // Materials
            DropHelper.DropItem(player, ItemID.RottenChunk, 10, 20);
            DropHelper.DropItem(player, ItemID.DemoniteBar, 9, 14);
            DropHelper.DropItem(player, mod.ItemType("TrueShadowScale"), 30, 40);
            DropHelper.DropItemCondition(player, ItemID.CursedFlame, Main.hardMode, 15, 30);

            // Weapons
            DropHelper.DropItemChance(player, mod.ItemType("PerfectDark"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("LeechingDagger"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("Shadethrower"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("ShadowdropStaff"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("ShaderainStaff"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("DankStaff"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("RotBall"), 3, 50, 75);

            // Equipment
            DropHelper.DropItem(player, mod.ItemType("RottenBrain"));

            // Vanity
            DropHelper.DropItemChance(player, mod.ItemType("HiveMindMask"), 7);
		}
	}
}
