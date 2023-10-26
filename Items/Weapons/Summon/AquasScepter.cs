using CalamityMod.Projectiles.Summon;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework; 
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
	public class AquasScepter : ModItem, ILocalizedModType
	{
		public new string LocalizationCategory => "Items.Weapons.Summon";
		public override void SetStaticDefaults()
		{
			ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
			ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.damage = 65;
			Item.mana = 50;
			Item.DamageType = DamageClass.Summon;
			Item.sentry = true;
			Item.width = 56;
			Item.height = 56;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = 1;
			Item.knockBack = 6;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
			Item.Calamity().devItem = true;
            Item.UseSound = SoundID.Item66;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<AquasScepterCloud>();
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			position = Main.MouseWorld;
			var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
			projectile.originalDamage = Item.damage;
			player.UpdateMaxTurrets();
			return false;
		}

		public override void AddRecipes()
		{
			Recipe recipe = Recipe.Create(ModContent.ItemType<Items.Weapons.Summon.AquasScepter>());
			recipe.AddIngredient(ItemID.NimbusRod);
			recipe.AddIngredient(ItemID.AquaScepter);
            recipe.AddIngredient<ArmoredShell>(3);
            recipe.AddTile(TileID.LunarCraftingStation); //LunarCraftingStation = Ancient Manipulator
			recipe.Register();
		}
	}
}
