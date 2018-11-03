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
	public class BrimstoneFlameblaster : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brimstone Flameblaster");
		}

	    public override void SetDefaults()
	    {
			item.damage = 64;
			item.ranged = true;
			item.width = 50;
			item.height = 18;
			item.useTime = 20;
			item.useAnimation = 20;
			item.useStyle = 5;
			item.noMelee = true; //so the item's animation doesn't do damage
			item.knockBack = 3.5f;
			item.UseSound = SoundID.Item34;
			item.value = 500000;
			item.rare = 6;
			item.autoReuse = true;
			item.shoot = mod.ProjectileType("BrimstoneBallFriendly"); //idk why but all the guns in the vanilla source have this
			item.shootSpeed = 8.5f;
			item.useAmmo = 23;
		}
	}
}