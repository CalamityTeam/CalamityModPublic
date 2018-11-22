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
    public class DarkSunRing : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Sun Ring");
            Tooltip.SetDefault("Increase to damage, melee speed, critical strike chance, life regeneration,\n" +
                "defense, pick speed, max minions, and minion knockback\n" +
                "During the day the player has +2 life regen\n" +
                "During the night the player has +20 defense\n" +
                "Contains the power of the dark sun");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 6));
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = 5000000;
            item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(108, 45, 199);
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.darkSunRing = true;
            player.maxMinions += 1;
            player.lifeRegen += 1;
            player.statDefense += 5;
            player.meleeSpeed += 0.05f;
            player.meleeDamage += 0.05f;
            player.meleeCrit += 5;
            player.rangedDamage += 0.05f;
            player.rangedCrit += 5;
            player.magicDamage += 0.05f;
            player.magicCrit += 5;
            player.pickSpeed -= 0.3f;
            player.minionDamage += 0.05f;
            player.minionKB += 1f;
            player.thrownDamage += 0.05f;
            player.thrownCrit += 5;
            if (Main.dayTime)
            {
                player.lifeRegen += 2;
            }
            else
            {
                player.statDefense += 20;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "UeliaceBar", 10);
            recipe.AddIngredient(null, "DarksunFragment", 100);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}