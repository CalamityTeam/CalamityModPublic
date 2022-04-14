using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.OldDuke;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.TreasureBags
{
    public class OldDukeBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<OldDuke>();

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
            DisplayName.SetDefault("Treasure Bag (The Old Duke)");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
        }

        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 24;
            Item.height = 24;
            Item.expert = true;
            Item.rare = ItemRarityID.Red;
        }

        public override bool CanRightClick() => true;

        public override void PostUpdate() => CalamityUtils.ForceItemIntoWorld(Item);

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
                DropHelper.WeightStack<InsidiousImpaler>(w),
                DropHelper.WeightStack<FetidEmesis>(w),
                DropHelper.WeightStack<SepticSkewer>(w),
                DropHelper.WeightStack<VitriolicViper>(w),
                DropHelper.WeightStack<CadaverousCarrion>(w),
                DropHelper.WeightStack<ToxicantTwister>(w),
                DropHelper.WeightStack<DukeScales>(w)
            );

            // Equipment
            DropHelper.DropItem(s, player, ModContent.ItemType<MutatedTruffle>());
            DropHelper.DropItemChance(s, player, ModContent.ItemType<TheReaper>(), 0.1f);

            // Vanity
            DropHelper.DropItemChance(s, player, ModContent.ItemType<OldDukeMask>(), 7);
        }
    }
}
