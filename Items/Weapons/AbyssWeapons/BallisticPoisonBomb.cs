using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.AbyssWeapons 
{
	public class BallisticPoisonBomb : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ballistic Poison Bomb");
		}

		public override void SetDefaults()
		{
			item.width = 30;
			item.damage = 60;
			item.thrown = true;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useAnimation = 22;
			item.useStyle = 1;
			item.useTime = 22;
			item.knockBack = 6.5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 38;
			item.value = 500000;
			item.rare = 7;
			item.shoot = mod.ProjectileType("BallisticPoisonBomb");
			item.shootSpeed = 12f;
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "DepthCells", 10);
            recipe.AddIngredient(null, "SulphurousSand", 20);
            recipe.AddIngredient(null, "Tenebris", 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
