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
	public class Impaler : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Impaler");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 120;
	        item.ranged = true;
	        item.crit += 14;
	        item.width = 40;
	        item.height = 26;
	        item.useTime = 20;
	        item.useAnimation = 20;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 7f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
	        item.UseSound = SoundID.Item5;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("FlamingStake");
	        item.shootSpeed = 10f;
	        item.useAmmo = 1836;
	    }
	    
	    public override Vector2? HoldoutOffset()
		{
			return new Vector2(0, -10);
		}
	    
	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	        float SpeedX = speedX + (float) Main.rand.Next(-5, 6) * 0.05f;
	        float SpeedY = speedY + (float) Main.rand.Next(-5, 6) * 0.05f;
	        if (Main.rand.Next(3) == 0)
	        {
	        	Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, mod.ProjectileType("ExplodingStake"), (int)((double)damage), knockBack, player.whoAmI, 0.0f, 0.0f);
	        }
	        else
	        {
	        	Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, mod.ProjectileType("FlamingStake"), (int)((double)damage), knockBack, player.whoAmI, 0.0f, 0.0f);
	        }
	    	return false;
		}
	
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "CoreofChaos", 5);
	        recipe.AddIngredient(ItemID.StakeLauncher);
	        recipe.AddIngredient(ItemID.ExplosivePowder, 100);
	        recipe.AddIngredient(ItemID.LivingFireBlock, 75);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}