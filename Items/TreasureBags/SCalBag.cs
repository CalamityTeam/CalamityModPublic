using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

using SCalBoss = CalamityMod.NPCs.SupremeCalamitas.SupremeCalamitas;

namespace CalamityMod.Items.TreasureBags
{
    public class SCalBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<SCalBoss>();

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Treasure Coffer");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
        }

        public override void SetDefaults()
        {
            item.maxStack = 999;
            item.consumable = true;
            item.width = 24;
            item.height = 24;
            item.rare = ItemRarityID.Purple;
            item.expert = true;
        }

        public override bool CanRightClick() => true;

        public override void OpenBossBag(Player player)
        {
            player.TryGettingDevArmor();

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(player,
                DropHelper.WeightStack<Vehemenc>(w),
                DropHelper.WeightStack<Heresy>(w),
                DropHelper.WeightStack<Perdition>(w),
                DropHelper.WeightStack<Vigilance>(w),
                DropHelper.WeightStack<Sacrifice>(w),
                DropHelper.WeightStack<Violence>(w)
            );

            // Equipment
            DropHelper.DropItem(player, ModContent.ItemType<Calamity>());

            // Vanity
            if (Main.rand.NextBool(7))
            {
                DropHelper.DropItem(player, ModContent.ItemType<AshenHorns>());
                DropHelper.DropItem(player, ModContent.ItemType<SCalMask>());
                DropHelper.DropItem(player, ModContent.ItemType<SCalRobes>());
                DropHelper.DropItem(player, ModContent.ItemType<SCalBoots>());
            }
        }
    }
}
