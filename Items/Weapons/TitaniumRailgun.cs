using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons {
public class TitaniumRailgun : ModItem
{
	public override void SetStaticDefaults()
	{
		DisplayName.SetDefault("Titanium Railgun");
	}

    public override void SetDefaults()
    {
        item.damage = 50;
        item.ranged = true;
        item.width = 62;
        item.height = 26;
        item.useTime = 25;
        item.useAnimation = 25;
        item.useStyle = 5;
        item.noMelee = true; //so the item's animation doesn't do damage
        item.knockBack = 3.5f;
        item.value = 550000;
        item.rare = 5;
        item.UseSound = SoundID.Item72;
        item.autoReuse = true;
        item.shootSpeed = 6f;
        item.shoot = mod.ProjectileType("TitRail");
        item.useAmmo = 97;
    }
    
    public override Vector2? HoldoutOffset()
	{
		return new Vector2(-5, 0);
	}
    
    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
	{
    	Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("TitRail"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
    	return false;
	}

    public override void AddRecipes()
    {
        ModRecipe recipe = new ModRecipe(mod);
        recipe.AddIngredient(ItemID.TitaniumBar, 10);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.SetResult(this);
        recipe.AddRecipe();
    }
}}