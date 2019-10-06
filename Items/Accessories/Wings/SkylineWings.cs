using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories.Wings
{
    [AutoloadEquip(EquipType.Wings)]
	public class SkylineWings : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Skyline Wings");
			Tooltip.SetDefault("Horizontal speed: 6.25\n" +
				"Acceleration multiplier: 1\n" +
				"Average vertical speed\n" +
				"Flight time: 60");
		}

		public override void SetDefaults()
		{
			item.width = 22;
			item.height = 20;
			item.value = Item.buyPrice(0, 9, 0, 0);
			item.rare = 3;
			item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.wingTimeMax = 60;
			player.noFallDmg = true;
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
			speed = 6.25f;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "AerialiteBar", 5);
			recipe.AddIngredient(ItemID.Feather, 5);
			recipe.AddIngredient(ItemID.FallenStar, 5);
			recipe.AddIngredient(ItemID.Bone, 15);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
