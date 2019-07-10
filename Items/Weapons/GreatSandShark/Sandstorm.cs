using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.GreatSandShark
{
	public class Sandstorm : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sandstorm");
			Tooltip.SetDefault("Fires sand bullets that explode");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 73;
	        item.ranged = true;
	        item.width = 62;
	        item.height = 26;
	        item.useTime = 15;
	        item.useAnimation = 15;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 5f;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
	        item.UseSound = SoundID.Item11;
	        item.autoReuse = true;
	        item.shootSpeed = 12f;
	        item.shoot = mod.ProjectileType("SandstormBullet");
	        item.useAmmo = AmmoID.Sand;
	    }
	    
	    public override Vector2? HoldoutOffset()
		{
			return new Vector2(-10, 0);
		}
	    
	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("SandstormBullet"), damage, knockBack, player.whoAmI, 0f, 0f);
	    	return false;
		}
	
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.Sandgun);
	        recipe.AddIngredient(null, "GrandScale");
	        recipe.AddIngredient(ItemID.Amber, 5);
            recipe.AddIngredient(ItemID.SandBlock, 50);
            recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}
