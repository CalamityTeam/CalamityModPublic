using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.NPCs.ExoMechs.Ares;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.TreasureBags
{
    public class DraedonTreasureBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<AresBody>();

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Treasure Bag (Exo Mechs)");
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
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        public override bool CanRightClick() => true;

        public override void PostUpdate() => CalamityUtils.ForceItemIntoWorld(Item);

        public override void OpenBossBag(Player player)
        {
            // IEntitySource my beloathed
            var s = player.GetItemSource_OpenItem(Item.type);

            player.TryGettingDevArmor(s);

            // Materials
            DropHelper.DropItem(s, player, ModContent.ItemType<ExoPrism>(), 30, 40);

            // Weapons
            // Higher chance due to how the drops work
            float w = DropHelper.BagWeaponDropRateFloat * 2f;
            if (DownedBossSystem.downedAres)
            {
                DropHelper.DropEntireWeightedSet(s, player,
                    DropHelper.WeightStack<PhotonRipper>(w),
                    DropHelper.WeightStack<TheJailor>(w)
                );
            }
            if (DownedBossSystem.downedThanatos)
            {
                DropHelper.DropEntireWeightedSet(s, player,
                    DropHelper.WeightStack<SpineOfThanatos>(w),
                    DropHelper.WeightStack<RefractionRotor>(w)
                );
            }
            if (DownedBossSystem.downedArtemisAndApollo)
            {
                DropHelper.DropEntireWeightedSet(s, player,
                    DropHelper.WeightStack<SurgeDriver>(w),
                    DropHelper.WeightStack<TheAtomSplitter>(w)
                );
            }

            // Equipment
            DropHelper.DropItem(s, player, ModContent.ItemType<DraedonsHeart>());
            DropHelper.DropItem(s, player, ModContent.ItemType<ExoThrone>());

            // Vanity
            // Higher chance due to how the drops work
            float maskDropRate = 1f / 3.5f;
            if (DownedBossSystem.downedThanatos)
                DropHelper.DropItemChance(s, player, ModContent.ItemType<ThanatosMask>(), maskDropRate);

            if (DownedBossSystem.downedArtemisAndApollo)
            {
                DropHelper.DropItemChance(s, player, ModContent.ItemType<ArtemisMask>(), maskDropRate);
                DropHelper.DropItemChance(s, player, ModContent.ItemType<ApolloMask>(), maskDropRate);
            }

            if (DownedBossSystem.downedAres)
                DropHelper.DropItemChance(s, player, ModContent.ItemType<AresMask>(), maskDropRate);

            DropHelper.DropItemChance(s, player, ModContent.ItemType<DraedonMask>(), maskDropRate);
        }
    }
}
