using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class TeslaCannon : ModItem
	{
		private int BaseDamage = 8000;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Tesla Cannon");
			Tooltip.SetDefault("Lightweight energy cannon that blasts an intense electrical beam that explodes\n" +
				"Beams can arc to nearby targets\n" +
				"Inflicts severe nervous system damage to organic targets");
		}

		public override void SetDefaults()
		{
			item.width = 78;
			item.height = 28;
			item.magic = true;
			item.damage = BaseDamage;
			item.knockBack = 10f;
			item.useTime = 90;
			item.useAnimation = 90;
			item.autoReuse = true;

			item.useStyle = ItemUseStyleID.HoldingOut;
			item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/TeslaCannonFire");
			item.noMelee = true;

			item.value = Item.buyPrice(1, 80, 0, 0);
			item.rare = 10;
			item.Calamity().customRarity = CalamityRarity.RareVariant;

			item.shoot = ModContent.ProjectileType<TeslaCannonShot>();
			item.shootSpeed = 1f;
			item.useAmmo = AmmoID.Bullet;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Vector2 velocity = new Vector2(speedX, speedY);
			if (velocity.Length() > 5f)
			{
				velocity.Normalize();
				velocity *= 5f;
			}

			float SpeedX = velocity.X + (float)Main.rand.Next(-1, 2) * 0.02f;
			float SpeedY = velocity.Y + (float)Main.rand.Next(-1, 2) * 0.02f;

			Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<TeslaCannonShot>(), damage, knockBack, player.whoAmI, 0f, 0f);

			// Consume 30 ammo per shot
			CalamityGlobalItem.ConsumeAdditionalAmmo(player, item, 30);

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
