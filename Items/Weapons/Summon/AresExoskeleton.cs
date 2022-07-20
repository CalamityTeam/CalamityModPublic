using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Summon.SmallAresArms;

namespace CalamityMod.Items.Weapons.Summon
{
    public class AresExoskeleton : ModItem
    {
        public const int PlasmaCannonShootRate = 30;

        public const int TeslaCannonShootRate = 30;

        public const int LaserCannonNormalShootRate = 30;

        public const int GaussNukeShootRate = 210;

        public const float TargetingDistance = 1020f;

        public const float MinionSlotsPerCannon = 3f;

        public const float NukeDamageFactor = 2.7f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ares' Exoskeleton");
            Tooltip.SetDefault("Ares arms. STRONG cannons. BIG explosions. FUN");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.mana = 80;
            Item.damage = 972;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noUseGraphic = true;
            Item.width = Item.height = 56;
            Item.useTime = Item.useAnimation = 9;
            Item.noMelee = true;
            Item.knockBack = 1f;

            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.Calamity().customRarity = CalamityRarity.Violet;

            Item.UseSound = SoundID.Item117;
            Item.shoot = ModContent.ProjectileType<ExoskeletonPlasmaCannon>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
            Item.Calamity().CannotBeEnchanted = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int? projectileToSummon = null;
            int plasmaCannonID = ModContent.ProjectileType<ExoskeletonPlasmaCannon>();
            int teslaCannonID = ModContent.ProjectileType<ExoskeletonTeslaCannon>();
            int laserCannonID = ModContent.ProjectileType<ExoskeletonLaserCannon>();
            int gaussNukeID = ModContent.ProjectileType<ExoskeletonGaussNukeCannon>();

            if (player.ownedProjectileCounts[plasmaCannonID] <= 0)
                projectileToSummon = plasmaCannonID;
            else if (player.ownedProjectileCounts[teslaCannonID] <= 0)
                projectileToSummon = teslaCannonID;
            else if (player.ownedProjectileCounts[laserCannonID] <= 0)
                projectileToSummon = laserCannonID;
            else if (player.ownedProjectileCounts[gaussNukeID] <= 0)
                projectileToSummon = gaussNukeID;

            // Don't fire anything if no cannon type was selected.
            if (!projectileToSummon.HasValue)
                return false;

            int cannon = Projectile.NewProjectile(source, position, Vector2.Zero, projectileToSummon.Value, damage, knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(cannon))
                Main.projectile[cannon].originalDamage = Item.damage;

            return false;
        }
    }
}
