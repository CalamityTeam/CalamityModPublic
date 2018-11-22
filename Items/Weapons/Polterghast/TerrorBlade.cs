using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Polterghast
{
	public class TerrorBlade : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Terror Blade");
			Tooltip.SetDefault("Fires a terror beam that bounces off tiles\nOn every bounce it emits an explosion");
		}

		public override void SetDefaults()
		{
			item.width = 88;
			item.damage = 330;
			item.melee = true;
			item.useAnimation = 18;
			item.useTime = 18;
			item.useTurn = true;
			item.useStyle = 1;
			item.knockBack = 8.5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 80;
			item.value = 1000000;
			item.shoot = mod.ProjectileType("TerrorBeam");
			item.shootSpeed = 20f;
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
	
	    public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.Next(3) == 0)
	        {
	        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 60);
	        }
	    }
	}
}
