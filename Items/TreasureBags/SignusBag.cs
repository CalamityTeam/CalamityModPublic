using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.Signus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class SignusBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<Signus>();

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 3;
            DisplayName.SetDefault("Treasure Bag (Signus, Envoy of the Devourer)");
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
            DropHelper.DropItem(s, player, ModContent.ItemType<TwistingNether>(), 3, 4);

            // Weapons
            DropHelper.DropItemChance(s, player, ModContent.ItemType<CosmicKunai>(), DropHelper.BagWeaponDropRateInt);
            DropHelper.DropItemChance(s, player, ModContent.ItemType<Cosmilamp>(), DropHelper.BagWeaponDropRateInt);

            // Equipment
            DropHelper.DropItem(s, player, ModContent.ItemType<SpectralVeil>());

            // Vanity
            DropHelper.DropItemChance(s, player, ModContent.ItemType<SignusMask>(), 7);
            if (Main.rand.NextBool(20))
            {
                DropHelper.DropItem(s, player, ModContent.ItemType<AncientGodSlayerHelm>());
                DropHelper.DropItem(s, player, ModContent.ItemType<AncientGodSlayerChestplate>());
                DropHelper.DropItem(s, player, ModContent.ItemType<AncientGodSlayerLeggings>());
            }
        }
    }
}
