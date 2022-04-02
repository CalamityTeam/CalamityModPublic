using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.Ravager;
using CalamityMod.World;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.TreasureBags
{
    public class RavagerBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<RavagerBody>();

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
            item.expert = true;
            item.rare = ItemRarityID.Cyan;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void OpenBossBag(Player player)
        {
            player.TryGettingDevArmor();

            // Materials
            DropHelper.DropItemCondition(player, ModContent.ItemType<FleshyGeodeT1>(), !CalamityWorld.downedProvidence);
            DropHelper.DropItemCondition(player, ModContent.ItemType<FleshyGeodeT2>(), CalamityWorld.downedProvidence);

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(player,
                DropHelper.WeightStack<UltimusCleaver>(w),
                DropHelper.WeightStack<RealmRavager>(w),
                DropHelper.WeightStack<Hematemesis>(w),
                DropHelper.WeightStack<SpikecragStaff>(w),
                DropHelper.WeightStack<CraniumSmasher>(w)
            );
            DropHelper.DropItemChance(player, ModContent.ItemType<CorpusAvertor>(), 0.05f);

            // Equipment
            DropHelper.DropItemChance(player, ModContent.ItemType<BloodPact>(), 0.5f);
            DropHelper.DropItemChance(player, ModContent.ItemType<FleshTotem>(), 0.5f);
            DropHelper.DropItemCondition(player, ModContent.ItemType<BloodflareCore>(), CalamityWorld.downedProvidence);
            DropHelper.DropItemCondition(player, ModContent.ItemType<InfernalBlood>(), CalamityWorld.revenge && !player.Calamity().rageBoostTwo);
            DropHelper.DropItemChance(player, ModContent.ItemType<Vesuvius>(), 0.1f);

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<RavagerMask>(), 7);
        }
    }
}
