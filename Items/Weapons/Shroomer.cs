using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons {
public class Shroomer : ModItem
{
	public override void SetStaticDefaults()
	{
		DisplayName.SetDefault("Shroomer");
	}

    public override void SetDefaults()
    {
        item.damage = 200;
        item.ranged = true;
        item.width = 96;
        item.height = 40;
        item.crit += 35;
        item.useTime = 26;
        item.useAnimation = 26;
        item.useStyle = 5;
        item.noMelee = true;
        item.knockBack = 9.75f;
        item.value = 900000;
        item.rare = 9;
        item.UseSound = SoundID.Item40;
        item.autoReuse = true;
        item.shoot = 10;
        item.shootSpeed = 10f;
        item.useAmmo = 97;
    }
    
    public override Vector2? HoldoutOffset()
	{
		return new Vector2(-25, 0);
	}
    
    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
	{
    	Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
    	if (Main.rand.Next(5) == 0)
    	{
    		Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("Shroom"), (int)((double)damage * 1.5f), knockBack, player.whoAmI, 0.0f, 0.0f);
    	}
    	return false;
	}

    public override void AddRecipes()
	{
		ModRecipe recipe = new ModRecipe(mod);
        recipe.AddIngredient(ItemID.SniperRifle);
        recipe.AddIngredient(ItemID.ShroomiteBar, 11);
        recipe.AddIngredient(ItemID.FragmentVortex, 15);
        recipe.AddTile(TileID.LunarCraftingStation);
        recipe.SetResult(this);
        recipe.AddRecipe();
	}
}}