using Terraria;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.Items.Polterghast
{
	public class PolterghastBag : ModItem
	{
        public override int BossBagNPC => mod.NPCType("Polterghast");

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

            // Materials
            DropHelper.DropItem(player, mod.ItemType("RuinousSoul"), 6, 10);

            // Weapons
            DropHelper.DropItemChance(player, mod.ItemType("TerrorBlade"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("BansheeHook"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("DaemonsFlame"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("FatesReveal"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("GhastlyVisage"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("EtherealSubjugator"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("GhoulishGouger"), 3);

            // Equipment
            DropHelper.DropItem(player, mod.ItemType("Affliction"));
            DropHelper.DropItemCondition(player, mod.ItemType("Ectoheart"), CalamityWorld.revenge);

            // Vanity
            // there is no Polterghast mask yet
		}
	}
}
