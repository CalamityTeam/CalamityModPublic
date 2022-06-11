using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Accessories;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.World;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Armor.Vanity;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.TreasureBags
{
    public class BumblebirbBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<Bumblefuck>();

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 3;
            DisplayName.SetDefault("Treasure Bag (The Dragonfolly)");
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
            DropHelper.DropItem(s, player, ModContent.ItemType<EffulgentFeather>(), 15, 21);

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(s, player,
                DropHelper.WeightStack<GildedProboscis>(w),
                DropHelper.WeightStack<GoldenEagle>(w),
                DropHelper.WeightStack<RougeSlash>(w),
                DropHelper.WeightStack<FollyFeed>(w)
            );

            // Equipment
            DropHelper.DropItemChance(s, player, ModContent.ItemType<Swordsplosion>(), 0.1f);
            DropHelper.DropItem(s, player, ModContent.ItemType<DynamoStemCells>());
            DropHelper.DropItemCondition(s, player, ModContent.ItemType<RedLightningContainer>(), CalamityWorld.revenge && !player.Calamity().rageBoostThree);

            // Vanity
            DropHelper.DropItemChance(s, player, ModContent.ItemType<BumblefuckMask>(), 7);
        }
    }
}
