using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Accessories
{
    public class BadgeofBravery : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Badge of Bravery");
            Tooltip.SetDefault("The lower the health the greater the melee speed");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.value = 150000;
            item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(0, 255, 200);
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.statLife <= (player.statLifeMax2 * 0.8f) && player.statLife > (player.statLifeMax2 * 0.6f))
            {
                player.meleeSpeed += 0.05f;
            }
            else if (player.statLife <= (player.statLifeMax2 * 0.6f) && player.statLife > (player.statLifeMax2 * 0.4f))
            {
                player.meleeSpeed += 0.1f;
            }
            else if (player.statLife <= (player.statLifeMax2 * 0.4f) && player.statLife > (player.statLifeMax2 * 0.2f))
            {
                player.meleeSpeed += 0.15f;
            }
            else if (player.statLife <= (player.statLifeMax2 * 0.2f))
            {
                player.meleeSpeed += 0.2f;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "UeliaceBar", 2);
            recipe.AddIngredient(ItemID.FeralClaws);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}