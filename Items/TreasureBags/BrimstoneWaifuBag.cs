using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.World;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.TreasureBags
{
    public class BrimstoneWaifuBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<BrimstoneElemental>();

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
            DisplayName.SetDefault("Treasure Bag (Brimstone Elemental)");
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
            DropHelper.DropItem(s, player, ModContent.ItemType<EssenceofChaos>(), 5, 9);
            DropHelper.DropItemCondition(s, player, ModContent.ItemType<Bloodstone>(), DownedBossSystem.downedProvidence, 25, 35);

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(s, player,
                DropHelper.WeightStack<Brimlance>(w),
                DropHelper.WeightStack<SeethingDischarge>(w),
                DropHelper.WeightStack<DormantBrimseeker>(w),
                DropHelper.WeightStack<RoseStone>(w)
            );

            // Equipment
            DropHelper.DropItem(s, player, ModContent.ItemType<Abaddon>());
            DropHelper.DropItem(s, player, ModContent.ItemType<Gehenna>());
            DropHelper.DropItemChance(s, player, ModContent.ItemType<Hellborn>(), 0.1f);
            DropHelper.DropItemChance(s, player, ModContent.ItemType<FabledTortoiseShell>(), 0.1f);
            DropHelper.DropItemCondition(s, player, ModContent.ItemType<Brimrose>(), DownedBossSystem.downedProvidence);

            // Vanity
            DropHelper.DropItemCondition(s, player, ModContent.ItemType<CharredRelic>(), CalamityWorld.revenge);
            DropHelper.DropItemChance(s, player, ModContent.ItemType<BrimstoneWaifuMask>(), 7);
        }
    }
}
