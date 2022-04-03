using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.Leviathan;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class LeviathanBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<Siren>();

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

        public override void PostUpdate() => CalamityUtils.ForceItemIntoWorld(Item);

        public override void OpenBossBag(Player player)
        {
            // siren & levi are available PHM, so this check is necessary to keep vanilla consistency
            if (Main.hardMode)
				player.TryGettingDevArmor(player.GetItemSource_OpenItem(Item.type));

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(player,
                DropHelper.WeightStack<Greentide>(w),
                DropHelper.WeightStack<Leviatitan>(w),
                DropHelper.WeightStack<SirensSong>(w),
                DropHelper.WeightStack<Atlantis>(w),
                DropHelper.WeightStack<GastricBelcherStaff>(w),
                DropHelper.WeightStack<BrackishFlask>(w),
                DropHelper.WeightStack<LeviathanTeeth>(w),
                DropHelper.WeightStack<LureofEnthrallment>(w)
            );

            // Equipment
            DropHelper.DropItem(player, ModContent.ItemType<LeviathanAmbergris>());
            DropHelper.DropItemChance(player, ModContent.ItemType<TheCommunity>(), 0.1f);

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<LeviathanMask>(), 7);
            DropHelper.DropItemChance(player, ModContent.ItemType<AnahitaMask>(), 7);

            // Fishing
            DropHelper.DropItemChance(player, ItemID.HotlineFishingHook, 10);
            DropHelper.DropItemChance(player, ItemID.BottomlessBucket, 10);
            DropHelper.DropItemChance(player, ItemID.SuperAbsorbantSponge, 10);
            DropHelper.DropItemChance(player, ItemID.FishingPotion, 5, 5, 8);
            DropHelper.DropItemChance(player, ItemID.SonarPotion, 5, 5, 8);
            DropHelper.DropItemChance(player, ItemID.CratePotion, 5, 5, 8);
        }
    }
}
