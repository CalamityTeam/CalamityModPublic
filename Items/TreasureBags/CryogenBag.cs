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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class CryogenBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<Cryogen>();

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 3;
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
            DropHelper.DropItem(s, player, ModContent.ItemType<EssenceofEleum>(), 5, 9);

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(s, player,
                DropHelper.WeightStack<Avalanche>(w),
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
