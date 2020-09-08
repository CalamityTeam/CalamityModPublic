using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.DraedonsArsenal;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
	public class PulseRifle : ModItem
	{
		private int BaseDamage = 6560;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Pulse Rifle");
			Tooltip.SetDefault("Draedon's former pulse rifle, used in emergencies for creations which turned against him.\n" +
				"When the pulse hits a target it will arc to another nearby target\n" +
				"Inflicts exceptional damage against inorganic targets");
		}

		public override void SetDefaults()
		{
			CalamityGlobalItem modItem = item.Calamity();

			item.width = 62;
			item.height = 22;
			item.ranged = true;
			item.damage = BaseDamage;
			item.knockBack = 0f;
			item.useTime = 35;
			item.useAnimation = 35;
			item.autoReuse = true;

			item.useStyle = ItemUseStyleID.HoldingOut;
			item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PulseRifleFire");
			item.noMelee = true;

			item.value = CalamityGlobalItem.RarityVioletBuyPrice;
			item.rare = ItemRarityID.Red;
			modItem.customRarity = CalamityRarity.DraedonRust;

			item.shoot = ModContent.ProjectileType<PulseRifleShot>();
			item.shootSpeed = 5f;

			modItem.UsesCharge = true;
			modItem.MaxCharge = 250f;
			modItem.ChargePerUse = 0.3f;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Vector2 velocity = new Vector2(speedX, speedY);
			if (velocity.Length() > 5f)
			{
				velocity.Normalize();
				velocity *= 5f;
			}

			float SpeedX = velocity.X + (float)Main.rand.Next(-1, 2) * 0.05f;
			float SpeedY = velocity.Y + (float)Main.rand.Next(-1, 2) * 0.05f;

			Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<PulseRifleShot>(), damage, knockBack, player.whoAmI, 0f, 0f);
			return false;
		}

		public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 20);
			recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 20);
			recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 4);
			recipe.AddTile(ModContent.TileType<DraedonsForge>());
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
