using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.Crabulon
{
	public class Fungicide : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fungicide");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 11;
	        item.ranged = true;
	        item.width = 40;
	        item.height = 26;
	        item.useTime = 20;
	        item.useAnimation = 20;
	        item.useStyle = 5;
	        item.noMelee = true; //so the item's animation doesn't do damage
	        item.knockBack = 2.5f;
	        item.value = 40000;
	        item.rare = 2;
	        item.UseSound = SoundID.Item61;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("FungiOrb"); //idk why but all the guns in the vanilla source have this
	        item.shootSpeed = 14f;
	        item.useAmmo = 97;
	    }
	    
	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("FungiOrb"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
	    	return false;
		}
	}
}