using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Armor;
using CalamityMod.Items.CalamityCustomThrowingDamage;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class TarragonBreastplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tarragon Breastplate");
            Tooltip.SetDefault("10% increased damage and 5% increased critical strike chance\n" +
                       "+2 life regen\n" +
                       "+40 max life and mana\n" +
                       "Breastplate of the exiler");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.lifeRegen = 2;
            item.value = 1500000;
            item.defense = 37;
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

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 40;
            player.statManaMax2 += 40;
            player.meleeDamage += 0.1f;
            player.meleeCrit += 5;
            player.magicDamage += 0.1f;
            player.magicCrit += 5;
            player.rangedDamage += 0.1f;
            player.rangedCrit += 5;
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.1f;
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 5;
            player.minionDamage += 0.1f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "UeliaceBar", 15);
            recipe.AddIngredient(null, "DivineGeode", 18);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}