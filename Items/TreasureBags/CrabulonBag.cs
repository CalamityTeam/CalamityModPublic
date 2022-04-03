
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            // Materials
            DropHelper.DropItem(player, ItemID.GlowingMushroom, 25, 35);
            DropHelper.DropItem(player, ItemID.MushroomGrassSeeds, 5, 10);

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(player,
                DropHelper.WeightStack<MycelialClaws>(w),
                DropHelper.WeightStack<Fungicide>(w),
                DropHelper.WeightStack<HyphaeRod>(w),
                DropHelper.WeightStack<Mycoroot>(w),
                DropHelper.WeightStack<Shroomerang>(w)
            );

            // Equipment
            DropHelper.DropItem(player, ModContent.ItemType<FungalClump>());
            DropHelper.DropItemCondition(player, ModContent.ItemType<MushroomPlasmaRoot>(), CalamityWorld.revenge && !player.Calamity().rageBoostOne);

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<CrabulonMask>(), 7);
        }
    }
}
