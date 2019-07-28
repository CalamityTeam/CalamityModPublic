using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class CosmicBolter : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cosmic Bolter");
			Tooltip.SetDefault("Fires three bouncing energy bolts");
		}

		public override void SetDefaults()
		{
			item.damage = 60;
			item.ranged = true;
			item.width = 40;
			item.height = 76;
			item.useTime = 19;
			item.useAnimation = 19;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 2.75f;
			item.value = Item.buyPrice(0, 80, 0, 0);
			item.rare = 8;
			item.UseSound = SoundID.Item75;
			item.autoReuse = true;
			item.shoot = mod.ProjectileType("LunarBolt2");
			item.shootSpeed = 10f;
			item.useAmmo = 40;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
			float num117 = 0.314159274f;
			int num118 = 3;
			Vector2 vector7 = new Vector2(speedX, speedY);
			vector7.Normalize();
			vector7 *= 30f;
			bool flag11 = Collision.CanHit(vector2, 0, 0, vector2 + vector7, 0, 0);
			for (int num119 = 0; num119 < num118; num119++)
			{
				float num120 = (float)num119 - ((float)num118 - 1f) / 2f;
				Vector2 value9 = vector7.RotatedBy((double)(num117 * num120), default(Vector2));
				if (!flag11)
				{
					value9 -= vector7;
				}
				int num121 = Projectile.NewProjectile(vector2.X + value9.X, vector2.Y + value9.Y, speedX, speedY, mod.ProjectileType("LunarBolt2"), damage, knockBack, player.whoAmI, 0f, 0f);
				Main.projectile[num121].noDropItem = true;
			}
			return false;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "LunarianBow");
			recipe.AddIngredient(null, "LivingShard", 5);
			recipe.AddIngredient(ItemID.HallowedBar, 5);
			recipe.AddIngredient(ItemID.SoulofSight, 5);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
