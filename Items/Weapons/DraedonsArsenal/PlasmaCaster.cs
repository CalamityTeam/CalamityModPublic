using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class PlasmaCaster : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Plasma Caster");
			Tooltip.SetDefault("Industrial tool used to fuse metal together with super-heated plasma\n" +
				"Melts through target defense to deal extra damage to high defense targets\n" +
				"Right click for turbo mode");
		}

		public override void SetDefaults()
		{
			item.width = 62;
			item.height = 30;
			item.magic = true;
			item.damage = 1100;
			item.knockBack = 7f;
			item.useTime = 45;
			item.useAnimation = 45;
			item.autoReuse = true;

			item.useStyle = ItemUseStyleID.HoldingOut;
			item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PlasmaCasterFire");
			item.noMelee = true;

			item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
			item.rare = 10;
			item.Calamity().customRarity = CalamityRarity.Turquoise;

			item.shoot = ModContent.ProjectileType<PlasmaCasterShot>();
			item.shootSpeed = 5f;

			item.Calamity().Chargeable = true;
			item.Calamity().ChargeMax = 190;
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				item.useAnimation = 15;
				item.useTime = 15;
				item.damage = BaseDamage / 3;
				item.knockBack = 3f;
			}
			else
			{
				item.useAnimation = 45;
				item.useTime = 45;
				item.damage = BaseDamage;
				item.knockBack = 7f;
			}
			return base.CanUseItem(player);
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Vector2 velocity = new Vector2(speedX, speedY);
			if (velocity.Length() > 5f)
			{
				velocity.Normalize();
				velocity *= 5f;
			}

			int ammoConsumed = 20;
			float SpeedX = velocity.X + (float)Main.rand.Next(-3, 4) * 0.05f;
			float SpeedY = velocity.Y + (float)Main.rand.Next(-3, 4) * 0.05f;
			if (player.altFunctionUse == 2)
			{
				ammoConsumed = 5;
				SpeedX = velocity.X + (float)Main.rand.Next(-15, 16) * 0.05f;
				SpeedY = velocity.Y + (float)Main.rand.Next(-15, 16) * 0.05f;
			}

			Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<PlasmaCasterShot>(), damage, knockBack, player.whoAmI, 0f, 0f);

			// Consume 20 or 5 ammo per shot
			CalamityGlobalItem.ConsumeAdditionalAmmo(player, item, ammoConsumed);

			return false;
		}

		// Disable vanilla ammo consumption
		public override bool ConsumeAmmo(Player player)
		{
			return false;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-10, 0);
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 15);
			recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 15);
			recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 10);
			recipe.AddIngredient(ItemID.LunarBar, 5);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
