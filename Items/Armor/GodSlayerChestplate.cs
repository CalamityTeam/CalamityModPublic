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
    public class GodSlayerChestplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("God Slayer Chestplate");
            Tooltip.SetDefault("+60 max life and mana\n" +
                       "15% increased movement speed\n" +
                       "Enemies take damage when they hit you\n" +
                       "Attacks have a 2% chance to do no damage to you\n" +
                       "10% increased damage and 6% increased critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 4250000;
            item.defense = 41;
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

        public override void UpdateEquip(Player player)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.godSlayerReflect = true;
            player.thorns += 0.5f;
            player.statLifeMax2 += 60;
            player.statManaMax2 += 60;
            player.moveSpeed += 0.15f;
            player.meleeDamage += 0.1f;
            player.meleeCrit += 6;
            player.magicDamage += 0.1f;
            player.magicCrit += 6;
            player.rangedDamage += 0.1f;
            player.rangedCrit += 6;
            player.thrownDamage += 0.1f;
            player.thrownCrit += 6;
            player.minionDamage += 0.1f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CosmiliteBar", 23);
            recipe.AddIngredient(null, "NightmareFuel", 11);
            recipe.AddIngredient(null, "EndothermicEnergy", 11);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}