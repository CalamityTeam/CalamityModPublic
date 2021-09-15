using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
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
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(player,
                DropHelper.WeightStack<PhotonRipper>(w),
                DropHelper.WeightStack<SpineOfThanatos>(w),
                DropHelper.WeightStack<SurgeDriver>(w),
                DropHelper.WeightStack<TheJailor>(w),
                DropHelper.WeightStack<RefractionRotor>(w),
                DropHelper.WeightStack<TheAtomSplitter>(w)
            );

            // Equipment
            DropHelper.DropItem(player, ModContent.ItemType<DraedonsHeart>());

            // Vanity
            // DropHelper.DropItemChance(player, ModContent.ItemType<ThanatosMask>(), 7);
            // DropHelper.DropItemChance(player, ModContent.ItemType<ArtemisMask>(), 7);
            // DropHelper.DropItemChance(player, ModContent.ItemType<ApolloMask>(), 7);
            // DropHelper.DropItemChance(player, ModContent.ItemType<AresMask>(), 7);
        }
    }
}
