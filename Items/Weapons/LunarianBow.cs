using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons
{
	public class LunarianBow : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Lunarian Bow");
			Tooltip.SetDefault("Fires two bouncing energy bolts");
		}

		public override void SetDefaults()
		{
			item.damage = 21;
			item.ranged = true;
			item.width = 22;
			item.height = 58;
			item.useTime = 18;
			item.useAnimation = 18;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 2f;
			item.value = Item.buyPrice(0, 12, 0, 0);
			item.rare = 4;
			item.UseSound = SoundID.Item75;
			item.autoReuse = true;
			item.shoot = mod.ProjectileType("LunarBolt");
			item.shootSpeed = 8f;
			item.useAmmo = 40;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
			float num117 = 0.314159274f;
			int num118 = 2;
			Vector2 vector7 = new Vector2(speedX, speedY);
			vector7.Normalize();
			vector7 *= 15f;
			bool flag11 = Collision.CanHit(vector2, 0, 0, vector2 + vector7, 0, 0);
			for (int num119 = 0; num119 < num118; num119++)
			{
				float num120 = (float)num119 - ((float)num118 - 1f) / 2f;
				Vector2 value9 = vector7.RotatedBy((double)(num117 * num120), default(Vector2));
				if (!flag11)
				{
					value9 -= vector7;
				}
				int num121 = Projectile.NewProjectile(vector2.X + value9.X, vector2.Y + value9.Y, speedX, speedY, mod.ProjectileType("LunarBolt"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
				Main.projectile[num121].noDropItem = true;
			}
			return false;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.DemonBow);
			recipe.AddIngredient(ItemID.MoltenFury);
			recipe.AddIngredient(ItemID.BeesKnees);
			recipe.AddIngredient(null, "PurifiedGel", 10);
			recipe.AddTile(TileID.DemonAltar);
			recipe.SetResult(this);
			recipe.AddRecipe();
			recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.TendonBow);
			recipe.AddIngredient(ItemID.MoltenFury);
			recipe.AddIngredient(ItemID.BeesKnees);
			recipe.AddIngredient(null, "PurifiedGel", 10);
			recipe.AddTile(TileID.DemonAltar);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}