using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Armor 
{
	[AutoloadEquip(EquipType.Legs)]
	public class DemonshadeGreaves : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Demonshade Greaves");
            Tooltip.SetDefault("Shadow speed");
        }

	    public override void SetDefaults()
	    {
	        item.width = 18;
	        item.height = 18;
			item.value = Item.buyPrice(3, 0, 0, 0);
			item.defense = 50; //15
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 16;
		}
	
	    public override void UpdateEquip(Player player)
	    {
	    	CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
    		modPlayer.shadowSpeed = true;
	        player.moveSpeed += 1f;
	    }
	
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "ShadowspecBar", 11);
	        recipe.AddTile(null, "DraedonsForge");
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}