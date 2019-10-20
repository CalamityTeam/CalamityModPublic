using CalamityMod.NPCs;

using CalamityMod.World;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Armor.Vanity;

namespace CalamityMod.Items.TreasureBags
{
    public class CrabulonBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<CrabulonIdle>();

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
            DropHelper.DropItem(player, ItemID.GlowingMushroom, 25, 35);
            DropHelper.DropItem(player, ItemID.MushroomGrassSeeds, 5, 10);

            // Weapons
            DropHelper.DropItemChance(player, ModContent.ItemType<MycelialClaws>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<Fungicide>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<HyphaeRod>(), 3);
            DropHelper.DropItemChance(player, ModContent.ItemType<Mycoroot>(), 3);

            // Equipment
            DropHelper.DropItem(player, ModContent.ItemType<FungalClump>());
            DropHelper.DropItemCondition(player, ModContent.ItemType<MushroomPlasmaRoot>(), CalamityWorld.revenge);

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<CrabulonMask>(), 7);
        }
    }
}
