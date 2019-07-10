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
	public class FirestormCannon : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Firestorm Cannon");
			Tooltip.SetDefault("70% chance to not consume flares\n" +
				"Right click to fire a spread of flares");
		}

	    public override void SetDefaults()
	    {
			item.damage = 11;
			item.ranged = true;
			item.width = 56;
			item.height = 28;
			item.useTime = 9;
			item.useAnimation = 9;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 1.5f;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
			item.UseSound = SoundID.Item11;
			item.autoReuse = true;
			item.shoot = 163;
			item.shootSpeed = 5.5f;
			item.useAmmo = 931;
		}
	    
	    public override bool ConsumeAmmo(Player player)
	    {
	    	if (Main.rand.Next(0, 100) < 70)
	    		return false;
	    	return true;
	    }
	    
	    public override bool AltFunctionUse(Player player)
		{
			return true;
		}
	    
	    public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				item.useTime = 27;
				item.useAnimation = 27;
			}
			else
			{
				item.useTime = 9;
				item.useAnimation = 9;
			}
			return base.CanUseItem(player);
		}
		
		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			if (player.altFunctionUse == 2)
			{
				int num6 = Main.rand.Next(4, 6);
		        for (int index = 0; index < num6; ++index)
		        {
		            float SpeedX = speedX + (float) Main.rand.Next(-50, 51) * 0.05f;
		            float SpeedY = speedY + (float) Main.rand.Next(-50, 51) * 0.05f;
		            int flare = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
		            Main.projectile[flare].penetrate = 1;
		            Main.projectile[flare].timeLeft = 600;
		        }
		        return false;
			}
			else
			{
			    int num6 = Main.rand.Next(1, 3);
			    for (int index = 0; index < num6; ++index)
			    {
			        float num7 = speedX;
			        float num8 = speedY;
			        float SpeedX = speedX + (float) Main.rand.Next(-40, 41) * 0.05f;
			        float SpeedY = speedY + (float) Main.rand.Next(-40, 41) * 0.05f;
			        int projectile = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, (int)((double)damage * 0.85), knockBack, player.whoAmI, 0.0f, 0.0f);
			        Main.projectile[projectile].timeLeft = 200;
			    }
			    return false;
			}
		}
		
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.FlareGun);
            recipe.AddIngredient(ItemID.Boomstick);
            recipe.AddIngredient(ItemID.GoldBar, 10);
            recipe.AddIngredient(null, "VictoryShard", 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.FlareGun);
            recipe.AddIngredient(ItemID.Boomstick);
            recipe.AddIngredient(ItemID.PlatinumBar, 10);
            recipe.AddIngredient(null, "VictoryShard", 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
		}
	}
}
