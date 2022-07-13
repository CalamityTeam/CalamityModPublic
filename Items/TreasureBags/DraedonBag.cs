using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.NPCs.ExoMechs.Ares;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    [LegacyName("DraedonTreasureBag")]
    public class DraedonBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<AresBody>();

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 3;
            DisplayName.SetDefault("Treasure Box (Exo Mechs)");
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
            var s = player.GetSource_OpenItem(Item.type);

            player.TryGettingDevArmor(s);

            // Materials
            DropHelper.DropItem(s, player, ModContent.ItemType<ExoPrism>(), 30, 40);

            // Weapons
            if (DownedBossSystem.downedAres)
            {
                DropHelper.DropItem(s, player, ModContent.ItemType<PhotonRipper>());
                DropHelper.DropItem(s, player, ModContent.ItemType<TheJailor>());
            }
            if (DownedBossSystem.downedThanatos)
            {
                DropHelper.DropItem(s, player, ModContent.ItemType<SpineOfThanatos>());
                DropHelper.DropItem(s, player, ModContent.ItemType<RefractionRotor>());
            }
            if (DownedBossSystem.downedArtemisAndApollo)
            {
                DropHelper.DropItem(s, player, ModContent.ItemType<SurgeDriver>());
                DropHelper.DropItem(s, player, ModContent.ItemType<TheAtomSplitter>());
            }

            // Equipment
            DropHelper.DropItem(s, player, ModContent.ItemType<DraedonsHeart>());
            DropHelper.DropItem(s, player, ModContent.ItemType<ExoThrone>());

            // Vanity
            // Higher chance due to how the drops work
            float maskDropRate = 1f / 3.5f;
            if (DownedBossSystem.downedThanatos)
                DropHelper.DropItemChance(s, player, ModContent.ItemType<ThanatosMask>(), maskDropRate);

            if (DownedBossSystem.downedArtemisAndApollo)
            {
                DropHelper.DropItemChance(s, player, ModContent.ItemType<ArtemisMask>(), maskDropRate);
                DropHelper.DropItemChance(s, player, ModContent.ItemType<ApolloMask>(), maskDropRate);
            }

            if (DownedBossSystem.downedAres)
                DropHelper.DropItemChance(s, player, ModContent.ItemType<AresMask>(), maskDropRate);

            DropHelper.DropItemChance(s, player, ModContent.ItemType<DraedonMask>(), maskDropRate);
        }
    }
}
