using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons {
public class TomeofFates : ModItem
{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Tome of Fates");
			Tooltip.SetDefault("Casts cosmic tentacles to spear your enemies\nCan randomly fire a brimstone tentacle for immense damage");
		}

    public override void SetDefaults()
    {
        item.damage = 79;
        item.magic = true;
        item.mana = 8;
        item.width = 28;
        item.crit = 3;
        item.height = 30;
        item.useTime = 5;
        item.useAnimation = 20;
        item.useStyle = 5;
        item.noMelee = true; //so the item's animation doesn't do damage
        item.knockBack = 3.5f;
        item.value = 300000;
        item.rare = 9;
        item.UseSound = SoundID.Item103;
        item.autoReuse = true;
        item.shoot = mod.ProjectileType("CosmicTentacle");
        item.shootSpeed = 17f;
    }
    
    public override void AddRecipes()
	{
		ModRecipe recipe = new ModRecipe(mod);
		recipe.AddIngredient(null, "MeldiateBar", 9);
		recipe.AddIngredient(ItemID.SpellTome);
        recipe.AddTile(TileID.LunarCraftingStation);
        recipe.SetResult(this);
        recipe.AddRecipe();
	}
    
    public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
	{
    	int i = Main.myPlayer;
		int num73 = damage;
		float num74 = knockBack;
    	num74 = player.GetWeaponKnockback(item, num74);
    	player.itemTime = item.useTime;
    	Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
    	float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
		float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
    	Vector2 value2 = new Vector2(num78, num79);
		value2.Normalize();
		Vector2 value3 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
		value3.Normalize();
		value2 = value2 * 4f + value3;
		value2.Normalize();
		value2 *= item.shootSpeed;
		int projChoice = Main.rand.Next(7);
		float num91 = (float)Main.rand.Next(10, 160) * 0.001f;
		if (Main.rand.Next(2) == 0)
		{
			num91 *= -1f;
		}
		float num92 = (float)Main.rand.Next(10, 160) * 0.001f;
		if (Main.rand.Next(2) == 0)
		{
			num92 *= -1f;
		}
		if (projChoice == 0)
		{
			Projectile.NewProjectile(vector2.X, vector2.Y, value2.X, value2.Y, mod.ProjectileType("BrimstoneTentacle"), (int)((double)num73 * 1.5f), num74, i, num92, num91);
		}
		else
		{
			Projectile.NewProjectile(vector2.X, vector2.Y, value2.X, value2.Y, mod.ProjectileType("CosmicTentacle"), num73, num74, i, num92, num91);
		}
    	return false;
	}
}}