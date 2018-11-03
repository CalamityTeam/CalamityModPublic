using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items {
public class AstralHamaxe : ModItem
{
	public override void SetStaticDefaults()
	{
		DisplayName.SetDefault("Astral Hamaxe");
	}
		
    public override void SetDefaults()
    {
        item.damage = 70;
        item.crit += 25;
        item.melee = true;
        item.width = 38;
        item.height = 36;
        item.useTime = 10;
        item.useAnimation = 20;
        item.useTurn = true;
        item.axe = 30;
        item.hammer = 150;
        item.useStyle = 1;
        item.knockBack = 5f;
        item.value = 350000;
        item.rare = 7;
        item.UseSound = SoundID.Item1;
        item.autoReuse = true;
        item.tileBoost += 3;
    }
    
    public override void MeleeEffects(Player player, Rectangle hitbox)
    {
        if (Main.rand.Next(5) == 0)
        {
        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 173);
        }
    }

    public override void AddRecipes()
    {
        ModRecipe recipe = new ModRecipe(mod);
        recipe.AddIngredient(null, "AstralBar", 8);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.SetResult(this);
        recipe.AddRecipe();
    }
}}