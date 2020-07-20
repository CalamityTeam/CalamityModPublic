using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class LaserRifle : ModItem
	{
		private int BaseDamage = 100;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Heavy Laser Rifle");
			Tooltip.SetDefault("Laser weapon used by heavy infantry units in Yharim's army\n" +
				"Incredibly accurate, but lacks the power to punch through defensive targets");
		}

		public override void SetDefaults()
		{
			item.width = 84;
			item.height = 28;
			item.ranged = true;
			item.damage = BaseDamage;
			item.knockBack = 4f;
			item.useTime = 25;
			item.useAnimation = 25;
			item.autoReuse = true;

			item.useStyle = ItemUseStyleID.HoldingOut;
			item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserRifleFire");
			item.noMelee = true;

			item.value = CalamityGlobalItem.Rarity10BuyPrice;
			item.rare = 10;
			item.Calamity().customRarity = CalamityRarity.RareVariant;

			item.shoot = ModContent.ProjectileType<LaserRifleShot>();
			item.shootSpeed = 5f;

			item.Calamity().Chargeable = true;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Vector2 velocity = new Vector2(speedX, speedY);
			if (velocity.Length() > 5f)
			{
				velocity.Normalize();
				velocity *= 5f;
			}

			for (int i = 0; i < 2; i++)
			{
				float SpeedX = velocity.X + Main.rand.Next(-1, 2) * 0.05f;
				float SpeedY = velocity.Y + Main.rand.Next(-1, 2) * 0.05f;
				Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<LaserRifleShot>(), damage, knockBack, player.whoAmI, i, 0f);
			}

			// Consume 4 ammo per shot
			CalamityGlobalItem.ConsumeAdditionalAmmo(player, item, 4);

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
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 15);
			recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 15);
			recipe.AddIngredient(ModContent.ItemType<MeldiateBar>(), 5);
			recipe.AddIngredient(ItemID.LaserRifle);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
