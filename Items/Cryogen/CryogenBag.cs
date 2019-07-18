using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.World;

namespace CalamityMod.Items.Cryogen
{
	public class CryogenBag : ModItem
	{
        public override int BossBagNPC => mod.NPCType("Cryogen");

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

            // Materials
            DropHelper.DropItem(player, ItemID.SoulofMight, 25, 40);
            DropHelper.DropItem(player, mod.ItemType("CryoBar"), 20, 40);
            DropHelper.DropItem(player, mod.ItemType("EssenceofEleum"), 5, 9);
            DropHelper.DropItem(player, ItemID.FrostCore);

            // Weapons
            DropHelper.DropItemChance(player, mod.ItemType("Avalanche"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("GlacialCrusher"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("EffluviumBow"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("BittercoldStaff"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("SnowstormStaff"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("Icebreaker"), 3);
            DropHelper.DropItemChance(player, mod.ItemType("IceStar"), 3, 150, 200);

            // Equipment
            DropHelper.DropItem(player, mod.ItemType("SoulfofCryogen"));
            DropHelper.DropItemCondition(player, mod.ItemType("FrostFlare"), CalamityWorld.revenge);
            DropHelper.DropItemChance(player, mod.ItemType("CryoStone"), 10);
            DropHelper.DropItemChance(player, mod.ItemType("Regenator"), DropHelper.RareVariantDropRateInt);

            // Vanity
            DropHelper.DropItemChance(player, mod.ItemType("CryogenMask"), 7);

            // Other
            DropHelper.DropItemChance(player, ItemID.FrozenKey, 5);
		}
	}
}
