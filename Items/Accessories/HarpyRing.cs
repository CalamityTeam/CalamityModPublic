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
    public class HarpyRing : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Harpy Ring");
            Tooltip.SetDefault("Increased movement speed\n" +
                "Boosts your maximum flight time by 25%");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 22;
            item.lifeRegen = 2;
            item.value = 50000;
            item.rare = 4;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.harpyRing = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "AerialiteBar", 2);
            recipe.AddIngredient(ItemID.Feather, 5);
            recipe.AddIngredient(ItemID.FallenStar);
            recipe.AddTile(TileID.SkyMill);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}