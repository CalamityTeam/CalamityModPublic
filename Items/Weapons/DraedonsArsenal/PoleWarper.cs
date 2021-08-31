using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.DraedonsArsenal;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
	public class PoleWarper : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Pole Warper");
			Tooltip.SetDefault("Magnetic devices which tear at foes by propelling themselves off their opposite counterparts. Incredibly dangerous.\n" +
			"Summons a pair of floating magnets that repel each other and relentlessly swarm enemies");
		}

		public override void SetDefaults()
		{
			CalamityGlobalItem modItem = item.Calamity();

			item.shootSpeed = 10f;
			item.damage = 248;
			item.mana = 12;
			item.width = 38;
			item.height = 24;
			item.useTime = item.useAnimation = 10;
			item.useStyle = ItemUseStyleID.HoldingUp;
			item.noMelee = true;
			item.knockBack = 8f;

			item.value = CalamityGlobalItem.Rarity14BuyPrice;
			item.rare = ItemRarityID.Purple;
			modItem.customRarity = CalamityRarity.DraedonRust;

			item.UseSound = SoundID.Item15;
			item.autoReuse = true;
			item.shoot = ModContent.ProjectileType<PoleWarperSummon>();
			item.shootSpeed = 10f;
			item.summon = true;

			modItem.UsesCharge = true;
			modItem.MaxCharge = 250f;
			modItem.ChargePerUse = 1.25f;
			modItem.ChargePerAltUse = 0f;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Projectile north = Projectile.NewProjectileDirect(Main.MouseWorld + Vector2.UnitY * 30f, Vector2.Zero, type, damage, knockBack, player.whoAmI);
			Projectile south = Projectile.NewProjectileDirect(Main.MouseWorld - Vector2.UnitY * 30f, Vector2.Zero, type, damage, knockBack, player.whoAmI);
			north.ai[1] = 1f;
			south.ai[1] = 0f;

			float magnetCount = 0f;

			for (int i = 0; i < Main.projectile.Length; i++)
			{
				if (Main.projectile[i].type == type && Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI)
				{
					magnetCount++;
				}
			}

			// Adjust the offset of all existing magnets such that they form a psuedo-circle.
			// This offset is used when determining where a magnet should move to relative to its true destination (such as the player or an enemy).
			int magnetIndex = 0;
			for (int i = 0; i < Main.projectile.Length; i++)
			{
				if (Main.projectile[i].type == type && Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI)
				{
					((PoleWarperSummon)Main.projectile[i].modProjectile).Time = 0f;
					((PoleWarperSummon)Main.projectile[i].modProjectile).AngularOffset = MathHelper.TwoPi * magnetIndex / magnetCount;
					magnetIndex++;
				}
			}

			return false;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 25);
			recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 15);
			recipe.AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 2);
			recipe.AddTile(ModContent.TileType<DraedonsForge>());
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
