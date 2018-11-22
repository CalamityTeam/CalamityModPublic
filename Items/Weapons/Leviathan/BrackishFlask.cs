using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.Leviathan
{
    public class BrackishFlask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brackish Flask");
            Tooltip.SetDefault("Full of poisonous saltwater");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.damage = 42;
            item.thrown = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 35;
            item.useStyle = 1;
            item.useTime = 35;
            item.knockBack = 6.5f;
            item.UseSound = SoundID.Item106;
            item.autoReuse = true;
            item.height = 30;
            item.maxStack = 1;
            item.value = 600000;
            item.rare = 7;
            item.shoot = mod.ProjectileType("BrackishFlask");
            item.shootSpeed = 12f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "IOU");
            recipe.AddIngredient(null, "LivingShard");
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
