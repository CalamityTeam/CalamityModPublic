using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class ShaderainStaff : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        #region Other stats

        // Stats for the shaderain.
        public const int RainAmount = 3;
        public const float LesserRainVELMultiplier = 0.75f; // The lowest speed modifier the rain can get.
        public const float HigherRainVELMultiplier = 1.25f; // The highest speed modifier the rain can get.
        public const float GravityStrenght = 0.15f;

        // Stats for the shade clouds.
        public const int FadeoutSpeed = 2;
        public const float CloudDMGMultiplier = 1.25f;
        public const float CloudVELMultiplier = 1.25f;
        public const float DeaccelerationStrenght = 0.95f; // A number lower than 1, non-including 1, changing it very slightly will have drastic results.

        #endregion

        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 21;
            Item.shootSpeed = 10f;
            Item.useTime = Item.useAnimation = 25;
            Item.mana = 10;
            Item.knockBack = 0f;

            Item.shoot = ModContent.ProjectileType<Shaderain>();

            Item.width = Item.height = 42;
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item66;
            Item.rare = ItemRarityID.Orange;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.autoReuse = true;
            Item.noMelee = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Shoots the shaderain.
            for (int shadeRainIndex = 0; shadeRainIndex < RainAmount; shadeRainIndex++)
            {
                Projectile.NewProjectile(source,
                player.Center,
                velocity * Main.rand.NextFloat(LesserRainVELMultiplier, HigherRainVELMultiplier),
                type,
                damage,
                knockback,
                player.whoAmI);
            }

            // Shoots the small cloud.
            Projectile.NewProjectile(source,
                player.Center,
                velocity * CloudVELMultiplier,
                ModContent.ProjectileType<ShadeNimbusCloud>(),
                (int)(damage * CloudDMGMultiplier),
                knockback,
                player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.DemoniteBar, 3).
                AddIngredient<RottenMatter>(9).
                AddIngredient(ItemID.RottenChunk, 3).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
