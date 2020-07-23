using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class PulseTurretRemote : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Pulse Turret Remote");
		}

		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 26;
			item.sentry = true;
			item.damage = 100;
			item.knockBack = 0f;
			item.useTime = item.useAnimation = 35;
			item.autoReuse = true;

			item.useStyle = ItemUseStyleID.HoldingUp;
			item.UseSound = SoundID.Item15;
			item.noMelee = true;

			item.value = CalamityGlobalItem.Rarity5BuyPrice;
			item.rare = ItemRarityID.Red;
			item.Calamity().customRarity = CalamityRarity.DraedonRust;

			item.shoot = ModContent.ProjectileType<PulseTurret>();
			item.shootSpeed = 1f;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Point mouseTileCoords = Main.MouseWorld.ToTileCoordinates();
			if (!CalamityUtils.ParanoidTileRetrieval(mouseTileCoords.X, mouseTileCoords.Y).active())
			{
				int existingTurrets = player.ownedProjectileCounts[type];
				if (existingTurrets > 0)
				{
					for (int i = 0; i < Main.projectile.Length || existingTurrets > 0; i++)
					{
						if (Main.projectile[i].type == type &&
							Main.projectile[i].owner == player.whoAmI &&
							Main.projectile[i].active)
						{
							Main.projectile[i].Kill();
							existingTurrets--;
						}
					}
				}
				Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, damage, knockBack, player.whoAmI);
			}
			return false;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 10);
			recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 10);
			recipe.AddIngredient(ItemID.HallowedBar, 5);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
