using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class Mariana : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mariana");
			Tooltip.SetDefault("Tropical and deadly\n" +
				"Enemies explode into water orbs on death");
		}

		public override void SetDefaults()
		{
			item.damage = 90;
			item.width = 54;
			item.height = 62;
			item.melee = true;
			item.useAnimation = 24;
			item.useStyle = 1;
			item.useTime = 24;
			item.useTurn = true;
			item.knockBack = 6.5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.value = Item.buyPrice(0, 60, 0, 0);
			item.rare = 7;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.ChlorophyteClaymore);
			recipe.AddIngredient(ItemID.Coral, 3);
			recipe.AddIngredient(ItemID.Starfish, 3);
			recipe.AddIngredient(ItemID.Seashell, 3);
			recipe.AddIngredient(null, "DepthCells", 10);
			recipe.AddIngredient(null, "Lumenite", 10);
			recipe.AddIngredient(null, "Tenebris", 5);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

		public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
		{
			if (target.life <= 0)
			{
				int num251 = Main.rand.Next(4, 6);
				for (int num252 = 0; num252 < num251; num252++)
				{
					Vector2 value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
					while (value15.X == 0f && value15.Y == 0f)
					{
						value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
					}
					value15.Normalize();
					value15 *= (float)Main.rand.Next(70, 101) * 0.1f;
					Projectile.NewProjectile(target.Center.X, target.Center.Y, value15.X, value15.Y, mod.ProjectileType("MarianaProjectile"), (int)((float)item.damage * player.meleeDamage), knockback, player.whoAmI, 0f, 0f);
				}
				for (int num621 = 0; num621 < 30; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 59, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 50; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 59, 0f, 0f, 100, default(Color), 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 59, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num624].velocity *= 2f;
				}
			}
		}

		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			if (Main.rand.Next(3) == 0)
			{
				int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 59);
			}
		}
	}
}
