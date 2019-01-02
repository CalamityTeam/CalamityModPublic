using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;
using CalamityMod.Items.CalamityCustomThrowingDamage;

namespace CalamityMod.Items.Weapons 
{
	public class MarkedMagnum : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Marked Magnum");
			Tooltip.SetDefault("Shots reduce enemy protection\nProjectile damage is multiplied by all of your damage bonuses");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 3;
	        item.width = 54;
	        item.height = 20;
	        item.useTime = 15;
	        item.useAnimation = 15;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 3f;
	        item.value = 30000;
	        item.rare = 2;
	        item.UseSound = SoundID.Item33;
	        item.autoReuse = false;
	        item.shootSpeed = 12f;
	        item.shoot = mod.ProjectileType("MarkRound");
	    }
	    
	    public override Vector2? HoldoutOffset()
		{
			return new Vector2(-5, 0);
		}
	    
	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	float damageMult = player.meleeDamage + player.rangedDamage + player.magicDamage + 
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage + player.minionDamage;
	    	Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, (int)((double)damage * damageMult), knockBack, player.whoAmI, 0.0f, 0.0f);
	    	return false;
		}
	
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.HellstoneBar, 7);
	        recipe.AddIngredient(ItemID.Obsidian, 15);
	        recipe.AddIngredient(ItemID.GlowingMushroom, 15);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}