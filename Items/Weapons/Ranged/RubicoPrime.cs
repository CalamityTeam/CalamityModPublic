using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class RubicoPrime : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rubico Prime");
            Tooltip.SetDefault("Semi-automatic sniper that fires in 5 second bursts\n" +
                "Fires impact rounds that have an increased crit multiplier and deal bonus damage to inorganic targets");
                //would do less to organic targets if like this wasn't meant to be used against yharon lole
        }

        public override void SetDefaults()
        {
            item.damage = 1178;
            item.ranged = true;
            item.knockBack = 10f;
            item.useTime = 30;
            item.useAnimation = 300;
            item.autoReuse = false;

            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.width = 82;
            item.height = 28;

            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 12f;
            item.useAmmo = AmmoID.Bullet;

            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.Calamity().donorItem = true;
			item.Calamity().canFirePointBlankShots = true;
		}

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 40;

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<PestilentDefiler>());
            recipe.AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 2);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<ImpactRound>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
