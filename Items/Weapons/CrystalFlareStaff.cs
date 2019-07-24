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
	public class CrystalFlareStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Crystal Flare Staff");
			Tooltip.SetDefault("Fires blue frost flames that explode");
			Item.staff[item.type] = true;
		}

	    public override void SetDefaults()
	    {
	        item.damage = 49;
	        item.magic = true;
	        item.mana = 15;
	        item.width = 44;
	        item.height = 46;
	        item.useTime = 12;
	        item.useAnimation = 24;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 5.25f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
	        item.UseSound = SoundID.Item20;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("SpiritFlameCurse");
	        item.shootSpeed = 14f;
	    }
	    
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "EssenceofEleum", 3);
	        recipe.AddIngredient(ItemID.CrystalShard, 15);
	        recipe.AddIngredient(ItemID.FrostStaff);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}