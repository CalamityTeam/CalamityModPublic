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
            // AS is available PHM, so this check is necessary to keep vanilla consistency
            if (Main.hardMode)
				player.TryGettingDevArmor(GetItemSource_OpenItem(Item.type));

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
