using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.Calamitas
{
	public class BrimstoneFlamesprayer : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Havoc's Breath");
		}

	    public override void SetDefaults()
	    {
			item.damage = 59;
			item.ranged = true;
			item.width = 50;
			item.height = 18;
			item.useTime = 9;
			item.useAnimation = 30;
			item.useStyle = 5;
			item.noMelee = true; //so the item's animation doesn't do damage
			item.knockBack = 1.5f;
			item.UseSound = SoundID.Item34;
			item.value = 500000;
			item.rare = 6;
			item.autoReuse = true;
			item.shoot = mod.ProjectileType("BrimstoneFireFriendly"); //idk why but all the guns in the vanilla source have this
			item.shootSpeed = 8.5f;
			item.useAmmo = 23;
		}
	}
}