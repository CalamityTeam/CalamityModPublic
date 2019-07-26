using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;
using CalamityMod.Projectiles;

namespace CalamityMod.Items.Weapons
{
	public class SpectralstormCannon : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spectralstorm Cannon");
			Tooltip.SetDefault("70% chance to not consume flares\n" +
				"Fires a storm of ectoplasm and flares");
		}

	    public override void SetDefaults()
	    {
			item.damage = 44;
			item.ranged = true;
			item.width = 66;
			item.height = 26;
			item.useTime = 3;
			item.useAnimation = 9;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 1.5f;
            item.value = Item.buyPrice(0, 95, 0, 0);
            item.rare = 9;
			item.UseSound = SoundID.Item11;
			item.autoReuse = true;
			item.shoot = 163;
			item.shootSpeed = 9.5f;
			item.useAmmo = 931;
		}

	    public override bool ConsumeAmmo(Player player)
	    {
	    	if (Main.rand.Next(0, 100) < 70)
	    		return false;
	    	return true;
	    }

		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
		    int num6 = Main.rand.Next(1, 2);
		    for (int index = 0; index < num6; ++index)
		    {
		        float num7 = speedX;
		        float num8 = speedY;
		        float SpeedX = speedX + (float) Main.rand.Next(-40, 41) * 0.05f;
		        float SpeedY = speedY + (float) Main.rand.Next(-40, 41) * 0.05f;
		        int projectile = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
		        Main.projectile[projectile].timeLeft = 200;
		    }
		    int proj = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, 297, damage, knockBack, player.whoAmI, 0f, 0f);
			Main.projectile[proj].GetGlobalProjectile<CalamityGlobalProjectile>(mod).forceRanged = true;
			return false;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "FirestormCannon");
            recipe.AddIngredient(ItemID.FragmentVortex, 20);
            recipe.AddIngredient(ItemID.Ectoplasm, 10);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
		}
	}
}
