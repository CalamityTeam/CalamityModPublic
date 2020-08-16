using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.HiveMind;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class HiveMindBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<HiveMindP2>();

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
            // Materials
            DropHelper.DropItem(player, ItemID.RottenChunk, 10, 20);
            DropHelper.DropItem(player, ItemID.DemoniteBar, 9, 14);
            DropHelper.DropItem(player, ModContent.ItemType<TrueShadowScale>(), 30, 40);
            DropHelper.DropItemCondition(player, ItemID.CursedFlame, Main.hardMode, 15, 30);

            // Weapons
			int[] weapons = new int[] {
				ModContent.ItemType<PerfectDark>(),
				ModContent.ItemType<LeechingDagger>(),
				ModContent.ItemType<Shadethrower>(),
				ModContent.ItemType<ShadowdropStaff>(),
				ModContent.ItemType<ShaderainStaff>(),
				ModContent.ItemType<DankStaff>(),
				ModContent.ItemType<RotBall>()
			};

			bool droppedWeapon = false;
			int least = 1;
			int most = 1;
			for (int i = 0; i < weapons.Length; i++)
			{
				if (weapons[i] == ModContent.ItemType<RotBall>())
				{
					least = 50;
					most = 75;
				}
				if (DropHelper.DropItemChance(player, weapons[i], 3, least, most) > 0)
					droppedWeapon = true;
			}

			if (!droppedWeapon)
			{
				// Can't choose anything from an empty array.
				if (weapons is null || weapons.Length == 0)
					goto SKIPDROPS;

				// Choose which item to drop.
				int itemID = Main.rand.Next(weapons);
				if (itemID == ModContent.ItemType<RotBall>())
				{
					least = 50;
					most = 75;
				}

				DropHelper.DropItem(player, itemID, least, most);
			}
			SKIPDROPS:

            // Equipment
            DropHelper.DropItem(player, ModContent.ItemType<RottenBrain>());
            DropHelper.DropItemChance(player, ModContent.ItemType<FilthyGlove>(), 3);

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<HiveMindMask>(), 7);
        }
    }
}
