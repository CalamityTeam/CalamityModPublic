using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.DesertScourge;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class DesertScourgeBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<DesertScourgeHead>();

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
            DropHelper.DropItem(player, ModContent.ItemType<VictoryShard>(), 10, 16);
            DropHelper.DropItem(player, ItemID.Coral, 7, 11);
            DropHelper.DropItem(player, ItemID.Seashell, 7, 11);
            DropHelper.DropItem(player, ItemID.Starfish, 7, 11);

            // Weapons
			// Scourge of the Desert is separate due to RIV drop interactions
			int[] weapons = new int[] {
				ModContent.ItemType<AquaticDischarge>(),
				ModContent.ItemType<Barinade>(),
				ModContent.ItemType<StormSpray>(),
				ModContent.ItemType<SeaboundStaff>()
			};

			bool droppedWeapon = false;
			for (int i = 0; i < weapons.Length; i++)
			{
				if (DropHelper.DropItemChance(player, weapons[i], 3) > 0)
					droppedWeapon = true;
			}

			if (DropHelper.DropItemRIV(player, ModContent.ItemType<ScourgeoftheDesert>(), ModContent.ItemType<DuneHopper>(), 0.3333f, DropHelper.RareVariantDropRateFloat))
				droppedWeapon = true;

			if (!droppedWeapon)
			{
				// Can't choose anything from an empty array.
				if (weapons is null || weapons.Length == 0)
					goto SKIPDROPS;

				// Resize the array and add the last weapon
				Array.Resize(ref weapons, weapons.Length + 1);
				weapons[weapons.Length - 1] = ModContent.ItemType<ScourgeoftheDesert>();

				// Choose which item to drop.
				int itemID = Main.rand.Next(weapons);
				if (itemID == ModContent.ItemType<ScourgeoftheDesert>() && Main.rand.NextBool(DropHelper.RareVariantDropRateInt))
					itemID = ModContent.ItemType<DuneHopper>();

				DropHelper.DropItem(player, itemID);
			}
			SKIPDROPS:

            // Equipment
            DropHelper.DropItem(player, ModContent.ItemType<OceanCrest>());
            DropHelper.DropItemChance(player, ModContent.ItemType<AeroStone>(), 9);
            DropHelper.DropItemChance(player, ModContent.ItemType<SandCloak>(), 9);
            DropHelper.DropItemChance(player, ModContent.ItemType<DeepDiver>(), DropHelper.RareVariantDropRateInt);

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<DesertScourgeMask>(), 7);

            // Fishing
			DropHelper.DropItem(player, ModContent.ItemType<SandyAnglingKit>());
        }
    }
}
