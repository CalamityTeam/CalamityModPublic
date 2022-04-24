using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using SCalBoss = CalamityMod.NPCs.SupremeCalamitas.SupremeCalamitas;

namespace CalamityMod.Items.TreasureBags
{
    public class SCalBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<SCalBoss>();

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
            DisplayName.SetDefault("Treasure Coffer (Supreme Calamitas)");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
        }

        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.Purple;
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

            player.TryGettingDevArmor(s);

            // Materials
            DropHelper.DropItem(s, player, ModContent.ItemType<CalamitousEssence>(), 25, 35);

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(s, player,
                DropHelper.WeightStack<Violence>(w),
                DropHelper.WeightStack<Condemnation>(w),
                DropHelper.WeightStack<Heresy>(w),
                DropHelper.WeightStack<Vehemenc>(w),
                DropHelper.WeightStack<Perdition>(w),
                DropHelper.WeightStack<Vigilance>(w),
                DropHelper.WeightStack<Sacrifice>(w)
                );

            // Equipment
            DropHelper.DropItem(s, player, ModContent.ItemType<Calamity>());

            // Vanity

            // SCal vanity set (This drops all at once, or not at all)
            if (Main.rand.NextBool(7))
            {
                DropHelper.DropItem(s, player, ModContent.ItemType<AshenHorns>());
                DropHelper.DropItem(s, player, ModContent.ItemType<SCalMask>());
                DropHelper.DropItem(s, player, ModContent.ItemType<SCalRobes>());
                DropHelper.DropItem(s, player, ModContent.ItemType<SCalBoots>());
            }

            // The Brimstone Jewel is an expert-only vanity
            DropHelper.DropItem(s, player, ModContent.ItemType<BrimstoneJewel>());
        }
    }
}
