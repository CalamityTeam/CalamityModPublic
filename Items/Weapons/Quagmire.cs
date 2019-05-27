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
	public class Quagmire : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Quagmire");
			Tooltip.SetDefault("Fires spore clouds");
		}

	    public override void SetDefaults()
	    {
	    	item.CloneDefaults(ItemID.HelFire);
	        item.damage = 52;
	        item.useTime = 22;
	        item.useAnimation = 22;
	        item.useStyle = 5;
	        item.channel = true;
	        item.melee = true;
	        item.knockBack = 3.5f;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("QuagmireProjectile");
	    }
	    
	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
	    {
	        Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, (int)((double)damage), knockBack, player.whoAmI, 0.0f, 0.0f);
			return false;
		}
	    
	    public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "DraedonBar", 6);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}