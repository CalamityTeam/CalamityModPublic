using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.BrimstoneWaifu {
public class Brimlance : ModItem
{
	public override void SetStaticDefaults()
	{
		DisplayName.SetDefault("Brimlance");
		Tooltip.SetDefault("This spear causes brimstone explosions on enemy hits\nEnemies killed by the spear drop brimstone fire");
	}

	public override void SetDefaults()
	{
		item.width = 56;  //The width of the .png file in pixels divided by 2.
		item.damage = 75;  //Keep this reasonable please.
		item.melee = true;  //Dictates whether this is a melee-class weapon.
		item.noMelee = true;
		item.useTurn = true;
		item.noUseGraphic = true;
		item.useAnimation = 19;
		item.useStyle = 5;
		item.useTime = 19;
		item.knockBack = 7.5f;  //Ranges from 1 to 9.
		item.UseSound = SoundID.Item1;
		item.autoReuse = false;  //Dictates whether the weapon can be "auto-fired".
		item.height = 56;  //The height of the .png file in pixels divided by 2.
		item.maxStack = 1;
		item.value = 200000;  //Value is calculated in copper coins.
		item.rare = 5;  //Ranges from 1 to 11.
		item.shoot = mod.ProjectileType("Brimlance");
		item.shootSpeed = 12f;
	}
}}
