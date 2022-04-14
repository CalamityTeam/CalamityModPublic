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
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.TreasureBags
{
    public class YharonBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<Yharon>();

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
            DisplayName.SetDefault("Treasure Bag (Jungle Dragon, Yharon)");
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

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            return CalamityUtils.DrawTreasureBagInWorld(Item, spriteBatch, ref rotation, ref scale, whoAmI);
        }

        public override void OpenBossBag(Player player)
        {
            // IEntitySource my beloathed
            var s = player.GetItemSource_OpenItem(Item.type);

            player.TryGettingDevArmor(s);

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(s, player,
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
            DropHelper.DropItem(s, player, ModContent.ItemType<DrewsWings>());
            DropHelper.DropItem(s, player, ModContent.ItemType<YharimsGift>());
            DropHelper.DropItemChance(s, player, ModContent.ItemType<YharimsCrystal>(), 0.1f);

            int soulFragMin = 22;
            int soulFragMax = 28;
            DropHelper.DropItem(s, player, ModContent.ItemType<HellcasterFragment>(), soulFragMin, soulFragMax);

            // Vanity
            DropHelper.DropItemChance(s, player, ModContent.ItemType<YharonMask>(), 7);
            DropHelper.DropItemChance(s, player, ModContent.ItemType<ForgottenDragonEgg>(), 10);
            DropHelper.DropItemChance(s, player, ModContent.ItemType<McNuggets>(), 10);
            DropHelper.DropItemCondition(s, player, ModContent.ItemType<FoxDrive>(), CalamityWorld.revenge);
        }
    }
}
