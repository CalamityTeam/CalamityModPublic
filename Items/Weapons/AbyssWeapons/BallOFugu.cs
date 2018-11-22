using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.AbyssWeapons
{
	public class BallOFugu : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ball O' Fugu");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 40;
	        item.melee = true;
	        item.width = 30;
	        item.height = 10;
	        item.useTime = 20;
	        item.useAnimation = 20;
	        item.useStyle = 5;
	        item.noMelee = true;
            item.noUseGraphic = true;
	        item.knockBack = 8f;
	        item.value = 50000;
	        item.rare = 3;
	        item.UseSound = SoundID.Item1;
	        item.autoReuse = true;
            item.channel = true;
	        item.shoot = mod.ProjectileType("BallOFugu");
	        item.shootSpeed = 12f;
	    }
	}
}