using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.DraedonsArsenal;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
	public class GaussPistol : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gauss Pistol");
			Tooltip.SetDefault("A simple pistol that utilizes magic power; a weapon for the more magically adept\n" +
			"Fires a devastating high velocity blast with extreme knockback");
		}

		public override void SetDefaults()
		{
			CalamityGlobalItem modItem = item.Calamity();

			item.width = 40;
			item.height = 22;
			item.magic = true;
			item.mana = 6;
			item.damage = 110;
			item.knockBack = 11f;
			item.useTime = item.useAnimation = 20;
			item.autoReuse = true;

			item.useStyle = ItemUseStyleID.HoldingOut;
			item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/GaussWeaponFire");
			item.noMelee = true;

			item.value = CalamityGlobalItem.Rarity5BuyPrice;
			item.rare = ItemRarityID.Red;
			modItem.customRarity = CalamityRarity.DraedonRust;

			item.shoot = ModContent.ProjectileType<GaussPistolShot>();
			item.shootSpeed = 14f;

			modItem.MaxCharge = 85f;
			modItem.UsesCharge = true;
			modItem.ChargePerUse = 0.05f;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips) => CalamityGlobalItem.InsertKnowledgeTooltip(tooltips, 2);

		public override void AddRecipes()
		{
			ArsenalTierGatedRecipe recipe = new ArsenalTierGatedRecipe(mod, 2);
			recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 12);
			recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 8);
			recipe.AddRecipeGroup("AnyMythrilBar", 10);
            recipe.AddIngredient(ItemID.SoulofMight, 20);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
