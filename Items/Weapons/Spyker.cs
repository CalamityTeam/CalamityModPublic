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
	public class Spyker : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spyker");
			Tooltip.SetDefault("Fires spikes that stick to enemies/tiles and explode into shrapnel that also stick to enemies/tiles and explode");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 182;
	        item.ranged = true;
	        item.width = 44;
	        item.height = 26;
	        item.useTime = 13;
	        item.useAnimation = 13;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 6f;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item108;
	        item.autoReuse = true;
	        item.shootSpeed = 9f;
	        item.shoot = mod.ProjectileType("Spyker");
	        item.useAmmo = 97;
	    }
	    
	    public override void ModifyTooltips(List<TooltipLine> list)
	    {
	        foreach (TooltipLine line2 in list)
	        {
	            if (line2.mod == "Terraria" && line2.Name == "ItemName")
	            {
	                line2.overrideColor = new Color(0, 255, 200);
	            }
	        }
	    }
	    
	    public override Vector2? HoldoutOffset()
		{
			return new Vector2(-5, 0);
		}
	    
	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("Spyker"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
	    	return false;
		}
	    
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "Needler");
	        recipe.AddIngredient(ItemID.Stynger);
	        recipe.AddIngredient(null, "UeliaceBar", 5);
	        recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}