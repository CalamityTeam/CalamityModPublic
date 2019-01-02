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
    public class LivingDew : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Living Dew");
            Tooltip.SetDefault("5% increased damage reduction, +5 defense, and increased life regen while in the Jungle");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.buyPrice(0, 12, 0, 0);
            item.rare = 5;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.ZoneJungle)
            {
                player.lifeRegen += 1;
                player.statDefense += 5;
                player.endurance += 0.05f;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "ManeaterBulb", 2);
            recipe.AddIngredient(null, "TrapperBulb", 2);
            recipe.AddIngredient(null, "MurkyPaste", 5);
            recipe.AddIngredient(null, "GypsyPowder");
            recipe.AddIngredient(null, "BeetleJuice", 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}