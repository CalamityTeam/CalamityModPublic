using Terraria.DataStructures;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class AquashardShotgun : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aquashard Shotgun");
            Tooltip.SetDefault("Converts musket balls into aquashards that split upon hitting an enemy");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 14;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 62;
            Item.height = 26;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5.5f;
            Item.value = Item.buyPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item61;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Aquashard>();
            Item.shootSpeed = 22f;
            Item.useAmmo = AmmoID.Bullet;
            Item.Calamity().canFirePointBlankShots = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 6;

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int num6 = Main.rand.Next(2, 4);
            for (int index = 0; index < num6; ++index)
            {
                float SpeedX = velocity.X + Main.rand.Next(-40, 41) * 0.05f;
                float SpeedY = velocity.Y + Main.rand.Next(-40, 41) * 0.05f;

                if (type == ProjectileID.Bullet)
                {
                    int projectile = Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<Aquashard>(), damage, knockback, player.whoAmI);
                    Main.projectile[projectile].timeLeft = 200;
                }
                else
                    Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, damage, knockback, player.whoAmI);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Boomstick).
                AddIngredient<AerialiteBar>(2).
                AddIngredient<SeaPrism>(10).
                AddIngredient<PrismShard>(15).
                AddIngredient<SeaRemains>(5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
