using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons {
public class TeardropCleaver : ModItem
{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Teardrop Cleaver");
			Tooltip.SetDefault("Makes your enemies cry");
		}

	public override void SetDefaults()
	{
		item.width = 52;  //The width of the .png file in pixels divided by 2.
		item.damage = 15;  //Keep this reasonable please.
		item.melee = true;  //Dictates whether this is a melee-class weapon.
		item.useAnimation = 24;
		item.useStyle = 1;
		item.useTime = 24;
		item.useTurn = true;
		item.knockBack = 4.5f;  //Ranges from 1 to 9.
		item.UseSound = SoundID.Item1;
		item.autoReuse = true;
		item.height = 56;  //The height of the .png file in pixels divided by 2.
		item.maxStack = 1;
		item.value = 50000;  //Value is calculated in copper coins.
		item.rare = 2;  //Ranges from 1 to 11.
	}

    public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
    {
    	target.AddBuff(mod.BuffType("TemporalSadness"), 60);
	}
}}
