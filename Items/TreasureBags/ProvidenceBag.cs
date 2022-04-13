using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.Providence;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.TreasureBags
{
    public class ProvidenceBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<Providence>();

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Treasure Bag (Providence, the Profaned Goddess)");
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

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            return CalamityUtils.DrawTreasureBagInWorld(Item, spriteBatch, ref rotation, ref scale, whoAmI);
        }

        public override void OpenBossBag(Player player)
        {
            // IEntitySource my beloathed
            var s = player.GetItemSource_OpenItem(Item.type);

            player.TryGettingDevArmor(s);

            // Materials
            DropHelper.DropItem(s, player, ModContent.ItemType<UnholyEssence>(), 25, 35);
            DropHelper.DropItem(s, player, ModContent.ItemType<DivineGeode>(), 20, 30);

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(s, player,
                DropHelper.WeightStack<HolyCollider>(w),
                DropHelper.WeightStack<SolarFlare>(w),
                DropHelper.WeightStack<TelluricGlare>(w),
                DropHelper.WeightStack<BlissfulBombardier>(w),
                DropHelper.WeightStack<PurgeGuzzler>(w),
                DropHelper.WeightStack<DazzlingStabberStaff>(w),
                DropHelper.WeightStack<MoltenAmputator>(w)
            );

            // Equipment
            DropHelper.DropItem(s, player, ModContent.ItemType<BlazingCore>());
            DropHelper.DropItemChance(s, player, ModContent.ItemType<PristineFury>(), 0.1f);

            // Vanity
            DropHelper.DropItemChance(s, player, ModContent.ItemType<ProvidenceMask>(), 7);
        }
    }
}
