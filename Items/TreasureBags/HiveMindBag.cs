using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
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
        public override int BossBagNPC => ModContent.NPCType<HiveMind>();

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

        public override bool CanRightClick()
        {
            return true;
        }

        public override void OpenBossBag(Player player)
        {
            // Materials
            DropHelper.DropItem(player, ModContent.ItemType<TrueShadowScale>(), 30, 40);
            DropHelper.DropItem(player, ItemID.DemoniteBar, 9, 14);
            DropHelper.DropItem(player, ItemID.RottenChunk, 10, 20);
            DropHelper.DropItemCondition(player, ItemID.CursedFlame, Main.hardMode, 15, 30);
            DropHelper.DropItem(player, ItemID.CorruptSeeds, 10, 15);

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(player,
                DropHelper.WeightStack<PerfectDark>(w),
                DropHelper.WeightStack<LeechingDagger>(w),
                DropHelper.WeightStack<Shadethrower>(w),
                DropHelper.WeightStack<ShadowdropStaff>(w),
                DropHelper.WeightStack<ShaderainStaff>(w),
                DropHelper.WeightStack<DankStaff>(w),
                DropHelper.WeightStack<RotBall>(w, 50, 75),
                DropHelper.WeightStack<FilthyGlove>(w)
            );

            // Equipment
            DropHelper.DropItem(player, ModContent.ItemType<RottenBrain>());

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<HiveMindMask>(), 7);
            DropHelper.DropItemChance(player, ModContent.ItemType<RottingEyeball>(), 10);
        }
    }
}
