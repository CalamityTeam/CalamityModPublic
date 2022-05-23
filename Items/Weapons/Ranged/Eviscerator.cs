using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Eviscerator : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eviscerator");
            Tooltip.SetDefault("Converts musket balls into slow-moving blood clots");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 65;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 58;
            Item.height = 22;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7.5f;
            Item.value = Item.buyPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item40;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BloodClotFriendly>();
            Item.shootSpeed = 22f;
            Item.useAmmo = AmmoID.Bullet;
            Item.Calamity().canFirePointBlankShots = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 25;

        public override Vector2? HoldoutOffset() => new Vector2(-7, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (type == ProjectileID.Bullet)
                Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<BloodClotFriendly>(), damage, knockback, player.whoAmI);
            else
                Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BloodSample>(8).
                AddIngredient(ItemID.Vertebrae, 4).
                AddIngredient(ItemID.CrimtaneBar, 4).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
