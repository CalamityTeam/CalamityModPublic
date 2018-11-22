using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.AbyssItems
{
    public class ReaperToothNecklace : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reaper Tooth Necklace");
            Tooltip.SetDefault("Increases armor penetration by 100\n" +
                "Increases all damage and critical strike chance by 25%\n" +
                "Cuts your defense and damage reduction in half");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = 500000;
            item.rare = 10;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.reaperToothNecklace = true;
            player.armorPenetration += 100;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "ReaperTooth", 6);
            recipe.AddIngredient(null, "Lumenite", 15);
            recipe.AddIngredient(null, "DepthCells", 15);
            recipe.AddIngredient(null, "Tenebris", 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}