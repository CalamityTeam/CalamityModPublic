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
            Tooltip.SetDefault("33% chance to not consume ammo\n" +
                "Hold left click to load up to five pumpkin bombs for a 100% organic blast");
        }

        public override void SetDefaults()
        {
            item.damage = 8;
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
            item.UseSound = SoundID.Item11;
            item.autoReuse = true;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 11f;
            item.useAmmo = AmmoID.Bullet;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-30f, 0f);

        public override bool CanUseItem(Player player)
        {
            item.noUseGraphic = true;
            item.UseSound = SoundID.Item20;
            item.channel = true;
            return player.ownedProjectileCounts[ModContent.ProjectileType<CondemnationHoldout>()] <= 0;
        }

        public override float UseTimeMultiplier(Player player)
        {
            return 1f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {

            Vector2 shootVelocity = new Vector2(speedX, speedY);
            Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);
            // Charge-up. Done via a holdout projectile.
            Projectile.NewProjectile(position, shootDirection, ModContent.ProjectileType<PumplerHoldout>(), 0, 0f, player.whoAmI);
            return false;

        }

        public override bool ConsumeAmmo(Player player)
        {
            if (Main.rand.Next(0, 100) < 33)
                return false;
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Pumpkin, 30);
            recipe.AddIngredient(ItemID.PumpkinSeed, 5);
            recipe.AddIngredient(ItemID.IllegalGunParts);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

