using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.DesertScourge;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class DesertScourgeBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<DesertScourgeHead>();

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 3;
            DisplayName.SetDefault("Treasure Bag (Desert Scourge)");
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
            DropHelper.DropItem(s, player, ModContent.ItemType<VictoryShard>(), 30, 40);
            DropHelper.DropItem(s, player, ItemID.Coral, 30, 40);
            DropHelper.DropItem(s, player, ItemID.Seashell, 30, 40);
            DropHelper.DropItem(s, player, ItemID.Starfish, 30, 40);

            // Weapons
            // Set up the base drop set, which includes Scourge of the Desert at its normal drop chance.
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(s, player,
                DropHelper.WeightStack<AquaticDischarge>(w),
                DropHelper.WeightStack<Barinade>(w),
                DropHelper.WeightStack<StormSpray>(w),
                DropHelper.WeightStack<SeaboundStaff>(w),
                DropHelper.WeightStack<ScourgeoftheDesert>(w),
                DropHelper.WeightStack<AeroStone>(w),
                DropHelper.WeightStack<SandCloak>(w)
            );

            // Equipment
            DropHelper.DropItem(s, player, ModContent.ItemType<OceanCrest>());

            // Vanity
            DropHelper.DropItemChance(s, player, ModContent.ItemType<DesertScourgeMask>(), 7);

            // Fishing
            DropHelper.DropItem(s, player, ModContent.ItemType<SandyAnglingKit>());
        }
    }
}
