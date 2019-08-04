using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Patreon
{
    public class Karasawa : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Karasawa");
			Tooltip.SetDefault("...This is heavy...too heavy.");
		}

		public override void SetDefaults()
		{
			item.width = 94;
			item.height = 44;
			item.ranged = true;
			item.damage = 1400;
			item.knockBack = 12f;
			item.useTime = 52;
			item.useAnimation = 52;
			item.autoReuse = true;

			item.useStyle = 5;
			item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/MechGaussRifle");
			item.noMelee = true;

			item.value = Item.buyPrice(1, 80, 0, 0);
			item.rare = 10;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 21;

			item.shoot = mod.ProjectileType("KarasawaShot");
			item.shootSpeed = 1f;
			item.useAmmo = AmmoID.Bullet;
		}

		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Vector2 velocity = new Vector2(speedX, speedY);
			if (velocity.Length() > 5f)
			{
				velocity.Normalize();
				velocity *= 5f;
			}
			Projectile.NewProjectile(position.X, position.Y, velocity.X, velocity.Y, mod.ProjectileType("KarasawaShot"), damage, knockBack, player.whoAmI, 0f, 0f);

			// Consume 5 ammo per shot
			CalamityGlobalItem.ConsumeAdditionalAmmo(player, item, 5);

			return false;
		}

		// Disable vanilla ammo consumption
		public override bool ConsumeAmmo(Player player)
		{
			return false;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-20, 0);
		}

		public override void AddRecipes()
		{
			ModRecipe r = new ModRecipe(mod);
			r.AddIngredient(null, "CrownJewel");
			r.AddIngredient(null, "GalacticaSingularity", 5);
			r.AddIngredient(null, "BarofLife", 10);
			r.AddIngredient(null, "CosmiliteBar", 15);
			r.AddTile(TileID.LunarCraftingStation);
			r.SetResult(this);
			r.AddRecipe();
		}
	}
}
