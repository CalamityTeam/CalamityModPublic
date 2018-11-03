using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Cryogen {
public class GlacialCrusher : ModItem
{
	public override void SetStaticDefaults()
	{
		DisplayName.SetDefault("Glacial Crusher");
	}

	public override void SetDefaults()
	{
		item.width = 60;  //The width of the .png file in pixels divided by 2.
		item.damage = 59;  //Keep this reasonable please.
		item.melee = true;  //Dictates whether this is a melee-class weapon.
		item.useAnimation = 27;
		item.useTime = 27;  //Ranges from 1 to 55. 
		item.useTurn = true;
		item.useStyle = 1;
		item.knockBack = 6.5f;  //Ranges from 1 to 9.
		item.UseSound = SoundID.Item1;
		item.autoReuse = true;  //Dictates whether the weapon can be "auto-fired".
		item.height = 58;  //The height of the .png file in pixels divided by 2.
		item.maxStack = 1;
		item.value = 450000;  //Value is calculated in copper coins.
		item.rare = 5;  //Ranges from 1 to 11.
		item.shoot = mod.ProjectileType("Iceberg");
		item.shootSpeed = 12f;
	}
	
	public override void AddRecipes()
	{
		ModRecipe recipe = new ModRecipe(mod);
		recipe.AddIngredient(null, "CryoBar", 10);
        recipe.AddTile(TileID.IceMachine);
        recipe.SetResult(this);
        recipe.AddRecipe();
	}

    public override void MeleeEffects(Player player, Rectangle hitbox)
    {
        if (Main.rand.Next(3) == 0)
        {
        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 67);
        }
    }
    
    public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
    {
		target.AddBuff(BuffID.Frostburn, 300);
	}
}}
