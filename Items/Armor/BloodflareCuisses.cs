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
    [AutoloadEquip(EquipType.Legs)]
    public class BloodflareCuisses : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodflare Cuisses");
            Tooltip.SetDefault("30% increased movement speed, 10% increased damage and critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 1762500;
            item.defense = 29;
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
            player.moveSpeed += 0.3f;
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
            recipe.AddIngredient(null, "BloodstoneCore", 13);
            recipe.AddIngredient(null, "RuinousSoul", 3);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}