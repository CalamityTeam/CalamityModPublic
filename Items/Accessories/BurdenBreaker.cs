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
    public class BurdenBreaker : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Determination Breaker");
            Tooltip.SetDefault("The bad time\n" +
                "Removes immunity frames\n" +
                "If you want a crazy challenge, equip this");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.value = 150000;
            item.rare = 3;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.immune)
            {
                player.immune = false;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Bone, 50);
            recipe.AddIngredient(ItemID.IronBar, 7);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Bone, 50);
            recipe.AddIngredient(ItemID.LeadBar, 7);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}