using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
	public class GatlingLaser : ModItem
	{
		// This is the amount of charge consumed every time the holdout projectile fires a laser.
		public const float HoldoutChargeUse = 0.0075f;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gatling Laser");
			Tooltip.SetDefault("Large laser cannon used primarily by Yharim's fleet and base defense force");
		}

		public override void SetDefaults()
		{
			CalamityGlobalItem modItem = item.Calamity();

			item.width = 43;
			item.height = 24;
			item.magic = true;
			item.damage = 43;
			item.knockBack = 1f;
			item.useTime = 2;
			item.useAnimation = 2;
			item.noUseGraphic = true;
			item.autoReuse = false;
			item.channel = true;
			item.mana = 6;

			item.useStyle = ItemUseStyleID.HoldingOut;
			item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/GatlingLaserFireStart");
			item.noMelee = true;

			item.value = CalamityGlobalItem.Rarity8BuyPrice;
			item.rare = ItemRarityID.Red;
			modItem.customRarity = CalamityRarity.DraedonRust;

			item.shoot = ModContent.ProjectileType<GatlingLaserProj>();
			item.shootSpeed = 24f;

			modItem.UsesCharge = true;
			modItem.MaxCharge = 135f;
			modItem.ChargePerUse = 0f; // This weapon is a holdout. Charge is consumed by the holdout projectile.
		}

		public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

		public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 3);

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<GatlingLaserProj>(), damage, knockBack, player.whoAmI, 0f, 0f);
			return false;
		}

		public override Vector2? HoldoutOffset() => new Vector2(-20, 0);

		public override void AddRecipes()
		{
			ArsenalTierGatedRecipe recipe = new ArsenalTierGatedRecipe(mod, 3);
			recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 15);
			recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 15);
			recipe.AddIngredient(ModContent.ItemType<BarofLife>(), 5);
			recipe.AddIngredient(ModContent.ItemType<InfectedArmorPlating>(), 5);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
