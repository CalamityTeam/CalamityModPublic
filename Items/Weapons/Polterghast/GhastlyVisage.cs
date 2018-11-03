using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.Polterghast
{
	public class GhastlyVisage : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ghastly Visage");
			Tooltip.SetDefault("Fires homing ghast energy that explodes");
		}


	    public override void SetDefaults()
	    {
	        item.damage = 110;
	        item.magic = true;
	        item.noUseGraphic = true;
			item.channel = true;
	        item.mana = 20;
	        item.width = 78;
	        item.height = 70;
	        item.useTime = 30;
	        item.useAnimation = 30;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 5f;
	        item.value = 1000000;
	        item.shootSpeed = 9f;
	        item.shoot = mod.ProjectileType("GhastlyVisage");
	    }
	    
	    public override void ModifyTooltips(List<TooltipLine> list)
	    {
	        foreach (TooltipLine line2 in list)
	        {
	            if (line2.mod == "Terraria" && line2.Name == "ItemName")
	            {
	                line2.overrideColor = new Color(0, 255, 0);
	            }
	        }
	    }
	    
	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
	    {
	    	Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("GhastlyVisage"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
	    	return false;
		}
	}
}