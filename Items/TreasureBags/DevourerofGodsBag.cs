using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
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
using Terraria.ID;

namespace CalamityMod.Items.TreasureBags
{
    public class DevourerofGodsBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<DevourerofGodsHead>();

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 3;
            DisplayName.SetDefault("Treasure Bag (The Devourer of Gods)");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
        }

        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 36;
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
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/TreasureBags/DevourerofGodsBagGlow").Value);
        }

        public override bool CanRightClick() => true;

        public override void PostUpdate() => CalamityUtils.ForceItemIntoWorld(Item);

        public override void OpenBossBag(Player player)
        {
            // IEntitySource my beloathed
            var s = player.GetSource_OpenItem(Item.type);

            player.TryGettingDevArmor(s);

            // Materials
            DropHelper.DropItem(s, player, ModContent.ItemType<CosmiliteBar>(), 30, 40);
            DropHelper.DropItem(s, player, ModContent.ItemType<CosmiliteBrick>(), 200, 320);

            // Weapons
            float w = DropHelper.BagWeaponDropRateFloat;
            DropHelper.DropEntireWeightedSet(s, player,
                DropHelper.WeightStack<Excelsus>(w),
                DropHelper.WeightStack<TheObliterator>(w),
                DropHelper.WeightStack<Deathwind>(w),
                DropHelper.WeightStack<DeathhailStaff>(w),
                DropHelper.WeightStack<StaffoftheMechworm>(w),
                DropHelper.WeightStack<Eradicator>(w)
            );

            // Equipment
            DropHelper.DropItem(s, player, ModContent.ItemType<NebulousCore>());
            DropHelper.DropItemChance(s, player, ModContent.ItemType<Norfleet>(), 0.1f);
            DropHelper.DropItemChance(s, player, ModContent.ItemType<CosmicDischarge>(), 0.1f);

            // Vanity
            DropHelper.DropItemChance(s, player, ModContent.ItemType<DevourerofGodsMask>(), 7);

            DropHelper.DropItemCondition(s, player, ModContent.ItemType<CosmicPlushie>(), CalamityWorld.death && player.difficulty == 2);
        }
    }
}
