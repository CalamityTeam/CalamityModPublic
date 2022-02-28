using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
	public class ScorchedEarth : ModItem
	{
		private int counter = 0;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Scorched Earth");
			Tooltip.SetDefault("Fires a burst of four fuel-air rockets which explode into cluster bombs\n" +
			"Each burst consumes two rockets each\n" +
			"Burns your targets to a fine crisp");
		}

		public override void SetDefaults()
		{
			item.damage = 500;
			item.ranged = true;
			item.useTime = 8;
			item.useAnimation = 32; // 4 shots in just over half a second
			item.reuseDelay = 60; // 1 second recharge
			item.width = 104;
			item.height = 44;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.noMelee = true;
			item.knockBack = 8.7f;
			item.value = CalamityGlobalItem.Rarity14BuyPrice;
			item.rare = ItemRarityID.Purple;
			item.Calamity().customRarity = CalamityRarity.DarkBlue;
			item.autoReuse = true;
			item.shootSpeed = 12.6f;
			item.shoot = ModContent.ProjectileType<ScorchedEarthRocket>();
			item.useAmmo = AmmoID.Rocket;
			item.Calamity().donorItem = true;
		}

		// Consume two ammo per fire
		public override bool ConsumeAmmo(Player player) => counter % 2 == 0;

		public override Vector2? HoldoutOffset() => new Vector2(-30, 0);

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<ScorchedEarthRocket>(), damage, knockBack, player.whoAmI);

			if (counter == 0)
			{
				Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/ScorchedEarthShot" + Main.rand.Next(1,4)), position);
			}

			counter++;
			if (counter == 4)
				counter = 0;
			return false;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<BlissfulBombardier>());
			recipe.AddIngredient(ModContent.ItemType<DarksunFragment>(), 10);
			recipe.AddIngredient(ItemID.FragmentSolar, 50);
			recipe.AddRecipeGroup("AnyAdamantiteBar", 15);
			recipe.AddTile(ModContent.TileType<CosmicAnvil>());
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
