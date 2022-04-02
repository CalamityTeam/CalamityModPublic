using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.AstrumDeus;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class AstrumDeusBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<AstrumDeusHeadSpectral>();

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

        public override void PostUpdate() => CalamityUtils.ForceItemIntoWorld(item);

        public override void OpenBossBag(Player player)
        {
            player.TryGettingDevArmor();

            // Materials
            DropHelper.DropItem(player, ModContent.ItemType<Stardust>(), 60, 90);
            DropHelper.DropItem(player, ItemID.FallenStar, 30, 50);

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(player,
                DropHelper.WeightStack<TheMicrowave>(w),
                DropHelper.WeightStack<StarSputter>(w),
                DropHelper.WeightStack<Starfall>(w),
                DropHelper.WeightStack<GodspawnHelixStaff>(w),
                DropHelper.WeightStack<RegulusRiot>(w)
            );

            // Equipment
            DropHelper.DropItem(player, ModContent.ItemType<HideofAstrumDeus>());
            DropHelper.DropItemChance(player, ModContent.ItemType<ChromaticOrb>(), 5);

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<AstrumDeusMask>(), 7);
        }
    }
}
