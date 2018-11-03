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
    public class YharimsInsignia : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yharim's Insignia");
            Tooltip.SetDefault("10% increased damage when under 50% life\n" +
                "Increased melee speed as health lowers\n" +
                "7% increased melee speed and damage\n" +
                "Melee attacks inflict holy fire\n" +
                "Increased invincibility after taking damage\n" +
                "You are immune to lava\n" +
                "Increased melee knockback");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 38;
            item.value = 5000000;
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
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.yInsignia = true;
            player.longInvince = true;
            player.kbGlove = true;
            player.meleeDamage += 0.07f;
            player.meleeSpeed += 0.07f;
            player.lavaImmune = true;
            if (player.statLife <= (player.statLifeMax2 * 0.5f))
            {
                player.meleeDamage += 0.1f;
                player.magicDamage += 0.1f;
                player.rangedDamage += 0.1f;
                player.thrownDamage += 0.1f;
                player.minionDamage += 0.1f;
            }
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
            recipe.AddIngredient(ItemID.WarriorEmblem);
            recipe.AddIngredient(null, "NecklaceofVexation");
            recipe.AddIngredient(null, "CoreofCinder", 5);
            recipe.AddIngredient(ItemID.CrossNecklace);
            recipe.AddIngredient(null, "BadgeofBravery");
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}