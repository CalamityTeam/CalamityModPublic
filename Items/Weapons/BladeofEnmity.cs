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
	public class BladeofEnmity : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blade of Enmity");
		}

		public override void SetDefaults()
		{
			item.width = 60;
			item.damage = 158;
			item.melee = true;
			item.useAnimation = 10;
			item.useStyle = 1;
			item.useTime = 10;
			item.useTurn = true;
			item.knockBack = 8f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 68;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "BarofLife", 5);
			recipe.AddIngredient(null, "CoreofCalamity", 3);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}

	    public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
	    {
			target.AddBuff(mod.BuffType("BrimstoneFlames"), 600);
		}
	}
}
