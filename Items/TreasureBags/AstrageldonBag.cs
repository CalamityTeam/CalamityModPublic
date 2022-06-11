using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.TreasureBags
{
    public class AstrageldonBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<AstrumAureus>();

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 3;
            DisplayName.SetDefault("Treasure Bag (Astrum Aureus)");
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
            DropHelper.DropItem(s, player, ModContent.ItemType<AureusCell>(), 12, 16);
            DropHelper.DropItem(s, player, ModContent.ItemType<Stardust>(), 30, 40);
            DropHelper.DropItem(s, player, ItemID.FallenStar, 20, 30);

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(s, player,
                DropHelper.WeightStack<Nebulash>(w),
                DropHelper.WeightStack<AuroraBlazer>(w),
                DropHelper.WeightStack<AlulaAustralis>(w),
                DropHelper.WeightStack<BorealisBomber>(w),
                DropHelper.WeightStack<AuroradicalThrow>(w)
            );

            // Equipment
            DropHelper.DropItemCondition(s, player, ModContent.ItemType<SuspiciousLookingJellyBean>(), NPC.downedMoonlord);
            DropHelper.DropItem(s, player, ModContent.ItemType<GravistarSabaton>());
            DropHelper.DropItemChance(s, player, ModContent.ItemType<LeonidProgenitor>(), 0.1f);

            // Vanity
            DropHelper.DropItemChance(s, player, ModContent.ItemType<AstrumAureusMask>(), 7);

            // Other
            DropHelper.DropItemCondition(s, player, ModContent.ItemType<StarlightFuelCell>(), CalamityWorld.revenge && !player.Calamity().adrenalineBoostTwo);
        }
    }
}
