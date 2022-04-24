using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.TreasureBags
{
    public class PolterghastBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<Polterghast>();

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
            DisplayName.SetDefault("Treasure Bag (Polterghast)");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
        }

        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 32;
            Item.height = 34;
            Item.rare = ItemRarityID.Cyan;
            Item.expert = true;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            return CalamityUtils.DrawTreasureBagInWorld(Item, spriteBatch, ref rotation, ref scale, whoAmI);
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/TreasureBags/PolterghastBagGlow").Value);
        }

        public override bool CanRightClick() => true;

        public override void OpenBossBag(Player player)
        {
            // IEntitySource my beloathed
            var s = player.GetSource_OpenItem(Item.type);

            player.TryGettingDevArmor(s);

            // Materials
            DropHelper.DropItem(s, player, ModContent.ItemType<RuinousSoul>(), 10, 20);
            DropHelper.DropItem(s, player, ModContent.ItemType<Phantoplasm>(), 40, 50);

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(s, player,
                DropHelper.WeightStack<TerrorBlade>(w),
                DropHelper.WeightStack<BansheeHook>(w),
                DropHelper.WeightStack<DaemonsFlame>(w),
                DropHelper.WeightStack<FatesReveal>(w),
                DropHelper.WeightStack<GhastlyVisage>(w),
                DropHelper.WeightStack<EtherealSubjugator>(w),
                DropHelper.WeightStack<GhoulishGouger>(w)
            );

            // Equipment
            DropHelper.DropItem(s, player, ModContent.ItemType<Affliction>());
            DropHelper.DropItemCondition(s, player, ModContent.ItemType<Ectoheart>(), CalamityWorld.revenge && !player.Calamity().adrenalineBoostThree);

            // Vanity
            DropHelper.DropItemChance(s, player, ModContent.ItemType<PolterghastMask>(), 7);
        }
    }
}
