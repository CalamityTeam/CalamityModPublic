using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class PlasmaCaster : ModItem
	{
		private int BaseDamage = 2800;

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
			item.damage = BaseDamage;
			item.knockBack = 7f;
			item.useTime = 45;
			item.useAnimation = 45;
			item.autoReuse = true;

			item.useStyle = 5;
			item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PlasmaCasterFire");
			item.noMelee = true;

			item.value = Item.buyPrice(1, 80, 0, 0);
			item.rare = 10;
			item.Calamity().customRarity = CalamityRarity.RareVariant;

			item.shoot = ModContent.ProjectileType<PlasmaCasterShot>();
			item.shootSpeed = 1f;
			item.useAmmo = AmmoID.Bullet;
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

		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
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

		/*public override void AddRecipes()
		{
			ModRecipe r = new ModRecipe(mod);
			r.AddIngredient(null, "CrownJewel");
			r.AddIngredient(null, "GalacticaSingularity", 5);
			r.AddIngredient(null, "BarofLife", 10);
			r.AddIngredient(null, "CosmiliteBar", 15);
			r.AddTile(TileID.LunarCraftingStation);
			r.SetResult(this);
			r.AddRecipe();
		}*/
	}
}
