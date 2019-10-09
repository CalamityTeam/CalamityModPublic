using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Accessories.Wings
{
    [AutoloadEquip(EquipType.Wings)]
	public class HadarianWings : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hadarian Wings");
			Tooltip.SetDefault("Powered by the Astral Infection\n" +
				"Hold down to hover during flight, greatly extending flight duration\n" +
				"Horizontal speed: 9\n" +
				"Acceleration multiplier: 1.75\n" +
				"Good vertical speed\n" +
				"Flight time: 120\n" +
				"20% increased movement speed and 40% increased jump speed while wearing the Astral Armor");
		}

		public override void SetDefaults()
		{
			item.width = 24;
			item.height = 36;
			item.value = Item.buyPrice(0, 39, 99, 99);
			item.rare = 9;
			item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			if (player.controlJump && player.wingTime > 0f && !player.jumpAgainCloud && player.jump == 0)
			{
				if (player.controlDown && !player.merman)
				{
					player.velocity.Y = player.velocity.Y * 0.9f;
					if (player.velocity.Y > -2f && player.velocity.Y < 1f)
					{
						player.velocity.Y = 1E-05f;
					}
					player.wingTime += 0.75f;
				}
				if (player.velocity.Y != 0f && !hideVisual)
				{
					int num59 = 4;
					if (player.direction == 1)
					{
						num59 = -40;
					}
					int num60 = Dust.NewDust(new Vector2(player.position.X + (float)(player.width / 2) + (float)num59, player.position.Y + (float)(player.height / 2) - 15f), 30, 30, mod.DustType("AstralOrange"), 0f, 0f, 100, default, 1.75f);
					Main.dust[num60].noGravity = true;
					Main.dust[num60].velocity *= 0.3f;
					if (Main.rand.NextBool(10))
					{
						Main.dust[num60].fadeIn = 2f;
					}
					Main.dust[num60].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
				}
			}
			player.wingTimeMax = 120;
			player.noFallDmg = true;
		}

		public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
		{
			CalamityPlayer modPlayer = player.GetCalamityPlayer();

			if (modPlayer.astralStarRain)
			{
				player.moveSpeed += 0.2f;
				player.jumpSpeedBoost += 0.4f;
			}

			ascentWhenFalling = 0.8f;
			ascentWhenRising = 0.155f;
			maxCanAscendMultiplier = 1.05f;
			maxAscentMultiplier = 2.55f;
			constantAscend = 0.13f;
		}

		public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
		{
			speed = 9f;
			acceleration *= 1.75f;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "AstralBar", 8);
			recipe.AddIngredient(null, "HadarianMembrane", 10);
			recipe.AddIngredient(ItemID.SoulofFlight, 20);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
