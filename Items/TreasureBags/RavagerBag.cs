using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.Ravager;
using CalamityMod.World;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.TreasureBags
{
    public class RavagerBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<RavagerBody>();

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
            DisplayName.SetDefault("Treasure Bag (Ravager)");
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
            var s = player.GetSource_OpenItem(Item.type);

            player.TryGettingDevArmor(s);

            // Materials
            DropHelper.DropItemCondition(s, player, ModContent.ItemType<FleshyGeodeT1>(), !DownedBossSystem.downedProvidence);
            DropHelper.DropItemCondition(s, player, ModContent.ItemType<FleshyGeodeT2>(), DownedBossSystem.downedProvidence);

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(s, player,
                DropHelper.WeightStack<UltimusCleaver>(w),
                DropHelper.WeightStack<RealmRavager>(w),
                DropHelper.WeightStack<Hematemesis>(w),
                DropHelper.WeightStack<SpikecragStaff>(w),
                DropHelper.WeightStack<CraniumSmasher>(w)
            );
            DropHelper.DropItemChance(s, player, ModContent.ItemType<CorpusAvertor>(), 0.05f);

            // Equipment
            DropHelper.DropItemChance(s, player, ModContent.ItemType<BloodPact>(), 0.5f);
            DropHelper.DropItemChance(s, player, ModContent.ItemType<FleshTotem>(), 0.5f);
            DropHelper.DropItemCondition(s, player, ModContent.ItemType<BloodflareCore>(), DownedBossSystem.downedProvidence);
            DropHelper.DropItemCondition(s, player, ModContent.ItemType<InfernalBlood>(), CalamityWorld.revenge && !player.Calamity().rageBoostTwo);
            DropHelper.DropItemChance(s, player, ModContent.ItemType<Vesuvius>(), 0.1f);

            // Vanity
            DropHelper.DropItemChance(s, player, ModContent.ItemType<RavagerMask>(), 7);
        }
    }
}
