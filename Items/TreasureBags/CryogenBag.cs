
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
            item.rare = ItemRarityID.Cyan;
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
            DropHelper.DropItem(player, ModContent.ItemType<EssenceofEleum>(), 5, 9);

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(player,
                DropHelper.WeightStack<Avalanche>(w),
                DropHelper.WeightStack<GlacialCrusher>(w),
                DropHelper.WeightStack<EffluviumBow>(w),
                DropHelper.WeightStack<SnowstormStaff>(w),
                DropHelper.WeightStack<Icebreaker>(w),
				DropHelper.WeightStack<CryoStone>(w),
				DropHelper.WeightStack<FrostFlare>(w)
			);

			// Equipment
			DropHelper.DropItem(player, ModContent.ItemType<SoulofCryogen>());
            DropHelper.DropItemChance(player, ModContent.ItemType<ColdDivinity>(), 0.1f);

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<CryogenMask>(), 7);
        }
    }
}
