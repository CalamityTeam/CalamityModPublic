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
	public class ElementalBlaster : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Elemental Blaster");
			Tooltip.SetDefault("Does not consume ammo\nFires a storm of rainbow blasts");
		}

	    public override void SetDefaults()
	    {
			item.damage = 77;
			item.ranged = true;
			item.width = 104;
			item.height = 42;
			item.useTime = 2;
			item.useAnimation = 6;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 1.75f;
			item.value = 1000000;
			item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PlasmaBolt");
			item.autoReuse = true;
			item.shoot = mod.ProjectileType("RainbowBlast");
			item.shootSpeed = 18f;
		}
	    
	    public override Vector2? HoldoutOffset()
		{
			return new Vector2(-15, 0);
		}
	    
	    public override void ModifyTooltips(List<TooltipLine> list)
	    {
	        foreach (TooltipLine line2 in list)
	        {
	            if (line2.mod == "Terraria" && line2.Name == "ItemName")
	            {
	                line2.overrideColor = new Color(0, 255, 200);
	            }
	        }
	    }
		
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "SpectralstormCannon");
            recipe.AddIngredient(null, "ClockGatlignum");
            recipe.AddIngredient(null, "PaintballBlaster");
            recipe.AddIngredient(null, "GalacticaSingularity", 5);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
		}
	}
}