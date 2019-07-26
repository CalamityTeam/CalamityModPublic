using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Accessories.Wings
{
	[AutoloadEquip(EquipType.Wings)]
	public class StarlightWings : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Starlight Wings");
			Tooltip.SetDefault("Wings of the Nightingale\n" +
				"Horizontal speed: 7.5\n" +
				"Acceleration multiplier: 1\n" +
				"Average vertical speed\n" +
				"Flight time: 150");
		}

		public override void SetDefaults()
		{
			item.width = 22;
			item.height = 20;
			item.value = Item.buyPrice(0, 39, 99, 99);
			item.rare = 5;
			item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			if (player.controlJump && player.wingTime > 0f && !player.jumpAgainCloud && player.jump == 0 && player.velocity.Y != 0f && !hideVisual)
			{
				int num59 = 4;
				if (player.direction == 1)
				{
					num59 = -40;
				}
				int num60 = Dust.NewDust(new Vector2(player.position.X + (float)(player.width / 2) + (float)num59, player.position.Y + (float)(player.height / 2) - 15f), 30, 30, 173, 0f, 0f, 100, default(Color), 2.4f);
				Main.dust[num60].noGravity = true;
				Main.dust[num60].velocity *= 0.3f;
				if (Main.rand.Next(10) == 0)
				{
					Main.dust[num60].fadeIn = 2f;
				}
				Main.dust[num60].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
			}
			player.wingTimeMax = 150;
		}

		public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
		{
			ascentWhenFalling = 0.5f;
			ascentWhenRising = 0.1f;
			maxCanAscendMultiplier = 0.5f;
			maxAscentMultiplier = 1.5f;
			constantAscend = 0.1f;
		}

		public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
		{
			speed = 7.5f;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "VerstaltiteBar", 5);
			recipe.AddIngredient(null, "EssenceofEleum");
			recipe.AddIngredient(ItemID.SoulofFlight, 20);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
