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
	public class CleansingBlaze : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cleansing Blaze");
			Tooltip.SetDefault("90% chance to not consume gel");
		}

	    public override void SetDefaults()
	    {
			item.damage = 150;
			item.ranged = true;
			item.width = 60;
			item.height = 34;
			item.useTime = 3;
			item.useAnimation = 12;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 4f;
			item.UseSound = SoundID.Item34;
			item.value = 1350000;
			item.autoReuse = true;
			item.shoot = mod.ProjectileType("EssenceFire");
			item.shootSpeed = 14f;
			item.useAmmo = 23;
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
	    
	    public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	        int num6 = Main.rand.Next(2, 4);
	        for (int index = 0; index < num6; ++index)
	        {
	            float SpeedX = speedX + (float) Main.rand.Next(-15, 16) * 0.05f;
	            float SpeedY = speedY + (float) Main.rand.Next(-15, 16) * 0.05f;
	            Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
	    	}
	    	return false;
		}
	    
	    public override Vector2? HoldoutOffset()
		{
			return new Vector2(-5, 0);
		}
	    
	    public override bool ConsumeAmmo(Player player)
	    {
	    	if (Main.rand.Next(0, 100) <= 90)
	    		return false;
	    	return true;
	    }
	    
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "CosmiliteBar", 12);
	        recipe.AddTile(null, "DraedonsForge");
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}