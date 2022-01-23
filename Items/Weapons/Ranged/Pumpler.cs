using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Ranged;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Pumpler : ModItem
    {

        public const int MaxPumpkins = 5;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pumpler");
            Tooltip.SetDefault("Hold left click to load up to five pumpkin grenades into the gun");
        }

        public override void SetDefaults()
        {
            item.damage = 27;
            item.ranged = true;
            item.width = 72;
            item.height = 34;
            item.useTime = 60;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 1.25f;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = ItemRarityID.Green;
            item.noUseGraphic = true;
            item.UseSound = SoundID.Item20;
            item.autoReuse = true;
            item.channel = true;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 11f;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-30f, 0f);

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<PumplerHoldout>()] <= 0;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {

            Vector2 shootVelocity = new Vector2(speedX, speedY);
            Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);
            // Charge-up. Done via a holdout projectile.
            Projectile.NewProjectile(position, shootDirection, ModContent.ProjectileType<PumplerHoldout>(), 0, 0f, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.IllegalGunParts);
            recipe.AddIngredient(ItemID.Pumpkin, 30);
            recipe.AddIngredient(ItemID.PumpkinSeed, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

