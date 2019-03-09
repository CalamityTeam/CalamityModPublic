using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.Polterghast
{
	public class GhastlyVisage : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ghastly Visage");
			Tooltip.SetDefault("Fires homing ghast energy that explodes");
		}


	    public override void SetDefaults()
	    {
	        item.damage = 92;
	        item.magic = true;
	        item.noUseGraphic = true;
			item.channel = true;
	        item.mana = 20;
	        item.width = 78;
	        item.height = 70;
	        item.useTime = 27;
	        item.useAnimation = 27;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 5f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.shootSpeed = 9f;
	        item.shoot = mod.ProjectileType("GhastlyVisage");
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 13;
		}
	    
	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
	    {
	    	Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("GhastlyVisage"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
	    	return false;
		}
	}
}