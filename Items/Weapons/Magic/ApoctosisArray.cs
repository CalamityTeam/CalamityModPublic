using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
	public class ApoctosisArray : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Apoctosis Array");
			Tooltip.SetDefault("Fires ion blasts that speed up and then explode\n" +
				"The higher your mana the more damage they will do\n" +
				"Astral steroids can inhibit the potential of this weapon");
		}

		public override void SetDefaults()
		{
			item.width = 98;
			item.damage = 49;
			item.magic = true;
			item.mana = 12;
			item.useAnimation = 7;
			item.useTime = 7;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.knockBack = 6.75f;
			item.UseSound = SoundID.Item91;
			item.autoReuse = true;
			item.noMelee = true;
			item.height = 34;
			item.value = Item.buyPrice(1, 20, 0, 0);
			item.rare = 10;
			item.shoot = ModContent.ProjectileType<IonBlast>();
			item.shootSpeed = 8f;
			item.Calamity().customRarity = CalamityRarity.Turquoise;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-25, 0);
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			float manaAmount = (float)player.statMana * 0.01f;
			float damageMult = manaAmount;
			float injectionNerf = player.Calamity().astralInjection ? 0.6f : 1f;
			int projectile = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, (int)(damage * damageMult * injectionNerf), knockBack, player.whoAmI);
			Main.projectile[projectile].scale = manaAmount * 0.375f;
			return false;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<IonBlaster>());
			recipe.AddIngredient(ItemID.LunarBar, 5);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
