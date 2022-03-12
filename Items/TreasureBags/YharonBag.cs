using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.Yharon;
using CalamityMod.World;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.Materials;

namespace CalamityMod.Items.TreasureBags
{
    public class YharonBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<Yharon>();

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

        public override bool CanRightClick() => true;

        public override void OpenBossBag(Player player)
        {
            player.TryGettingDevArmor();

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(player,
                DropHelper.WeightStack<DragonRage>(w),
                DropHelper.WeightStack<TheBurningSky>(w),
                DropHelper.WeightStack<DragonsBreath>(w),
                DropHelper.WeightStack<ChickenCannon>(w),
                DropHelper.WeightStack<PhoenixFlameBarrage>(w),
                DropHelper.WeightStack<AngryChickenStaff>(w), // Yharon Kindle Staff
                DropHelper.WeightStack<ProfanedTrident>(w), // Infernal Spear
                DropHelper.WeightStack<FinalDawn>(w)
			);

			// Equipment
			DropHelper.DropItem(player, ModContent.ItemType<DrewsWings>());
            DropHelper.DropItem(player, ModContent.ItemType<YharimsGift>());
            DropHelper.DropItemChance(player, ModContent.ItemType<YharimsCrystal>(), 0.1f);

            int soulFragMin = 22;
            int soulFragMax = 28;
            DropHelper.DropItem(player, ModContent.ItemType<HellcasterFragment>(), soulFragMin, soulFragMax);

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<YharonMask>(), 7);
            DropHelper.DropItemChance(player, ModContent.ItemType<ForgottenDragonEgg>(), 10);
            DropHelper.DropItemChance(player, ModContent.ItemType<McNuggets>(), 10);
            DropHelper.DropItemCondition(player, ModContent.ItemType<FoxDrive>(), CalamityWorld.revenge);
        }
    }
}
