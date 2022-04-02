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
        }

        public override void SetDefaults()
        {
            item.damage = 14;
            item.ranged = true;
            item.width = 62;
            item.height = 26;
            item.useTime = 26;
            item.useAnimation = 26;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 5.5f;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = ItemRarityID.Orange;
            item.UseSound = SoundID.Item61;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<Aquashard>();
            item.shootSpeed = 22f;
            item.useAmmo = AmmoID.Bullet;
            item.Calamity().canFirePointBlankShots = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 6;

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int num6 = Main.rand.Next(2, 4);
            for (int index = 0; index < num6; ++index)
            {
                float SpeedX = speedX + Main.rand.Next(-40, 41) * 0.05f;
                float SpeedY = speedY + Main.rand.Next(-40, 41) * 0.05f;

                if (type == ProjectileID.Bullet)
                {
                    int projectile = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<Aquashard>(), damage, knockBack, player.whoAmI);
                    Main.projectile[projectile].timeLeft = 200;
                }
                else
                    Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Boomstick);
            recipe.AddIngredient(ModContent.ItemType<AerialiteBar>(), 2);
            recipe.AddIngredient(ModContent.ItemType<SeaPrism>(), 10);
            recipe.AddIngredient(ModContent.ItemType<PrismShard>(), 15);
            recipe.AddIngredient(ModContent.ItemType<VictideBar>(), 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
