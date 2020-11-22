using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.FurnitureCosmilite;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class DevourerofGodsBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<DevourerofGodsHeadS>();

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Treasure Bag");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
        }

        public override void SetDefaults()
        {
            item.maxStack = 999;
            item.consumable = true;
            item.width = 24;
            item.height = 24;
            item.rare = 9;
            item.expert = true;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
			item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.GetTexture("CalamityMod/Items/TreasureBags/DevourerofGodsBagGlow"));
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void PostUpdate() => CalamityUtils.ForceItemIntoWorld(item);

        public override void OpenBossBag(Player player)
        {
            player.TryGettingDevArmor();

            // Materials
            DropHelper.DropItem(player, ModContent.ItemType<CosmiliteBar>(), 30, 39);
            DropHelper.DropItem(player, ModContent.ItemType<CosmiliteBrick>(), 200, 320);

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(player,
                DropHelper.WeightStack<Excelsus>(w),
                DropHelper.WeightStack<TheObliterator>(w),
                DropHelper.WeightStack<Deathwind>(w),
                DropHelper.WeightStack<DeathhailStaff>(w),
                DropHelper.WeightStack<StaffoftheMechworm>(w),
                DropHelper.WeightStack<Eradicator>(w)
            );

            DropHelper.DropItemChance(player, ModContent.ItemType<Skullmasher>(), DropHelper.RareVariantDropRateInt);
            DropHelper.DropItemChance(player, ModContent.ItemType<Norfleet>(), DropHelper.RareVariantDropRateInt);
            float dischargeChance = DropHelper.LegendaryDropRateFloat;
            DropHelper.DropItemCondition(player, ModContent.ItemType<CosmicDischarge>(), CalamityWorld.revenge, dischargeChance);

            // Equipment
            DropHelper.DropItem(player, ModContent.ItemType<NebulousCore>());
            bool vodka = player.Calamity().fabsolVodka;
            DropHelper.DropItemCondition(player, ModContent.ItemType<Fabsol>(), CalamityWorld.revenge && vodka);

            // Vanity
            DropHelper.DropItemChance(player, ModContent.ItemType<DevourerofGodsMask>(), 7);
            DropHelper.DropItemCondition(player, ModContent.ItemType<CosmicPlushie>(), CalamityWorld.death && player.difficulty == 2);
        }
    }
}
