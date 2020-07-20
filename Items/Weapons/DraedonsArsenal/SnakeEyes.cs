using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class SnakeEyes : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Snake Eyes");
			Tooltip.SetDefault("Summons a mechanical watcher that zaps and flies around enemies.");
		}

		public override void SetDefaults()
		{
			item.shootSpeed = 10f;
			item.damage = 9;
			item.mana = 12;
			item.width = 38;
			item.height = 24;
			item.useTime = item.useAnimation = 30;
			item.useStyle = ItemUseStyleID.HoldingUp;
			item.noMelee = true;
			item.knockBack = 3f;
			item.value = CalamityGlobalItem.Rarity3BuyPrice;
			item.rare = ItemRarityID.Orange;
			item.UseSound = SoundID.Item15;
			item.autoReuse = true;
			item.shoot = ModContent.ProjectileType<SnakeEyesSummon>();
			item.shootSpeed = 10f;
			item.summon = true;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, damage, knockBack, player.whoAmI);
			return false;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 7);
			recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 7);
			recipe.AddIngredient(ModContent.ItemType<AerialiteBar>(), 7);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
