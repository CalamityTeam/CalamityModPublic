using System;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Svantechnical : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public int SineCounter = 0;

        public static int ArmorPenetration = 200;
        public static int AmmoSavedPercent = 80;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ArmorPenetration, AmmoSavedPercent);

        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 26;
            Item.damage = 232;
            Item.ArmorPenetration = ArmorPenetration;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 1;
            Item.useAnimation = 4;
            Item.useLimitPerAnimation = 4;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.5f;

            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;

            Item.UseSound = SoundID.Item31;
            Item.autoReuse = true;
            Item.shootSpeed = 6f;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.useAmmo = AmmoID.Bullet;
            Item.consumeAmmoOnLastShotOnly = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
            position = position + (player.Calamity().mouseWorld - player.MountedCenter).SafeNormalize(Vector2.UnitX) * 65;
            float sine = (float)Math.Sin(SineCounter * 0.175f / MathHelper.Pi) * 4f;
            float sine2 = (float)Math.Sin(SineCounter * 0.275f / MathHelper.Pi) * 2f;
            SineCounter++;
            if (SineCounter % 4 == 0)
            {
                Vector2 helixVel1 = (velocity * Main.rand.NextFloat(0.9f, 1.1f)).RotatedBy(MathHelper.ToRadians(sine));
                Vector2 helixVel2 = (velocity * Main.rand.NextFloat(0.9f, 1.1f)).RotatedBy(MathHelper.ToRadians(-sine));
                Vector2 helixVel3 = (velocity * Main.rand.NextFloat(0.9f, 1.1f)).RotatedBy(MathHelper.ToRadians(sine2));
                Projectile.NewProjectile(source, position, helixVel1, ModContent.ProjectileType<ChargedBlast>(), damage, knockback, player.whoAmI, 0f, 0, 2f);
                Projectile.NewProjectile(source, position, helixVel2, ModContent.ProjectileType<ChargedBlast>(), damage, knockback, player.whoAmI, 0f, 0, 4f);
                Projectile.NewProjectile(source, position, helixVel3, ModContent.ProjectileType<ChargedBlast>(), damage, knockback, player.whoAmI, 0f, 0, 3f);
            }
            Particle spark2 = new LineParticle(position + Main.rand.NextVector2Circular(6, 6), (velocity * 4).RotatedByRandom(0.35f) * Main.rand.NextFloat(0.8f, 1.2f), false, Main.rand.Next(15, 25 + 1), Main.rand.NextFloat(1.5f, 2f), Main.rand.NextBool() ? Color.MediumOrchid : Color.DarkViolet);
            GeneralParticleHandler.SpawnParticle(spark2);
            return false;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.Next(100) >= AmmoSavedPercent;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Infinity>().
                AddIngredient<ShadowspecBar>(5).
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
