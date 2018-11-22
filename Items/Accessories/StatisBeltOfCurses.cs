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
    public class StatisBeltOfCurses : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Statis' Belt of Curses");
            Tooltip.SetDefault("Increases jump speed and allows constant jumping\n" +
                "Can climb walls, dash, and dodge attacks\n" +
                "10% increased throwing damage and velocity\n" +
                "10% increased throwing crit chance\n" +
                "Increased max minions by 3 and 10% increased minion damage\n" +
                "Increased minion knockback\n" +
                "Grants shadowflame powers to all minions\n" +
                "Minions make enemies cry on hit\n" +
                "Minion attacks have a chance to instantly kill normal enemies");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 32;
            item.value = 15000000;
            item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(43, 96, 222);
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.statisBeltOfCurses = true;
            modPlayer.shadowMinions = true;
            modPlayer.tearMinions = true;
            player.minionKB += 2.5f;
            player.minionDamage += 0.1f;
            player.maxMinions += 3;
            player.autoJump = true;
            player.jumpSpeedBoost += 1.2f;
            player.extraFall += 50;
            player.blackBelt = true;
            player.dash = 1;
            player.spikedBoots = 2;
            player.thrownDamage += 0.1f;
            player.thrownCrit += 10;
            player.thrownVelocity += 0.1f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "StatisNinjaBelt");
            recipe.AddIngredient(null, "StatisCurse");
            recipe.AddIngredient(null, "Phantoplasm", 20);
            recipe.AddIngredient(null, "NightmareFuel", 20);
            recipe.AddIngredient(null, "EndothermicEnergy", 20);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}