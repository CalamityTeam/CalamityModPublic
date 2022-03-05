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
	public class MatterModulator : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Matter Modulator");
			Tooltip.SetDefault("Using extra mass gained from collision with solid materials, it causes extra damage\n" +
			"Fires a burst of unstable matter that does significant damage after striking a tile\n" +
			"Before striking a tile, the matter pierces infinitely but deals little damage");
		}

		public override void SetDefaults()
		{
			CalamityGlobalItem modItem = item.Calamity();

			item.width = 40;
			item.height = 22;
			item.ranged = true;
			item.damage = 84;
			item.knockBack = 11f;
			item.useTime = item.useAnimation = 33;
			item.autoReuse = true;

			item.useStyle = ItemUseStyleID.HoldingOut;
			item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PlasmaBolt");
			item.noMelee = true;

			item.value = CalamityGlobalItem.Rarity5BuyPrice;
			item.rare = ItemRarityID.Red;
			modItem.customRarity = CalamityRarity.DraedonRust;

			item.shoot = ModContent.ProjectileType<UnstableMatter>();
			item.shootSpeed = 7f;

			modItem.UsesCharge = true;
			modItem.MaxCharge = 85f;
			modItem.ChargePerUse = 0.075f;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			for (int i = 0; i < Main.rand.Next(3, 5 + 1); i++)
			{
				Projectile.NewProjectile(position, new Vector2(speedX, speedY).RotatedByRandom(0.4f) * Main.rand.NextFloat(0.8f, 1.3f), type, damage, knockBack, player.whoAmI);
			}
			return false;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 2);

		public override void AddRecipes()
		{
			ArsenalTierGatedRecipe recipe = new ArsenalTierGatedRecipe(mod, 2);
			recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 12);
			recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 8);
			recipe.AddRecipeGroup("AnyMythrilBar", 10);
            recipe.AddIngredient(ItemID.SoulofFright, 20);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
