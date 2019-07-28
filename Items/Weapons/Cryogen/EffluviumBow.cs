using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Cryogen
{
    public class EffluviumBow : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Effluvium Bow");
			Tooltip.SetDefault("Fires two mist arrows at once");
		}

		public override void SetDefaults()
		{
			item.damage = 51;
			item.ranged = true;
			item.width = 26;
			item.height = 70;
			item.useTime = 25;
			item.useAnimation = 25;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 4f;
			item.value = Item.buyPrice(0, 36, 0, 0);
			item.rare = 5;
			item.UseSound = SoundID.Item5;
			item.autoReuse = true;
			item.shoot = mod.ProjectileType("MistArrow");
			item.shootSpeed = 12f;
			item.useAmmo = 40;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "CryoBar", 7);
			recipe.AddTile(TileID.IceMachine);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			float SpeedA = speedX;
			float SpeedB = speedY;
			for (int index = 0; index < 2; ++index)
			{
				float num7 = speedX;
				float num8 = speedY;
				float SpeedX = speedX + (float)Main.rand.Next(-30, 31) * 0.05f;
				float SpeedY = speedY + (float)Main.rand.Next(-30, 31) * 0.05f;
				Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, mod.ProjectileType("MistArrow"), damage, knockBack, player.whoAmI, 0f, 0f);
			}
			return false;
		}
	}
}
