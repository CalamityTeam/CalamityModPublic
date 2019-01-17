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
	public class SubsumingVortex : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Subsuming Vortex");
			Tooltip.SetDefault("Fires 3 vortexes of elemental energy");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 520;
	        item.magic = true;
	        item.mana = 20;
	        item.width = 28;
	        item.height = 30;
	        item.UseSound = SoundID.Item84;
	        item.useTime = 20;
	        item.useAnimation = 20;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 5f;
            item.value = Item.buyPrice(2, 50, 0, 0);
            item.rare = 10;
            item.autoReuse = true;
	        item.shoot = mod.ProjectileType("Vortex");
	        item.shootSpeed = 9f;
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
	    
	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{    
		    int num6 = 3;
		    for (int index = 0; index < num6; ++index)
		    {
		        float SpeedX = speedX + (float) Main.rand.Next(-50, 51) * 0.05f;
		        float SpeedY = speedY + (float) Main.rand.Next(-50, 51) * 0.05f;
		        float ai = (Main.rand.NextFloat() + 0.5f);
		        Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0.0f, ai);
		    }
		    return false;
		}
	    
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "AuguroftheElements");
	        recipe.AddIngredient(null, "NuclearFury");
	        recipe.AddIngredient(null, "RelicofRuin");
	        recipe.AddIngredient(null, "TearsofHeaven");
	        recipe.AddIngredient(null, "NightmareFuel", 5);
        	recipe.AddIngredient(null, "EndothermicEnergy", 5);
	        recipe.AddIngredient(null, "CosmiliteBar", 5);
            recipe.AddIngredient(null, "DarksunFragment", 5);
            recipe.AddIngredient(null, "HellcasterFragment", 3);
            recipe.AddIngredient(null, "Phantoplasm", 5);
            recipe.AddIngredient(null, "AuricOre", 25);
            recipe.AddTile(null, "DraedonsForge");
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}