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
	public class LaserRifle : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Heavy Laser Rifle");
			Tooltip.SetDefault("Laser weapon used by heavy infantry units in Yharim's army");
		}

		public override void SetDefaults()
		{
			CalamityGlobalItem modItem = item.Calamity();

			item.width = 84;
			item.height = 28;
			item.ranged = true;
			item.damage = 185;
			item.knockBack = 4f;
			item.useTime = 25;
			item.useAnimation = 25;
			item.autoReuse = true;

			item.useStyle = ItemUseStyleID.HoldingOut;
			item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserRifleFire");
			item.noMelee = true;

			item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
			item.rare = ItemRarityID.Red;
			modItem.customRarity = CalamityRarity.DraedonRust;

			item.shoot = ModContent.ProjectileType<LaserRifleShot>();
			item.shootSpeed = 5f;

			modItem.UsesCharge = true;
			modItem.MaxCharge = 190f;
			modItem.ChargePerUse = 0.125f;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Vector2 velocity = new Vector2(speedX, speedY);
			if (velocity.Length() > 5f)
			{
				velocity.Normalize();
				velocity *= 5f;
			}
			for (int i = 0; i < 2; i++)
			{
				float SpeedX = velocity.X + Main.rand.Next(-1, 2) * 0.05f;
				float SpeedY = velocity.Y + Main.rand.Next(-1, 2) * 0.05f;
				Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<LaserRifleShot>(), damage, knockBack, player.whoAmI, i, 0f);
			}
			return false;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 4);

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-20, 0);
		}

		public override void AddRecipes()
		{
			ArsenalTierGatedRecipe recipe = new ArsenalTierGatedRecipe(mod, 4);
			recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 15);
			recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 15);
			recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 8);
			recipe.AddIngredient(ItemID.LunarBar, 4);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
