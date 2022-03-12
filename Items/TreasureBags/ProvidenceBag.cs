using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.Providence;
using CalamityMod.World;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.TreasureBags
{
    public class ProvidenceBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<Providence>();

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
            DropHelper.DropItem(player, ModContent.ItemType<UnholyEssence>(), 25, 35);
            DropHelper.DropItem(player, ModContent.ItemType<DivineGeode>(), 20, 30);

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(player,
                DropHelper.WeightStack<HolyCollider>(w),
                DropHelper.WeightStack<SolarFlare>(w),
                DropHelper.WeightStack<TelluricGlare>(w),
                DropHelper.WeightStack<BlissfulBombardier>(w),
                DropHelper.WeightStack<PurgeGuzzler>(w),
                DropHelper.WeightStack<DazzlingStabberStaff>(w),
                DropHelper.WeightStack<MoltenAmputator>(w)
			);

			// Equipment
			DropHelper.DropItem(player, ModContent.ItemType<BlazingCore>());
            DropHelper.DropItemChance(player, ModContent.ItemType<PristineFury>(), 0.1f);

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<ProvidenceMask>(), 7);
        }
    }
}
