using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class CrabulonBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<Crabulon>();

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 3;
            DisplayName.SetDefault("Treasure Bag (Crabulon)");
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
            DropHelper.DropItem(s, player, ItemID.GlowingMushroom, 25, 35);
            DropHelper.DropItem(s, player, ItemID.MushroomGrassSeeds, 5, 10);

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(s, player,
                DropHelper.WeightStack<MycelialClaws>(w),
                DropHelper.WeightStack<Fungicide>(w),
                DropHelper.WeightStack<HyphaeRod>(w),
                DropHelper.WeightStack<Mycoroot>(w),
                DropHelper.WeightStack<InfestedClawmerang>(w)
            );

            // Equipment
            DropHelper.DropItem(s, player, ModContent.ItemType<FungalClump>());
            DropHelper.DropItemCondition(s, player, ModContent.ItemType<MushroomPlasmaRoot>(), CalamityWorld.revenge && !player.Calamity().rageBoostOne);

            // Vanity
            DropHelper.DropItemChance(s, player, ModContent.ItemType<CrabulonMask>(), 7);
        }
    }
}
