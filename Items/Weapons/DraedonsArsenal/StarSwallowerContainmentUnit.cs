using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class StarSwallowerContainmentUnit : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Star Swallower Containment Unit");
			Tooltip.SetDefault("Summons a mechanical frog that vomits plasma onto enemies");
		}

		public override void SetDefaults()
		{
			item.shootSpeed = 10f;
			item.damage = 20;
			item.mana = 10;
			item.width = 18;
			item.height = 28;
			item.useTime = item.useAnimation = 30;
			item.useStyle = ItemUseStyleID.HoldingUp;
			item.noMelee = true;
			item.knockBack = 2.25f;
			item.value = CalamityGlobalItem.Rarity3BuyPrice;
			item.rare = ItemRarityID.Red;
			item.Calamity().customRarity = CalamityRarity.DraedonRust;
			item.UseSound = SoundID.Item15;
			item.autoReuse = true;
			item.shoot = ModContent.ProjectileType<StarSwallowerSummon>();
			item.shootSpeed = 10f;
			item.summon = true;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Point mouseTileCoords = Main.MouseWorld.ToTileCoordinates();
			if (!CalamityUtils.ParanoidTileRetrieval(mouseTileCoords.X, mouseTileCoords.Y).active())
			{
				Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, damage, knockBack, player.whoAmI);
			}
			return false;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 6);
			recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 6);
			recipe.AddIngredient(ModContent.ItemType<AerialiteBar>(), 8);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
