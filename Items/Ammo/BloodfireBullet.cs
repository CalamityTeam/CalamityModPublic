using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
	public class BloodfireBullet : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bloodfire Bullet");
			Tooltip.SetDefault("Accelerates your life regeneration on hit\n" + "Deals bonus damage based on your current life regeneration");
		}

		public override void SetDefaults()
		{
			item.damage = 26;
			item.ranged = true;
			item.width = 14;
			item.height = 30;
			item.maxStack = 999;
			item.consumable = true;
			item.knockBack = 4.5f;
			item.value = Item.sellPrice(copper: 24);
			item.rare = ItemRarityID.Purple;
			item.Calamity().customRarity = CalamityRarity.Turquoise;
			item.shoot = ModContent.ProjectileType<BloodfireBulletProj>();
			item.shootSpeed = 4.8f;
			item.ammo = ItemID.MusketBall;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<BloodstoneCore>());
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.SetResult(this, 333);
			recipe.AddRecipe();
		}
	}
}
