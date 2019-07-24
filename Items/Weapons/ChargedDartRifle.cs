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
	public class ChargedDartRifle : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Charged Dart Blaster");
			Tooltip.SetDefault("Right click to fire an exploding energy blast that bounces");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 126;
	        item.ranged = true;
	        item.width = 60;
	        item.height = 24;
	        item.useTime = 25;
	        item.useAnimation = 25;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 7f;
            item.value = Item.buyPrice(0, 95, 0, 0);
            item.rare = 9;
	        item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon");
	        item.autoReuse = true;
	        item.shootSpeed = 22f;
	        item.shoot = mod.ProjectileType("ChargedBlast");
	        item.useAmmo = 283;
	    }
	    
	    public override Vector2? HoldoutOffset()
		{
			return new Vector2(-5, 0);
		}
	    
	    public override bool AltFunctionUse(Player player)
		{
			return true;
		}
	    
	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	if (player.altFunctionUse == 2)
	    	{
	    		Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("ChargedBlast3"), (int)((double)damage * 0.85), knockBack, player.whoAmI, 0f, 0f);
	    		return false;
	    	}
	    	else
	    	{
	    		Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("ChargedBlast"), damage, knockBack, player.whoAmI, 0f, 0f);
	    		return false;
	    	}
		}
	
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.DartRifle);
	        recipe.AddIngredient(ItemID.MartianConduitPlating, 25);
	        recipe.AddIngredient(null, "CoreofEleum", 3);
            recipe.AddIngredient(ItemID.FragmentVortex, 5);
            recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	        recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.DartPistol);
	        recipe.AddIngredient(ItemID.MartianConduitPlating, 25);
	        recipe.AddIngredient(null, "CoreofEleum", 3);
            recipe.AddIngredient(ItemID.FragmentVortex, 5);
            recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}