using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Accessories;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.World;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Armor.Vanity;
using Terraria.ID;

namespace CalamityMod.Items.TreasureBags
{
    public class BumblebirbBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<Bumblefuck>();

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

        public override bool CanRightClick() => true;

        public override void OpenBossBag(Player player)
        {
            player.TryGettingDevArmor();

            // Materials
            DropHelper.DropItem(player, ModContent.ItemType<EffulgentFeather>(), 15, 21);

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(player,
                DropHelper.WeightStack<GildedProboscis>(w),
                DropHelper.WeightStack<GoldenEagle>(w),
                DropHelper.WeightStack<RougeSlash>(w),
				DropHelper.WeightStack<BirdSeed>(w)
			);

            // Equipment
            DropHelper.DropItemChance(player, ModContent.ItemType<Swordsplosion>(), 0.1f);
            DropHelper.DropItem(player, ModContent.ItemType<DynamoStemCells>());
            DropHelper.DropItemCondition(player, ModContent.ItemType<RedLightningContainer>(), CalamityWorld.revenge && !player.Calamity().rageBoostThree);

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<BumblefuckMask>(), 7);
        }
    }
}
