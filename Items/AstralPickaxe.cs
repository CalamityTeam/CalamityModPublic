using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items {
public class AstralPickaxe : ModItem
{
	public override void SetStaticDefaults()
	{
 		DisplayName.SetDefault("Astral Pickaxe");
 	}
	
    public override void SetDefaults()
    {
        item.damage = 58;
        item.crit += 25;
        item.melee = true;
        item.width = 36;
        item.height = 36;
        item.useTime = 5;
        item.useAnimation = 10;
        item.useTurn = true;
        item.pick = 200;
        item.useStyle = 1;
        item.knockBack = 5f;
        item.value = 350000;
        item.rare = 7;
        item.UseSound = SoundID.Item1;
        item.autoReuse = true;
        item.tileBoost += 3;
    }

    public override void AddRecipes()
    {
        ModRecipe recipe = new ModRecipe(mod);
        recipe.AddIngredient(null, "AstralBar", 7);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.SetResult(this);
        recipe.AddRecipe();
    }

    public override void MeleeEffects(Player player, Rectangle hitbox)
    {
        if (Main.rand.Next(5) == 0)
        {
        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 173);
        }
    }
}}