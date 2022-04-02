using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
	public class RemsRevenge : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rem's Revenge");
			Tooltip.SetDefault("Wielded by the most powerful fighter.\n" +
			"Summons blood explosions and lowers enemy defense on hit");
		}

		public override void SetDefaults()
		{
			item.damage = 375;
			item.melee = true;
			item.width = 44;
			item.height = 34;
			item.useTime = 15;
			item.useAnimation = 15;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.knockBack = 10f;
			item.value = CalamityGlobalItem.Rarity10BuyPrice;
			item.rare = ItemRarityID.Red;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.channel = true;
			item.shoot = ModContent.ProjectileType<RemsRevengeProj>();
			item.shootSpeed = 12f;
			item.Calamity().donorItem = true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.BlueMoon);
			recipe.AddIngredient(ItemID.LunarBar, 5);
			recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 10);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
