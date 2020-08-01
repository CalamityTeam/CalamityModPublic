using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.DraedonsArsenal;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
	public class Phaseslayer : ModItem
	{
		public const int Damage = 10200;
		// When below this percentage of charge, the sword is small instead of big.
		public const float SizeChargeThreshold = 0.33f;
		public const float SmallDamageMultiplier = 0.66f;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Phaseslayer");
			Tooltip.SetDefault("A rough prototype of the Murasama blade, it is formed entirely from laser energy.\n" +
							   "Wield a colossal laser blade which is controlled by the cursor\n" +
							   "Faster swings deal more damage and release sword beams\n" +
							   "When at low charge, the blade is smaller and weaker\n" +
							   "Deals less damage against enemies with high defense");
		}
		public override void SetDefaults()
		{
			item.damage = Damage;
			item.melee = true;
			item.width = 26;
			item.height = 26;
			item.useTime = 24;
			item.useAnimation = 24;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.useTurn = false;
			item.knockBack = 7f;

			item.noUseGraphic = true;

			item.value = CalamityGlobalItem.RarityVioletBuyPrice;
			item.rare = ItemRarityID.Red;
			item.Calamity().customRarity = CalamityRarity.DraedonRust;

			item.UseSound = SoundID.Item1;
			item.autoReuse = true;

			item.Calamity().Chargeable = true;
			item.Calamity().ChargeMax = 250;

			item.shoot = ModContent.ProjectileType<PhaseslayerProjectile>();
			item.channel = true;
		}

		public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0 && item.Calamity().CurrentCharge > 0;

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Projectile blade = Projectile.NewProjectileDirect(position, Vector2.Zero, type, damage, knockBack, player.whoAmI);
			blade.rotation = blade.AngleTo(Main.MouseWorld);
			return false;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 15);
			recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 25);
			recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 4);
			recipe.AddTile(ModContent.TileType<DraedonsForge>());
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
