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
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.Cyan;
            Item.expert = true;
        }

        public override bool CanRightClick() => true;

        public override void OpenBossBag(Player player)
        {
            player.TryGettingDevArmor(player.GetItemSource_OpenItem(Item.type));

            // Materials
            DropHelper.DropItem(player, ModContent.ItemType<ArmoredShell>(), 7, 10);

            // Weapons
            DropHelper.DropItemChance(player, ModContent.ItemType<TheStorm>(), DropHelper.BagWeaponDropRateInt);
            DropHelper.DropItemChance(player, ModContent.ItemType<StormDragoon>(), DropHelper.BagWeaponDropRateInt);
            DropHelper.DropItemChance(player, ModContent.ItemType<Thunderstorm>(), 0.1f);

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
