using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons {
public class SpectreRifle : ModItem
{
	public override void SetStaticDefaults()
	{
		DisplayName.SetDefault("Spectre Rifle");
	}

    public override void SetDefaults()
    {
        item.damage = 150;
        item.ranged = true;
        item.width = 68;
        item.height = 24;
        item.crit += 22;
        item.useTime = 25;
        item.useAnimation = 25;
        item.useStyle = 5;
        item.noMelee = true;
        item.knockBack = 7;
        item.value = 550000;
        item.rare = 8;
        item.UseSound = SoundID.Item40;
        item.autoReuse = false;
        item.shoot = 297;
        item.shootSpeed = 24f;
        item.useAmmo = 97;
    }
    
    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
	{
    	int projectile2 = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, 297, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
    	Main.projectile[projectile2].magic = false;
		Main.projectile[projectile2].ranged = true;
    	return false;
	}

    public override void AddRecipes()
    {
        ModRecipe recipe = new ModRecipe(mod);
        recipe.AddIngredient(ItemID.SpectreBar, 7);
        recipe.AddIngredient(null, "CoreofEleum", 3);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.SetResult(this);
        recipe.AddRecipe();
    }
}}