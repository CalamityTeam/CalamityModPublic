using CalamityMod.Projectiles.Summon;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
	public class PlantationStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Plantation Staff");
			Tooltip.SetDefault("Summons a miniature plantera to protect you\n" +
			"Fires seeds, spiky balls, and spore clouds from afar to poison targets\n" +
			"Enrages when you get under 75% health and begins ramming enemies\n" +
			"Occupies 3 minion slots and there can only be one");
		}

		public override void SetDefaults()
		{
			item.damage = 55;
			item.mana = 10;
			item.width = 66;
			item.height = 70;
			item.useTime = item.useAnimation = 20;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.noMelee = true;
			item.knockBack = 1f;
			item.value = Item.buyPrice(0, 80, 0, 0);
			item.rare = ItemRarityID.Yellow;
			item.UseSound = SoundID.Item76;
			item.shoot = ModContent.ProjectileType<PlantSummon>();
			item.shootSpeed = 10f;
			item.summon = true;
		}

		public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0 && player.maxMinions >= 3;

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<EyeOfNight>());
			recipe.AddIngredient(ModContent.ItemType<DeepseaStaff>());
			recipe.AddIngredient(ItemID.OpticStaff);
			recipe.AddIngredient(ModContent.ItemType<LivingShard>(), 10);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<FleshOfInfidelity>());
            recipe.AddIngredient(ModContent.ItemType<DeepseaStaff>());
            recipe.AddIngredient(ItemID.OpticStaff);
			recipe.AddIngredient(ModContent.ItemType<LivingShard>(), 10);
			recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			if (player.altFunctionUse != 2)
			{
				CalamityUtils.KillShootProjectileMany(player, new int[] { type, ModContent.ProjectileType<PlantTentacle>() });

				float speed = item.shootSpeed;
				player.itemTime = item.useTime;
				Vector2 playerPos = player.RotatedRelativePoint(player.MountedCenter, true);
				float directionX = (float)Main.mouseX + Main.screenPosition.X - playerPos.X;
				float directionY = (float)Main.mouseY + Main.screenPosition.Y - playerPos.Y;
				if (player.gravDir == -1f)
				{
					directionY = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - playerPos.Y;
				}
				Vector2 spinningpoint = new Vector2(directionX, directionY);
				float vectorDist = spinningpoint.Length();
				if ((float.IsNaN(spinningpoint.X) && float.IsNaN(spinningpoint.Y)) || (spinningpoint.X == 0f && spinningpoint.Y == 0f))
				{
					spinningpoint.X = (float)player.direction;
					spinningpoint.Y = 0f;
					vectorDist = speed;
				}
				else
				{
					vectorDist = speed / vectorDist;
				}
				spinningpoint.X *= vectorDist;
				spinningpoint.Y *= vectorDist;
				playerPos.X = (float)Main.mouseX + Main.screenPosition.X;
				playerPos.Y = (float)Main.mouseY + Main.screenPosition.Y;
				spinningpoint = spinningpoint.RotatedBy(MathHelper.PiOver2, default);
				Projectile.NewProjectile(playerPos + spinningpoint, spinningpoint, type, damage, knockBack, player.whoAmI, 0f, 0f);
			}
			return false;
		}
	}
}
