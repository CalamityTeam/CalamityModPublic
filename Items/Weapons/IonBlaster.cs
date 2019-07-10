using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons 
{
	public class IonBlaster : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ion Blaster");
			Tooltip.SetDefault("Fires ion blasts that speed up and then explode\n" +
				"The higher your mana the more damage they will do");
		}

		public override void SetDefaults()
		{
			item.width = 44;
			item.damage = 20;
			item.magic = true;
			item.mana = 8;
			item.useAnimation = 10;
			item.useTime = 10;
			item.useStyle = 5;
			item.knockBack = 5.5f;
			item.UseSound = SoundID.Item91;
			item.autoReuse = true;
			item.noMelee = true;
			item.height = 28;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
			item.shoot = mod.ProjectileType("IonBlast");
			item.shootSpeed = 3f;
		}
		
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-5, 0);
		}
		
		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	        float manaAmount = ((float)player.statMana * 0.01f);
	        float damageMult = manaAmount * 0.75f;
	        int projectile = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, (int)((double)damage * damageMult), knockBack, player.whoAmI, 0.0f, 0.0f);
	    	Main.projectile[projectile].scale = manaAmount;
	    	return false;
		}
		
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.SoulofFright, 10);
			recipe.AddIngredient(ItemID.AdamantiteBar, 7);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	        recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.SoulofFright, 10);
			recipe.AddIngredient(ItemID.TitaniumBar, 7);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}
