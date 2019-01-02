using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.CalamityCustomThrowingDamage;

namespace CalamityMod.Items.Weapons 
{
	public class LunicEye : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Lunic Eye");
			Tooltip.SetDefault("Fires lunic beams that reduce enemy protection\nProjectile damage is multiplied by all of your damage bonuses");
		}

		public override void SetDefaults()
		{
			item.width = 80;
			item.damage = 7;
			item.rare = 5;
			item.useAnimation = 15;
			item.useTime = 15;
			item.useStyle = 5;
			item.knockBack = 4.5f;
			item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon");
			item.autoReuse = true;
			item.noMelee = true;
			item.height = 50;
			item.value = 100000;
			item.shoot = mod.ProjectileType("LunicBeam");
			item.shootSpeed = 13f;
		}
		
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-15, 0);
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
			recipe.AddIngredient(null, "Stardust", 20);
			recipe.AddIngredient(null, "AerialiteBar", 15);
			recipe.AddIngredient(ItemID.SunplateBlock, 15);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}
