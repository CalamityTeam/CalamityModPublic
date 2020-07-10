using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.DraedonsArsenal;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class GaussPistol : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gauss Pistol");
		}

		public override void SetDefaults()
		{
			item.width = 40;
			item.height = 22;
			item.magic = true;
			item.mana = 6;
			item.damage = 25;
			item.knockBack = 11f;
			item.useTime = item.useAnimation = 20;
			item.autoReuse = true;

			item.useStyle = ItemUseStyleID.HoldingOut;
			item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/GaussWeaponFire");
			item.noMelee = true;

			item.value = CalamityGlobalItem.Rarity3BuyPrice;
			item.rare = 3;

			item.shoot = ModContent.ProjectileType<GaussPistolShot>();
			item.shootSpeed = 14f;

			item.Calamity().Chargeable = true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 7);
			recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 5);
			recipe.AddIngredient(ModContent.ItemType<AerialiteBar>(), 10);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
