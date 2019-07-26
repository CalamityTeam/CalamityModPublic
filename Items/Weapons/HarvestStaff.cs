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
	public class HarvestStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Harvest Staff");
			Tooltip.SetDefault("Casts flaming pumpkins");
			Item.staff[item.type] = true;
		}

		public override void SetDefaults()
		{
			item.damage = 15;
			item.magic = true;
			item.mana = 5;
			item.width = 28;
			item.height = 30;
			item.useTime = 23;
			item.useAnimation = 23;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 5;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
			item.UseSound = SoundID.Item20;
			item.autoReuse = true;
			item.shoot = mod.ProjectileType("FlamingPumpkin");
			item.shootSpeed = 10f;
			item.scale = 0.9f;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Pumpkin, 20);
			recipe.AddIngredient(ItemID.FallenStar, 5);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
