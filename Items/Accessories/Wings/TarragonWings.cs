using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Accessories.Wings
{
    [AutoloadEquip(EquipType.Wings)]
	public class TarragonWings : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Tarragon Wings");
			Tooltip.SetDefault("Born of the jungle\n" +
				"Horizontal speed: 9.5\n" +
				"Acceleration multiplier: 2.5\n" +
				"Great vertical speed\n" +
				"Flight time: 210\n" +
				"+15 defense and +2 life regen while wearing the Tarragon Armor");
		}

		public override void SetDefaults()
		{
			item.width = 22;
			item.height = 20;
			item.value = Item.buyPrice(0, 39, 99, 99);
			item.accessory = true;
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

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			if (player.controlJump && player.wingTime > 0f && !player.jumpAgainCloud && player.jump == 0 && player.velocity.Y != 0f && !hideVisual)
			{
				int num59 = 4;
				if (player.direction == 1)
				{
					num59 = -40;
				}
				int num60 = Dust.NewDust(new Vector2(player.position.X + (float)(player.width / 2) + (float)num59, player.position.Y + (float)(player.height / 2) - 15f), 30, 30, 75, 0f, 0f, 100, default, 2.4f);
				Main.dust[num60].noGravity = true;
				Main.dust[num60].velocity *= 0.3f;
				if (Main.rand.NextBool(10))
				{
					Main.dust[num60].fadeIn = 2f;
				}
				Main.dust[num60].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
			}
			player.wingTimeMax = 210;
			player.noFallDmg = true;
		}

		public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
		{
			CalamityPlayer modPlayer = player.GetCalamityPlayer();

			if (modPlayer.tarraSet)
			{
				player.statDefense += 15;
				player.lifeRegen += 2;
			}

			ascentWhenFalling = 0.85f;
			ascentWhenRising = 0.15f;
			maxCanAscendMultiplier = 1f;
			maxAscentMultiplier = 3f;
			constantAscend = 0.135f;
		}

		public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
		{
			speed = 9.5f;
			acceleration *= 2.5f;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "UeliaceBar", 5);
			recipe.AddIngredient(ItemID.SoulofFlight, 30);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
