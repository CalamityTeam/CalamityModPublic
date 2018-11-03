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
		}

		public override void SetDefaults()
		{
			item.width = 18;
			item.damage = 2000;
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
			item.value = 4000000;
			item.shoot = mod.ProjectileType("ScourgeoftheCosmos");
			item.shootSpeed = 15f;
		}

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(43, 96, 222);
                }
            }
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
