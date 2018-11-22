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
    public class SilvaArmor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silva Armor");
            Tooltip.SetDefault("+80 max life and mana\n" +
                       "20% increased movement speed\n" +
                       "12% increased damage and 10% increased critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 8250000;
            item.defense = 44;
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

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 80;
            player.statManaMax2 += 80;
            player.moveSpeed += 0.2f;
            player.meleeDamage += 0.12f;
            player.meleeCrit += 10;
            player.rangedDamage += 0.12f;
            player.rangedCrit += 10;
            player.magicDamage += 0.12f;
            player.magicCrit += 10;
            player.thrownDamage += 0.12f;
            player.thrownCrit += 10;
            player.minionDamage += 0.12f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "DarksunFragment", 10);
            recipe.AddIngredient(null, "EffulgentFeather", 10);
            recipe.AddIngredient(null, "CosmiliteBar", 10);
            recipe.AddIngredient(null, "NightmareFuel", 16);
            recipe.AddIngredient(null, "EndothermicEnergy", 16);
            recipe.AddIngredient(null, "LeadCore");
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}