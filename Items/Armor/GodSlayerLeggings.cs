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
    public class GodSlayerLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("God Slayer Leggings");
            Tooltip.SetDefault("35% increased movement speed\n" +
                "10% increased damage and critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 3750000;
            item.defense = 35;
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
            player.moveSpeed += 0.35f;
            player.meleeDamage += 0.1f;
            player.meleeCrit += 10;
            player.magicDamage += 0.1f;
            player.magicCrit += 10;
            player.rangedDamage += 0.1f;
            player.rangedCrit += 10;
            player.thrownDamage += 0.1f;
            player.thrownCrit += 10;
            player.minionDamage += 0.1f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CosmiliteBar", 18);
            recipe.AddIngredient(null, "NightmareFuel", 9);
            recipe.AddIngredient(null, "EndothermicEnergy", 9);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}