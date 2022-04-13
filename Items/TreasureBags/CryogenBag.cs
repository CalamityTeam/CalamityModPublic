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
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.TreasureBags
{
    public class CryogenBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<Cryogen>();

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Treasure Bag (Cryogen)");
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

        public override void OpenBossBag(Player player)
        {
            // IEntitySource my beloathed
            var s = player.GetItemSource_OpenItem(Item.type);

            player.TryGettingDevArmor(s);

            // Materials
            DropHelper.DropItem(s, player, ModContent.ItemType<EssenceofEleum>(), 5, 9);

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(s, player,
                DropHelper.WeightStack<Avalanche>(w),
                DropHelper.WeightStack<GlacialCrusher>(w),
                DropHelper.WeightStack<EffluviumBow>(w),
                DropHelper.WeightStack<SnowstormStaff>(w),
                DropHelper.WeightStack<Icebreaker>(w),
                DropHelper.WeightStack<CryoStone>(w),
                DropHelper.WeightStack<FrostFlare>(w)
            );

            // Equipment
            DropHelper.DropItem(s, player, ModContent.ItemType<SoulofCryogen>());
            DropHelper.DropItemChance(s, player, ModContent.ItemType<ColdDivinity>(), 0.1f);

            // Vanity
            DropHelper.DropItemChance(s, player, ModContent.ItemType<CryogenMask>(), 7);
        }
    }
}
