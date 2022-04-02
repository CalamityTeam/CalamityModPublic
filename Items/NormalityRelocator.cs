using CalamityMod.CalPlayer;
using CalamityMod.Items.Placeables.Plates;
using CalamityMod.Items.Placeables.Ores;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class NormalityRelocator : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Normality Relocator");
            Tooltip.SetDefault("I'll be there in the blink of an eye\n" +
                "This line is modified below\n" +
                "Fall speed is doubled for 30 frames after teleporting\n" +
                "Teleportation is disabled while Chaos State is active\n" +
                "Works while in the inventory");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 7));
        }

        public override void SetDefaults()
        {
            item.width = 38;
            item.height = 38;
            item.value = CalamityGlobalItem.Rarity10BuyPrice;
            item.rare = ItemRarityID.Red;
            item.Calamity().donorItem = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string hotkey = CalamityMod.NormalityRelocatorHotKey.TooltipHotkeyString();
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
                {
                    line2.text = "Press " + hotkey + " to teleport to the position of the mouse";
                }
            }
        }

        public override void UpdateInventory(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.normalityRelocator = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.RodofDiscord);
            recipe.AddIngredient(ModContent.ItemType<Cinderplate>(), 5);
            recipe.AddIngredient(ModContent.ItemType<ExodiumClusterOre>(), 10);
            recipe.AddIngredient(ItemID.FragmentStardust, 30);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
