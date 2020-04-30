using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class PulseRifle : ModItem
	{
		private int BaseDamage = 2500;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Draedon's Pulse Rifle");
			Tooltip.SetDefault("Draedon's high-efficiency electromagnetic pulse rifle\n" +
				"Incredibly accurate pulse weapon, crafted and wielded by Draedon to defend against his own creations\n" +
				"When the pulse hits a target it will arc to another nearby target\n" +
				"Inflicts exceptional damage against inorganic targets");
		}

		public override void SetDefaults()
		{
			item.width = 62;
			item.height = 22;
			item.ranged = true;
			item.damage = BaseDamage;
			item.knockBack = 0f;
			item.useTime = 35;
			item.useAnimation = 35;
			item.autoReuse = true;

			item.useStyle = 5;
			item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PulseRifleFire");
			item.noMelee = true;

			item.value = Item.buyPrice(1, 80, 0, 0);
			item.rare = 10;
			item.Calamity().customRarity = CalamityRarity.RareVariant;

			item.shoot = ModContent.ProjectileType<PulseRifleShot>();
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

			float SpeedX = velocity.X + (float)Main.rand.Next(-1, 2) * 0.05f;
			float SpeedY = velocity.Y + (float)Main.rand.Next(-1, 2) * 0.05f;

			Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<PulseRifleShot>(), damage, knockBack, player.whoAmI, 0f, 0f);

			// Consume 6 ammo per shot
			CalamityGlobalItem.ConsumeAdditionalAmmo(player, item, 6);

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
