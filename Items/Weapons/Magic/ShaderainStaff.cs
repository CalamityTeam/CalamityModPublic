using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Systems.Collections;
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
        public const int RainAmount = 2;
        public const float LesserRainVELMultiplier = 0.9f; // The lowest speed modifier the rain can get.
        public const float HigherRainVELMultiplier = 1.2f; // The highest speed modifier the rain can get.
        public const float GravityStrenght = 0.15f;

        // Stats for the shade clouds.
        public const int FadeoutSpeed = 2;
        public const float CloudDMGMultiplier = 1f; // Keeping this here in case it gets changed in the future
        public const float CloudVELMultiplier = 1.25f;
        public const float DeaccelerationStrenght = 0.95f; // A number lower than 1, non-including 1, changing it very slightly will have drastic results.

        #endregion
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<BrainRot>()];
        }
        public override void SetDefaults()
        {
            Item.width = Item.height = 42;
            Item.damage = 19;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 14;
            Item.useAnimation = Item.useTime = 34;
            Item.knockBack = 0.25f;
            Item.shoot = ModContent.ProjectileType<Shaderain>();
            Item.shootSpeed = 11f;

            Item.UseSound = SoundID.Item66;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.noMelee = true;

            Item.rare = ItemRarityID.Orange;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
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
    }
}
