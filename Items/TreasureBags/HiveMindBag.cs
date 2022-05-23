using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.HiveMind;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class HiveMindBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<HiveMind>();

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 3;
            DisplayName.SetDefault("Treasure Bag (The Hive Mind)");
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
            var s = player.GetSource_OpenItem(Item.type);

            // Materials
            DropHelper.DropItem(s, player, ModContent.ItemType<TrueShadowScale>(), 30, 40);
            DropHelper.DropItem(s, player, ItemID.DemoniteBar, 9, 14);
            DropHelper.DropItem(s, player, ItemID.RottenChunk, 10, 20);
            DropHelper.DropItemCondition(s, player, ItemID.CursedFlame, Main.hardMode, 15, 30);
            DropHelper.DropItem(s, player, ItemID.CorruptSeeds, 10, 15);

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(s, player,
                DropHelper.WeightStack<PerfectDark>(w),
                DropHelper.WeightStack<LeechingDagger>(w),
                DropHelper.WeightStack<Shadethrower>(w),
                DropHelper.WeightStack<ShadowdropStaff>(w),
                DropHelper.WeightStack<ShaderainStaff>(w),
                DropHelper.WeightStack<DankStaff>(w),
                DropHelper.WeightStack<RotBall>(w, 50, 75),
                DropHelper.WeightStack<FilthyGlove>(w)
            );

            // Equipment
            DropHelper.DropItem(s, player, ModContent.ItemType<RottenBrain>());

            // Vanity
            DropHelper.DropItemChance(s, player, ModContent.ItemType<HiveMindMask>(), 7);
            DropHelper.DropItemChance(s, player, ModContent.ItemType<RottingEyeball>(), 10);
        }
    }
}
