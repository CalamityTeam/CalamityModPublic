using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.AbyssWeapons
{
	public class Valediction : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Valediction");
			Tooltip.SetDefault("Throws a homing reaper scythe");
		}

		public override void SetDefaults()
		{
			item.width = 80;
			item.damage = 405;
			item.thrown = true;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useAnimation = 15;
			item.useTime = 15;
			item.useStyle = 1;
			item.knockBack = 7f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 64;
			item.value = 3000000;
			item.shoot = mod.ProjectileType("Valediction");
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
	}
}
