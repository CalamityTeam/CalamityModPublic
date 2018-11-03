using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class SilvaLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silva Leggings");
            Tooltip.SetDefault("45% increased movement speed\n" +
                "12% increased damage and 9% increased critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 7750000;
            item.defense = 39;
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
            player.moveSpeed += 0.45f;
            player.meleeDamage += 0.12f;
            player.meleeCrit += 9;
            player.rangedDamage += 0.12f;
            player.rangedCrit += 9;
            player.magicDamage += 0.12f;
            player.magicCrit += 9;
            player.thrownDamage += 0.12f;
            player.thrownCrit += 9;
            player.minionDamage += 0.12f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "DarksunFragment", 7);
            recipe.AddIngredient(null, "EffulgentFeather", 7);
            recipe.AddIngredient(null, "CosmiliteBar", 7);
            recipe.AddIngredient(null, "NightmareFuel", 15);
            recipe.AddIngredient(null, "EndothermicEnergy", 15);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}