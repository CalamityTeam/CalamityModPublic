using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class DraedonTreasureBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<AresBody>();

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

        public override void PostUpdate() => CalamityUtils.ForceItemIntoWorld(item);

        public override void OpenBossBag(Player player)
        {
            player.TryGettingDevArmor();

            // Materials
            DropHelper.DropItem(player, ModContent.ItemType<ExoPrism>(), 30, 40);

            // Weapons
            // Higher chance due to how the drops work
            float w = DropHelper.BagWeaponDropRateFloat * 2f;
            if (CalamityWorld.downedAres)
            {
                DropHelper.DropEntireWeightedSet(player,
                    DropHelper.WeightStack<PhotonRipper>(w),
                    DropHelper.WeightStack<TheJailor>(w)
                );
            }
            if (CalamityWorld.downedThanatos)
            {
                DropHelper.DropEntireWeightedSet(player,
                    DropHelper.WeightStack<SpineOfThanatos>(w),
                    DropHelper.WeightStack<RefractionRotor>(w)
                );
            }
            if (CalamityWorld.downedArtemisAndApollo)
            {
                DropHelper.DropEntireWeightedSet(player,
                    DropHelper.WeightStack<SurgeDriver>(w),
                    DropHelper.WeightStack<TheAtomSplitter>(w)
                );
            }

            // Equipment
            DropHelper.DropItem(player, ModContent.ItemType<DraedonsHeart>());
            DropHelper.DropItem(player, ModContent.ItemType<ExoThrone>());

            // Vanity
            // Higher chance due to how the drops work
            float maskDropRate = 1f / 3.5f;
            if (CalamityWorld.downedThanatos)
                DropHelper.DropItemChance(player, ModContent.ItemType<ThanatosMask>(), maskDropRate);

            if (CalamityWorld.downedArtemisAndApollo)
            {
                DropHelper.DropItemChance(player, ModContent.ItemType<ArtemisMask>(), maskDropRate);
                DropHelper.DropItemChance(player, ModContent.ItemType<ApolloMask>(), maskDropRate);
            }

            if (CalamityWorld.downedAres)
                DropHelper.DropItemChance(player, ModContent.ItemType<AresMask>(), maskDropRate);

            DropHelper.DropItemChance(player, ModContent.ItemType<DraedonMask>(), maskDropRate);
        }
    }
}
