using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons {
public class YinYo : ModItem
{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("YinYo");
			Tooltip.SetDefault("Fires light or dark shards when enemies are near\nLight shards fly up and down while dark shards fly left and right");
		}

    public override void SetDefaults()
    {
    	item.CloneDefaults(ItemID.Chik);
        item.damage = 34;
        item.useTime = 25;
        item.useAnimation = 25;
        item.useStyle = 5;
        item.channel = true;
        item.melee = true;
        item.knockBack = 3.2f;
        item.value = 130000;
        item.rare = 5;
        item.autoReuse = true;
        item.shoot = mod.ProjectileType("YinYo");
    }
    
    public override void AddRecipes()
	{
		ModRecipe recipe = new ModRecipe(mod);
        recipe.AddIngredient(ItemID.DarkShard);
        recipe.AddIngredient(ItemID.LightShard);
        recipe.AddTile(TileID.Anvils);
        recipe.SetResult(this);
        recipe.AddRecipe();
	}
}}