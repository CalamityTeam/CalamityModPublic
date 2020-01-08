
using CalamityMod.Items.DifficultyItems;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class StarterBag : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Starter Bag");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
        }

        public override void SetDefaults()
        {
            item.consumable = true;
            item.width = 24;
            item.height = 24;
            item.rare = 1;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            // Weapons
            DropHelper.DropItem(player, ItemID.CopperBroadsword);
            DropHelper.DropItem(player, ItemID.CopperBow);
            DropHelper.DropItem(player, ItemID.WoodenArrow, 100);
            DropHelper.DropItem(player, ItemID.AmethystStaff);
            DropHelper.DropItem(player, ItemID.ManaCrystal);
            DropHelper.DropItem(player, ModContent.ItemType<WulfrumController>());
            DropHelper.DropItem(player, ModContent.ItemType<WulfrumKnife>(), 150);

            // Tools / Utility
            DropHelper.DropItem(player, ItemID.CopperHammer);
            DropHelper.DropItem(player, ItemID.Bomb, 10);
            DropHelper.DropItem(player, ItemID.MiningPotion);
            DropHelper.DropItem(player, ItemID.SpelunkerPotion, 2);
            DropHelper.DropItem(player, ItemID.SwiftnessPotion, 3);
            DropHelper.DropItem(player, ItemID.GillsPotion, 2);
            DropHelper.DropItem(player, ItemID.ShinePotion);
            DropHelper.DropItem(player, ItemID.RecallPotion, 3);

            // Tiles / Placeables
            DropHelper.DropItem(player, ItemID.Torch, 25);
            DropHelper.DropItem(player, ItemID.Chest, 3);

            // Difficulty items
            DropHelper.DropItem(player, ModContent.ItemType<Death>());
            DropHelper.DropItem(player, ModContent.ItemType<DefiledRune>());

            // Speedrun King Slime
            DropHelper.DropItem(player, ItemID.SlimeCrown);

            // Music box (if music mod installed)
            Mod musicMod = ModLoader.GetMod("CalamityModMusic");
            if (musicMod != null)
                DropHelper.DropItem(player, musicMod.ItemType("CalamityMusicbox"));
        }
    }
}
