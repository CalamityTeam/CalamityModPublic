using CalamityMod.CalPlayer;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class RaidersTalisman : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Raider's Talisman");
            Tooltip.SetDefault("Whenever you crit an enemy with a rogue weapon your rogue damage increases\n" +
                "This effect can stack up to 150 times\n" +
                "Max rogue damage boost is 15%\n" +
                "This line is modified below");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 36;
            item.value = CalamityGlobalItem.Rarity2BuyPrice;
            item.rare = ItemRarityID.Green;
            item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            int critLevel = Main.player[Main.myPlayer].Calamity().raiderStack;
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "Tooltip3")
                {
                    line2.text = "Rogue Crit Level: " + critLevel;
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.raiderTalisman = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Leather, 5);
            recipe.AddIngredient(ItemID.Obsidian, 20);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
