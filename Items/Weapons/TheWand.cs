using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons 
{
	public class TheWand : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Wand");
			Tooltip.SetDefault("The ultimate wand");
		}

		public override void SetDefaults()
		{
			item.width = 40;
			item.damage = 1;
			item.mana = 500;
			item.magic = true;
			item.noMelee = true;
			item.useAnimation = 20;
			item.useTime = 20;
			item.useTurn = true;
			item.useStyle = 1;
			item.knockBack = 0f;
			item.UseSound = SoundID.Item102;
			item.autoReuse = true;
			item.height = 36;
            item.value = Item.buyPrice(2, 50, 0, 0);
            item.rare = 10;
            item.shoot = mod.ProjectileType("SparkInfernal");
			item.shootSpeed = 24f;
		}
		
		public override void ModifyTooltips(List<TooltipLine> list)
		{
	        foreach (TooltipLine line2 in list)
	        {
	            if (line2.mod == "Terraria" && line2.Name == "ItemName")
	            {
	                line2.overrideColor = new Color(108, 45, 199);
	            }
	        }
	    }
		
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.WandofSparking);
			recipe.AddIngredient(null, "HellcasterFragment", 5);
	        recipe.AddTile(null, "DraedonsForge");
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	
	    public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.Next(3) == 0)
	        {
	        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 6);
	        }
	    }
	}
}
