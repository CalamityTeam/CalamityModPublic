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
	public class MagnaCannon : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Magna Cannon");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 19;
	        item.magic = true;
	        item.mana = 12;
	        item.width = 56;
	        item.height = 34;
	        item.useTime = 32;
	        item.useAnimation = 32;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 3.5f;
	        item.value = 90000;
	        item.rare = 3;
	        item.UseSound = SoundID.Item117;
	        item.autoReuse = true;
	        item.shootSpeed = 12f;
	        item.shoot = mod.ProjectileType("MagnaBlast");
	    }
	    
	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
	    {
	        int num6 = 3;
	        for (int index = 0; index < num6; ++index)
	        {
	            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, (int)((double)damage), knockBack, player.whoAmI, 0.0f, 0.0f);
	        }
	        return false;
		}
	
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.Granite, 25);
	        recipe.AddIngredient(ItemID.Obsidian, 15);
	        recipe.AddIngredient(ItemID.Amber, 5);
	        recipe.AddIngredient(ItemID.SpaceGun);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}