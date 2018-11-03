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
    public class XerocCuisses : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Xeroc Cuisses");
            Tooltip.SetDefault("5% increased critical strike chance\n" +
                       "20% increased movement speed\n" +
                       "Speed of the cosmos");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 375000;
            item.rare = 9;
            item.defense = 24;
        }

        public override void UpdateEquip(Player player)
        {
            player.meleeCrit += 5;
            player.rangedCrit += 5;
            player.thrownCrit += 5;
            player.magicCrit += 5;
            player.moveSpeed += 0.2f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "MeldiateBar", 18);
            recipe.AddIngredient(ItemID.LunarBar, 12);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}