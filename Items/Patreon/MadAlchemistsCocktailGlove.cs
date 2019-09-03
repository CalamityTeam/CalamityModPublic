using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Patreon
{
    public class MadAlchemistsCocktailGlove : ModItem
    {
		private int FlaskType = 0;
		private int BaseDamage = 300;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mad Alchemist's Cocktail Glove");
            Tooltip.SetDefault("Fires a variety of high-velocity flasks that have various effects\n" +
				"Right click to throw a flask that inflicts a variety of debuffs");
        }

        public override void SetDefaults()
        {
            item.damage = BaseDamage;
            item.magic = true;
			item.noUseGraphic = true;
            item.mana = 12;
            item.width = 26;
            item.height = 36;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = 1;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item106;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("MadAlchemistsCocktailRed");
            item.shootSpeed = 12f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 21;
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
				item.damage = (int)((double)BaseDamage * 1.5);
			else
				item.damage = BaseDamage;

			return base.CanUseItem(player);
		}

		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			if (player.altFunctionUse == 2)
			{
				type = mod.ProjectileType("MadAlchemistsCocktailAlt");
				Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, (int)((float)BaseDamage * 1.5f * player.magicDamage), knockBack, player.whoAmI, 0f, 0f);
			}
			else
			{
				switch (FlaskType)
				{
					case 0:
						type = mod.ProjectileType("MadAlchemistsCocktailRed");
						break;
					case 1:
						type = mod.ProjectileType("MadAlchemistsCocktailBlue");
						break;
					case 2:
						type = mod.ProjectileType("MadAlchemistsCocktailGreen");
						break;
					case 3:
						type = mod.ProjectileType("MadAlchemistsCocktailPurple");
						break;
					default:
						break;
				}

				Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, (int)((float)BaseDamage * player.magicDamage), knockBack, player.whoAmI, 0f, 0f);

				FlaskType++;
				if (FlaskType > 3)
					FlaskType = 0;
			}

			return false;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.ToxicFlask);
			recipe.AddIngredient(ItemID.BottledWater, 15);
			recipe.AddIngredient(ItemID.Leather, 5);
			recipe.AddIngredient(null, "EffulgentFeather", 5);
			recipe.AddIngredient(null, "CoreofEleum", 5);
			recipe.AddIngredient(null, "CoreofCinder", 5);
			recipe.AddIngredient(null, "CoreofChaos", 5);
			recipe.AddIngredient(null, "CoreofCalamity");
			recipe.AddTile(TileID.AlchemyTable);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
