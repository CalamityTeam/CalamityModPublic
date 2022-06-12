using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.PlaguebringerGoliath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class PlaguebringerGoliathBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<PlaguebringerGoliath>();

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 3;
            DisplayName.SetDefault("Treasure Bag (The Plaguebringer Goliath)");
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

            player.TryGettingDevArmor(s);

            // Materials
            DropHelper.DropItem(s, player, ModContent.ItemType<PlagueCellCanister>(), 13, 17);
            DropHelper.DropItem(s, player, ModContent.ItemType<InfectedArmorPlating>(), 16, 20);
            DropHelper.DropItem(s, player, ItemID.Stinger, 4, 8);

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(s, player,
                DropHelper.WeightStack<Virulence>(w), // Virulence
                DropHelper.WeightStack<DiseasedPike>(w),
                DropHelper.WeightStack<Pandemic>(w), // Pandemic
                DropHelper.WeightStack<Malevolence>(w),
                DropHelper.WeightStack<PestilentDefiler>(w),
                DropHelper.WeightStack<TheHive>(w),
                DropHelper.WeightStack<BlightSpewer>(w), // Blight Spewer
                DropHelper.WeightStack<PlagueStaff>(w),
                DropHelper.WeightStack<FuelCellBundle>(w),
                DropHelper.WeightStack<InfectedRemote>(w),
                DropHelper.WeightStack<TheSyringe>(w)
            );

            // Equipment
            DropHelper.DropItem(s, player, ModContent.ItemType<ToxicHeart>());
            DropHelper.DropItemChance(s, player, ModContent.ItemType<Malachite>(), 0.1f);

            // Vanity
            DropHelper.DropItemChance(s, player, ModContent.ItemType<PlaguebringerGoliathMask>(), 7);
            DropHelper.DropItemChance(s, player, ModContent.ItemType<PlagueCaller>(), 10);
        }
    }
}
