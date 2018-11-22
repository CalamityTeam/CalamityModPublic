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
	public class PlasmaRod : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Plasma Rod");
			Tooltip.SetDefault("Casts a low-damage plasma bolt\n" +
                "Shooting a tile will cause several bolts with increased damage to fire\n" +
                "Shooting an enemy will cause several debuffs for a short time");
			Item.staff[item.type] = true;
		}

	    public override void SetDefaults()
	    {
	        item.damage = 8;
	        item.magic = true;
	        item.mana = 10;
	        item.width = 40;
	        item.height = 40;
	        item.useTime = 36;
	        item.useAnimation = 36;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 2.5f;
	        item.value = 60000;
	        item.rare = 3;
	        item.UseSound = SoundID.Item109;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("PlasmaRay");
	        item.shootSpeed = 6f;
	    }
	}
}