using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.Calamitas;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.TreasureBags
{
    public class CalamitasBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<CalamitasRun3>();

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
            DisplayName.SetDefault("Treasure Bag (Calamitas)");
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
            DropHelper.DropItem(s, player, ModContent.ItemType<CalamityDust>(), 14, 18);
            DropHelper.DropItem(s, player, ModContent.ItemType<EssenceofChaos>(), 5, 9);
            DropHelper.DropItemCondition(s, player, ModContent.ItemType<Bloodstone>(), DownedBossSystem.downedProvidence, 35, 45);

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(s, player,
                DropHelper.WeightStack<TheEyeofCalamitas>(w),
                DropHelper.WeightStack<Animosity>(w),
                DropHelper.WeightStack<CalamitasInferno>(w),
                DropHelper.WeightStack<BlightedEyeStaff>(w),
                DropHelper.WeightStack<ChaosStone>(w)
            );

            // Equipment
            DropHelper.DropItem(s, player, ModContent.ItemType<VoidofCalamity>());
            DropHelper.DropItemChance(s, player, ModContent.ItemType<Regenator>(), 0.1f);

            // Vanity
            DropHelper.DropItemChance(s, player, ModContent.ItemType<CalamitasMask>(), 7);
            if (Main.rand.NextBool(10))
            {
                DropHelper.DropItem(s, player, ModContent.ItemType<CalamityHood>());
                DropHelper.DropItem(s, player, ModContent.ItemType<CalamityRobes>());
            }
        }
    }
}
