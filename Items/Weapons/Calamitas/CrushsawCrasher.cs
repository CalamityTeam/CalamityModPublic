using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.Calamitas {
public class CrushsawCrasher : ModItem
{
	public override void SetStaticDefaults()
	{
		DisplayName.SetDefault("Crushsaw Crasher");
	}

	public override void SetDefaults()
	{
		item.width = 38;  //The width of the .png file in pixels divided by 2.
		item.damage = 65;  //Keep this reasonable please.
		item.thrown = true;  //Dictates whether this is a melee-class weapon.
		item.useAnimation = 18;
		item.useStyle = 1;
		item.useTime = 18;
		item.knockBack = 7f;  //Ranges from 1 to 9.
		item.UseSound = SoundID.Item1;
		item.autoReuse = true;  //Dictates whether the weapon can be "auto-fired".
		item.height = 22;  //The height of the .png file in pixels divided by 2.
		item.maxStack = 1;
		item.value = 500000;  //Value is calculated in copper coins.
		item.rare = 6;  //Ranges from 1 to 11.
		item.shoot = mod.ProjectileType("Crushax");
		item.shootSpeed = 11f;
	}
	
	public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
    {
		target.AddBuff(mod.BuffType("BrimstoneFlames"), 300);
	}
}}
