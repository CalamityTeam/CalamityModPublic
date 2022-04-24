using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.NPCs.StormWeaver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.TreasureBags
{
    public class StormWeaverBag : ModItem
    {
        public override int BossBagNPC => ModContent.NPCType<StormWeaverHead>();

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
            DisplayName.SetDefault("Treasure Bag (Storm Weaver)");
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
            DropHelper.DropItem(s, player, ModContent.ItemType<ArmoredShell>(), 7, 10);

            // Weapons
            DropHelper.DropItemChance(s, player, ModContent.ItemType<TheStorm>(), DropHelper.BagWeaponDropRateInt);
            DropHelper.DropItemChance(s, player, ModContent.ItemType<StormDragoon>(), DropHelper.BagWeaponDropRateInt);
            DropHelper.DropItemChance(s, player, ModContent.ItemType<Thunderstorm>(), 0.1f);

            // Equipment
            // Stay tuned for Definitely Not Charged Perforator Runald's Band As A Single Item

            // Vanity
            DropHelper.DropItemChance(s, player, ModContent.ItemType<StormWeaverMask>(), 7);
            if (Main.rand.NextBool(20))
            {
                DropHelper.DropItem(s, player, ModContent.ItemType<AncientGodSlayerHelm>());
                DropHelper.DropItem(s, player, ModContent.ItemType<AncientGodSlayerChestplate>());
                DropHelper.DropItem(s, player, ModContent.ItemType<AncientGodSlayerLeggings>());
            }

            // Light Pet
            DropHelper.DropItemChance(s, player, ModContent.ItemType<LittleLight>(), 8);
        }
    }
}
