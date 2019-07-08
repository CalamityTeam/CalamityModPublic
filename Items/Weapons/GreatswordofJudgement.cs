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
	public class GreatswordofJudgement : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Greatsword of Judgement");
			Tooltip.SetDefault("A pale white sword from a forgotten land\n" +
								"Fires a white orb that emits white rain on death for a time\n" +
			                   "You can hear faint yet comforting whispers emanating from the blade\n" +
					            "'No matter where you may be you are never alone\n" +
					            "I shall always be at your side, my lord'");
		}

		public override void SetDefaults()
		{
			item.width = 70;
			item.damage = 58;
			item.melee = true;
			item.useAnimation = 18;
			item.useStyle = 1;
			item.useTime = 18;
			item.useTurn = true;
			item.knockBack = 7f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 72;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.shoot = mod.ProjectileType("Judgement");
			item.shootSpeed = 10f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 12;
		}
	
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.LunarBar, 11);
	        recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}
