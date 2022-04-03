using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.Perforator;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class PerforatorBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<PerforatorHive>();

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
            DropHelper.DropItem(player, ModContent.ItemType<BloodSample>(), 45, 60);
            DropHelper.DropItem(player, ItemID.CrimtaneBar, 15, 20);
            DropHelper.DropItem(player, ItemID.Vertebrae, 15, 20);
            DropHelper.DropItemCondition(player, ItemID.Ichor, Main.hardMode, 15, 30);
            DropHelper.DropItem(player, ItemID.CrimsonSeeds, 10, 15);

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(player,
                DropHelper.WeightStack<VeinBurster>(w),
                DropHelper.WeightStack<BloodyRupture>(w),
                DropHelper.WeightStack<SausageMaker>(w),
                DropHelper.WeightStack<Aorta>(w),
                DropHelper.WeightStack<Eviscerator>(w),
                DropHelper.WeightStack<BloodBath>(w),
                DropHelper.WeightStack<BloodClotStaff>(w),
                DropHelper.WeightStack<ToothBall>(w, 50, 75),
                DropHelper.WeightStack<BloodstainedGlove>(w)
            );

            // Equipment
            DropHelper.DropItem(player, ModContent.ItemType<BloodyWormTooth>());

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<PerforatorMask>(), 7);
            DropHelper.DropItemChance(player, ModContent.ItemType<BloodyVein>(), 10);
        }
    }
}
