using CalamityMod.Items.Pets;
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
            item.rare = ItemRarityID.Blue;
        }

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            // Weapons
            DropHelper.DropItem(player, WorldGen.CopperTierOre == TileID.Copper ? ItemID.CopperBroadsword : ItemID.TinBroadsword);
            DropHelper.DropItem(player, WorldGen.CopperTierOre == TileID.Copper ? ItemID.CopperBow : ItemID.TinBow);
            DropHelper.DropItem(player, ItemID.WoodenArrow, 100);
            DropHelper.DropItem(player, WorldGen.CopperTierOre == TileID.Copper ? ItemID.AmethystStaff : ItemID.TopazStaff);
            DropHelper.DropItem(player, ItemID.ManaCrystal);
            DropHelper.DropItem(player, ModContent.ItemType<SquirrelSquireStaff>());
            DropHelper.DropItem(player, ModContent.ItemType<ThrowingBrick>(), 150);

            // Tools / Utility
            DropHelper.DropItem(player, WorldGen.CopperTierOre == TileID.Copper ? ItemID.CopperHammer : ItemID.TinHammer);
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

            // Difficulty items (Revengeance, Death and Malice don't drop in Normal)
            DropHelper.DropItemCondition(player, ModContent.ItemType<Revenge>(), Main.expertMode);
            DropHelper.DropItemCondition(player, ModContent.ItemType<Death>(), Main.expertMode);
            DropHelper.DropItemCondition(player, ModContent.ItemType<Malice>(), Main.expertMode);

            // The Lad
            DropHelper.DropItemCondition(player, ModContent.ItemType<JoyfulHeart>(), player.name == "Aleksh" || player.name == "Shark Lad");

            // Music box (if music mod installed)
            Mod musicMod = CalamityMod.Instance.musicMod;
            if (musicMod != null)
                DropHelper.DropItem(player, musicMod.ItemType("CalamityMusicbox"));
        }
    }
}
