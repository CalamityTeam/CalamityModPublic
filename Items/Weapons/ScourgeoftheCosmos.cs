using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons 
{
	public class ScourgeoftheCosmos : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Scourge of the Cosmos");
			Tooltip.SetDefault("Throws a bouncing cosmic scourge that emits tiny homing cosmic scourges on death and tile hits");
		}

		public override void SetDefaults()
		{
			item.width = 18;
			item.damage = 1500;
			item.melee = true;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useAnimation = 20;
			item.useStyle = 5;
			item.useTime = 20;
			item.knockBack = 5f;
			item.UseSound = SoundID.Item109;
			item.autoReuse = true;
			item.height = 20;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = 10;
            item.shoot = mod.ProjectileType("ScourgeoftheCosmos");
			item.shootSpeed = 15f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 14;
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.ScourgeoftheCorruptor);
            recipe.AddIngredient(null, "CosmiliteBar", 10);
            recipe.AddIngredient(null, "DarksunFragment", 10);
            recipe.AddIngredient(null, "XerocPitchfork", 200);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
