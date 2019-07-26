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
	public class Megalodon : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Megalodon");
			Tooltip.SetDefault("50% chance to not consume ammo");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 32;
	        item.ranged = true;
	        item.width = 72;
	        item.height = 32;
	        item.useTime = 6;
	        item.useAnimation = 6;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 2.5f;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
	        item.UseSound = SoundID.Item11;
	        item.autoReuse = true;
	        item.shoot = 10;
	        item.shootSpeed = 16f;
	        item.useAmmo = 97;
	    }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
		    float SpeedX = speedX + (float) Main.rand.Next(-10, 11) * 0.05f;
		    float SpeedY = speedY + (float) Main.rand.Next(-10, 11) * 0.05f;
		    Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
		    return false;
		}

	    public override bool ConsumeAmmo(Player player)
	    {
	    	if (Main.rand.Next(0, 100) < 50)
	    		return false;
	    	return true;
	    }

	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.Megashark);
            recipe.AddIngredient(null, "DepthCells", 10);
            recipe.AddIngredient(null, "Lumenite", 10);
            recipe.AddIngredient(null, "Tenebris", 5);
            recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}
