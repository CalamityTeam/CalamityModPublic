using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons 
{
	public class MirrorBlade : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mirror Blade");
			Tooltip.SetDefault("The amount of contact damage an enemy does is added to this weapons' damage\n" +
                "You must hit an enemy with the blade to trigger this effect\n" +
                "Consumes mana to fire mirror blasts");
		}

		public override void SetDefaults()
		{
			item.width = 50;
			item.damage = 236;
			item.melee = true;
			item.mana = 4;
			item.useAnimation = 9;
			item.useTime = 9;
			item.useTurn = true;
			item.useStyle = 1;
			item.knockBack = 8f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 60;
			item.value = 2000000;
			item.shootSpeed = 16f;
	        item.shoot = mod.ProjectileType("MirrorBlast");
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
	    	Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
	    	return false;
		}
	    
	    public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
	    {
	    	int conDamage = target.damage + 236;
	    	if (conDamage < 236)
	    	{
	    		conDamage = 236;
	    	}
            if (conDamage > 500)
            {
                conDamage = 500;
            }
            item.damage = conDamage;
		}
	}
}
