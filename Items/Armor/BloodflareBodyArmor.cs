using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Armor;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class BloodflareBodyArmor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodflare Body Armor");
            Tooltip.SetDefault("12% increased damage and 7% increased critical strike chance\n" +
                       "You regenerate life quickly and gain +30 defense while in lava\n" +
                       "+40 max life and mana");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 1900000;
            item.defense = 35;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(0, 255, 0);
                }
            }
        }

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 40;
            player.statManaMax2 += 40;
            player.meleeDamage += 0.12f;
            player.meleeCrit += 7;
            player.magicDamage += 0.12f;
            player.magicCrit += 7;
            player.rangedDamage += 0.12f;
            player.rangedCrit += 7;
            player.thrownDamage += 0.12f;
            player.thrownCrit += 7;
            player.minionDamage += 0.12f;
            if (player.lavaWet)
            {
                player.statDefense += 30;
                player.lifeRegen += 10;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "BloodstoneCore", 16);
            recipe.AddIngredient(null, "RuinousSoul", 4);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}