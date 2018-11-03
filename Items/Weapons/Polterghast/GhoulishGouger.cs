using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Polterghast
{
	public class GhoulishGouger : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ghoulish Gouger");
			Tooltip.SetDefault("Throws a ghoulish scythe");
		}

		public override void SetDefaults()
		{
			item.width = 68;
			item.damage = 150;
			item.thrown = true;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useAnimation = 12;
			item.useTime = 12;
			item.useStyle = 1;
			item.knockBack = 7.5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 60;
			item.value = 1000000;
			item.shoot = mod.ProjectileType("GhoulishGouger");
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
