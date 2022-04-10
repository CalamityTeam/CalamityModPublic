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
            Item.consumable = true;
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.Blue;
        }

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            // IEntitySource my beloathed
            var s = player.GetItemSource_OpenItem(Item.type);

            // Weapons
            DropHelper.DropItem(s, player, WorldGen.SavedOreTiers.Copper == TileID.Copper ? ItemID.CopperBroadsword : ItemID.TinBroadsword);
            DropHelper.DropItem(s, player, WorldGen.SavedOreTiers.Copper == TileID.Copper ? ItemID.CopperBow : ItemID.TinBow);
            DropHelper.DropItem(s, player, ItemID.WoodenArrow, 100);
            DropHelper.DropItem(s, player, WorldGen.SavedOreTiers.Copper == TileID.Copper ? ItemID.AmethystStaff : ItemID.TopazStaff);
            DropHelper.DropItem(s, player, ItemID.ManaCrystal);
            DropHelper.DropItem(s, player, ModContent.ItemType<SquirrelSquireStaff>());
            DropHelper.DropItem(s, player, ModContent.ItemType<ThrowingBrick>(), 150);

            // Tools / Utility
            DropHelper.DropItem(s, player, WorldGen.SavedOreTiers.Copper == TileID.Copper ? ItemID.CopperHammer : ItemID.TinHammer);
            DropHelper.DropItem(s, player, ItemID.Bomb, 10);
            DropHelper.DropItem(s, player, ItemID.MiningPotion);
            DropHelper.DropItem(s, player, ItemID.SpelunkerPotion, 2);
            DropHelper.DropItem(s, player, ItemID.SwiftnessPotion, 3);
            DropHelper.DropItem(s, player, ItemID.GillsPotion, 2);
            DropHelper.DropItem(s, player, ItemID.ShinePotion);
            DropHelper.DropItem(s, player, ItemID.RecallPotion, 3);

            // Tiles / Placeables
            DropHelper.DropItem(s, player, ItemID.Torch, 25);
            DropHelper.DropItem(s, player, ItemID.Chest, 3);

            // Difficulty items (Revengeance, Death and Malice don't drop in Normal)
            DropHelper.DropItemCondition(s, player, ModContent.ItemType<Revenge>(), Main.expertMode);
            DropHelper.DropItemCondition(s, player, ModContent.ItemType<Death>(), Main.expertMode);
            DropHelper.DropItemCondition(s, player, ModContent.ItemType<Malice>(), Main.expertMode);

            // The Lad
            DropHelper.DropItemCondition(s, player, ModContent.ItemType<JoyfulHeart>(), player.name == "Aleksh" || player.name == "Shark Lad");

            // Music box (if music mod installed)
            Mod musicMod = CalamityMod.Instance.musicMod;
            if (musicMod != null)
                DropHelper.DropItem(s, player, musicMod.Find<ModItem>("CalamityMusicbox").Type);
        }
    }
}
