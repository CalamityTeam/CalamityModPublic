using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.AstrumDeus;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.TreasureBags
{
    public class AstrumDeusBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<AstrumDeusHeadSpectral>();

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Treasure Bag (Astrum Deus)");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
        }

        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 24;
            Item.height = 24;
            Item.expert = true;
            Item.rare = ItemRarityID.Cyan;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
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

            // Materials
            DropHelper.DropItem(s, player, ModContent.ItemType<Stardust>(), 60, 90);
            DropHelper.DropItem(s, player, ItemID.FallenStar, 30, 50);

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(s, player,
                DropHelper.WeightStack<TheMicrowave>(w),
                DropHelper.WeightStack<StarSputter>(w),
                DropHelper.WeightStack<Starfall>(w),
                DropHelper.WeightStack<GodspawnHelixStaff>(w),
                DropHelper.WeightStack<RegulusRiot>(w)
            );

            // Equipment
            DropHelper.DropItem(s, player, ModContent.ItemType<HideofAstrumDeus>());
            DropHelper.DropItemChance(s, player, ModContent.ItemType<ChromaticOrb>(), 5);

            // Vanity
            DropHelper.DropItemChance(s, player, ModContent.ItemType<AstrumDeusMask>(), 7);
        }
    }
}
