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
    public class Nanotech : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nanotech");
            Tooltip.SetDefault("Throwing projectiles leave behind nanoblades as they travel\n" +
                "While holding a throwing weapon your defense is boosted by 15 and your damage reduction is boosted by 10%\n" +
                "Throwing weapons have a chance to instantly kill normal enemies\n" +
                "10% increased throwing damage, crit chance, and velocity");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 32;
            item.value = 10000000;
            item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.nanotech = true;
            player.thrownDamage += 0.1f;
            player.thrownCrit += 10;
            player.thrownVelocity += 0.1f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.MartianConduitPlating, 250);
            recipe.AddIngredient(ItemID.Nanites, 500);
            recipe.AddIngredient(null, "Phantoplasm", 20);
            recipe.AddIngredient(null, "NightmareFuel", 20);
            recipe.AddIngredient(null, "EndothermicEnergy", 20);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}