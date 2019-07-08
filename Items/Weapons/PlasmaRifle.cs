using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons
{
	public class PlasmaRifle : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Plasma Rifle");
			Tooltip.SetDefault("Fires a plasma blast that explodes\n" +
				"Right click to fire plasma bolts");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 460;
	        item.mana = 40;
	        item.magic = true;
	        item.width = 48;
	        item.height = 22;
	        item.useTime = 40;
	        item.useAnimation = 40;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 4f;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PlasmaBlast");
	        item.autoReuse = true;
	        item.shootSpeed = 12f;
	        item.shoot = mod.ProjectileType("PlasmaShot");
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 12;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-10, 0);
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}
	    
	    public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				item.mana = 5;
				item.useTime = 8;
				item.useAnimation = 8;
				item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PlasmaBolt");
			}
			else
			{
				item.mana = 40;
				item.useTime = 40;
				item.useAnimation = 40;
				item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PlasmaBlast");
			}
			return base.CanUseItem(player);
		}
	    
	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	if (player.altFunctionUse == 2)
	    	{
	    		Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("PlasmaBolt"), (int)((double)damage * 0.75), knockBack, player.whoAmI, 0.0f, 0.0f);
	    	}
	    	else
	    	{
	    		Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("PlasmaShot"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
	    	}
			return false;
		}
	
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "UeliaceBar", 7);
	        recipe.AddIngredient(ItemID.Musket);
	        recipe.AddIngredient(ItemID.ToxicFlask);
	        recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	        recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "UeliaceBar", 7);
	        recipe.AddIngredient(ItemID.TheUndertaker);
	        recipe.AddIngredient(ItemID.ToxicFlask);
	        recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}