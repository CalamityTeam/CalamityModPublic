using CalamityMod.Items.Accessories;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.AquaticScourge;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Armor.Vanity;

namespace CalamityMod.Items.TreasureBags
{
	public class AquaticScourgeBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<AquaticScourgeHead>();

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

        public override bool CanRightClick()
        {
            return true;
        }

        public override void PostUpdate() => CalamityUtils.ForceItemIntoWorld(item);

        public override void OpenBossBag(Player player)
        {
            // AS is available PHM, so this check is necessary to keep vanilla consistency
            if (Main.hardMode)
                player.TryGettingDevArmor();

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(player,
                DropHelper.WeightStack<SubmarineShocker>(w),
                DropHelper.WeightStack<Barinautical>(w),
                DropHelper.WeightStack<Downpour>(w),
                DropHelper.WeightStack<DeepseaStaff>(w),
                DropHelper.WeightStack<ScourgeoftheSeas>(w),
				DropHelper.WeightStack<CorrosiveSpine>(w)
			);

			// Equipment
			DropHelper.DropItem(player, ModContent.ItemType<AquaticEmblem>());
            DropHelper.DropItemChance(player, ModContent.ItemType<DeepDiver>(), 0.1f);
            DropHelper.DropItemChance(player, ModContent.ItemType<SeasSearing>(), 0.1f);

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<AquaticScourgeMask>(), 7);

            // Fishing
            DropHelper.DropItem(player, ModContent.ItemType<BleachedAnglingKit>());
        }
    }
}
