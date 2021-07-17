using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Weapons.Ranged
{
	public class HeavenlyGale : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Heavenly Gale");
			Tooltip.SetDefault("Converts wooden arrows into barrages of 5 random exo arrows\n" +
				"Green exo arrows explode into a tornado on death\n" +
				"Blue exo arrows cause a second group of arrows to fire on enemy hits\n" +
				"Orange exo arrows cause explosions on death\n" +
				"Teal exo arrows ignore enemy immunity frames\n" +
				"66% chance to not consume ammo");
		}

		public override void SetDefaults()
		{
			item.damage = 198;
			item.ranged = true;
			item.width = 44;
			item.height = 58;
			item.useTime = 15;
			item.useAnimation = 30;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.noMelee = true;
			item.knockBack = 4f;
			item.value = Item.buyPrice(2, 50, 0, 0);
			item.rare = ItemRarityID.Red;
			item.UseSound = SoundID.Item5;
			item.autoReuse = true;
			item.shoot = ProjectileID.WoodenArrowFriendly;
			item.shootSpeed = 12f;
			item.useAmmo = AmmoID.Arrow;
			item.Calamity().customRarity = CalamityRarity.Violet;
			item.Calamity().canFirePointBlankShots = true;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Vector2 source = player.RotatedRelativePoint(player.MountedCenter, true);
			float piOver10 = MathHelper.Pi * 0.1f;
			int arrowAmt = 5;
			Vector2 speed = new Vector2(speedX, speedY);
			speed.Normalize();
			speed *= 40f;
			bool canHit = Collision.CanHit(source, 0, 0, source + speed, 0, 0);
			for (int i = 0; i < arrowAmt; i++)
			{
				float offsetAmt = i - (arrowAmt - 1f) / 2f;
				Vector2 offset = speed.RotatedBy(piOver10 * offsetAmt);
				if (!canHit)
					offset -= speed;

				if (type == ProjectileID.WoodenArrowFriendly)
				{
					int[] arrowTypes = new int[5] 
					{	ProjectileType<TealExoArrow>(),
						ProjectileType<OrangeExoArrow>(),
						ProjectileType<GreenExoArrow>(),
						ProjectileType<BlueExoArrow>(),
						ProjectileType<TealExoArrow>()
					};
					int j = arrowTypes.Length;
					var rng = new Random();
					while (j > 1)
					{
						int k = rng.Next(j--);
						int randomArrow = arrowTypes[j];
						arrowTypes[j] = arrowTypes[k];
						arrowTypes[k] = randomArrow;
					}
					int newType = type;
					switch (i)
					{
						case 0:
						case 4:
							newType = ProjectileType<TealExoArrow>();
							break;
						case 1:
							newType = ProjectileType<MiniSharkron>();
							break;
						case 2:
							newType = ProjectileType<GreenExoArrow>();
							break;
						case 3:
							newType = ProjectileType<MiniSharkron>();
							break;
					}

					float dmgMult = 1f;
					if (arrowTypes[i] == ProjectileType<TealExoArrow>())
						dmgMult = 0.5f;

					Projectile.NewProjectile(source.X + offset.X, source.Y + offset.Y, speedX, speedY, arrowTypes[i], (int)(damage * dmgMult), knockBack, player.whoAmI);
				}
				else
				{
					int proj = Projectile.NewProjectile(source.X + offset.X, source.Y + offset.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
					Main.projectile[proj].noDropItem = true;
				}
			}
			return false;
		}

		public override bool ConsumeAmmo(Player player)
		{
			if (Main.rand.Next(0, 100) < 66)
				return false;
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemType<Alluvion>());
			recipe.AddIngredient(ItemType<AstrealDefeat>());
			recipe.AddIngredient(ItemType<ClockworkBow>());
			recipe.AddIngredient(ItemType<Galeforce>());
			recipe.AddIngredient(ItemType<PlanetaryAnnihilation>());
			recipe.AddIngredient(ItemType<TheBallista>());
			recipe.AddIngredient(ItemType<AuricBar>(), 4);
			recipe.AddTile(TileType<DraedonsForge>());
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
