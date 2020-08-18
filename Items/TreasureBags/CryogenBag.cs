
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class CryogenBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<Cryogen>();

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
            DropHelper.DropItem(player, ModContent.ItemType<CryoBar>(), 20, 40);
            DropHelper.DropItem(player, ModContent.ItemType<EssenceofEleum>(), 5, 9);
            DropHelper.DropItem(player, ItemID.FrostCore);

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(player,
                DropHelper.WeightStack<Avalanche>(w),
                DropHelper.WeightStack<GlacialCrusher>(w),
                DropHelper.WeightStack<EffluviumBow>(w),
                DropHelper.WeightStack<BittercoldStaff>(w),
                DropHelper.WeightStack<SnowstormStaff>(w),
                DropHelper.WeightStack<Icebreaker>(w)
            );

            float divinityChance = DropHelper.LegendaryDropRateFloat;
            DropHelper.DropItemCondition(player, ModContent.ItemType<ColdDivinity>(), CalamityWorld.revenge, divinityChance);

            // Equipment
            DropHelper.DropItem(player, ModContent.ItemType<SoulofCryogen>());
            DropHelper.DropItemCondition(player, ModContent.ItemType<FrostFlare>(), CalamityWorld.revenge);
            DropHelper.DropItemChance(player, ModContent.ItemType<CryoStone>(), 10);
            DropHelper.DropItemChance(player, ModContent.ItemType<Regenator>(), DropHelper.RareVariantDropRateInt);

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<CryogenMask>(), 7);

            // Other
            DropHelper.DropItemChance(player, ItemID.FrozenKey, 5);
        }
    }
}
