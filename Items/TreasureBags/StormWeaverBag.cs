using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.NPCs.StormWeaver;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class StormWeaverBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<StormWeaverHead>();

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
            item.rare = ItemRarityID.Cyan;
            item.expert = true;
        }

        public override bool CanRightClick() => true;

        public override void OpenBossBag(Player player)
        {
            // Materials
            DropHelper.DropItem(player, ModContent.ItemType<ArmoredShell>(), 7, 10);

            // Weapons
			DropHelper.DropItemChance(player, ModContent.ItemType<TheStorm>(), DropHelper.BagWeaponDropRateInt);
			DropHelper.DropItemChance(player, ModContent.ItemType<StormDragoon>(), DropHelper.BagWeaponDropRateInt);
			DropHelper.DropItemChance(player, ModContent.ItemType<Thunderstorm>(), DropHelper.BagWeaponDropRateInt);

			// Equipment (None yet boohoo)

			// Vanity
			DropHelper.DropItemChance(player, ModContent.ItemType<StormWeaverMask>(), 7);
			if (Main.rand.NextBool(20))
			{
				DropHelper.DropItem(player, ModContent.ItemType<AncientGodSlayerHelm>());
				DropHelper.DropItem(player, ModContent.ItemType<AncientGodSlayerChestplate>());
				DropHelper.DropItem(player, ModContent.ItemType<AncientGodSlayerLeggings>());
			}

            // Light Pet
            DropHelper.DropItemChance(player, ModContent.ItemType<LittleLight>(), 8);
        }
    }
}
