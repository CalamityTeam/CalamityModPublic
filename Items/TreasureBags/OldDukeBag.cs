using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.OldDuke;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.TreasureBags
{
    public class OldDukeBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<OldDuke>();

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
            item.rare = ItemRarityID.Red;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void PostUpdate() => CalamityUtils.ForceItemIntoWorld(item);

        public override void OpenBossBag(Player player)
        {
            player.TryGettingDevArmor();

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(player,
                DropHelper.WeightStack<InsidiousImpaler>(w),
                DropHelper.WeightStack<FetidEmesis>(w),
                DropHelper.WeightStack<SepticSkewer>(w),
                DropHelper.WeightStack<VitriolicViper>(w),
                DropHelper.WeightStack<CadaverousCarrion>(w),
                DropHelper.WeightStack<ToxicantTwister>(w),
				DropHelper.WeightStack<DukeScales>(w)
			);

			// Equipment
            DropHelper.DropItem(player, ModContent.ItemType<MutatedTruffle>());
            DropHelper.DropItemChance(player, ModContent.ItemType<TheReaper>(), 0.1f);

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<OldDukeMask>(), 7);
        }
    }
}
