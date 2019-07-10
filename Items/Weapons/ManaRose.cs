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
	public class ManaRose : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mana Rose");
			Tooltip.SetDefault("Casts a mana bolt that explodes into smaller bolts");
			Item.staff[item.type] = true;
		}

	    public override void SetDefaults()
	    {
	        item.damage = 9;
	        item.magic = true;
	        item.mana = 8;
	        item.width = 38;
	        item.height = 38;
	        item.useTime = 27;
	        item.useAnimation = 27;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 3.25f;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
	        item.UseSound = SoundID.Item109;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("ManaBolt");
	        item.shootSpeed = 10f;
	    }
	    
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.NaturesGift);
	        recipe.AddIngredient(ItemID.JungleRose);
	        recipe.AddIngredient(ItemID.Moonglow, 5);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}
