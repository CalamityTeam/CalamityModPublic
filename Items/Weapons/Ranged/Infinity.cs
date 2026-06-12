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
    public class Infinity : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public int SineCounter = 0;

        public static int ArmorPenetration = 20;
        public static int AmmoSavedPercent = 80;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ArmorPenetration, AmmoSavedPercent);

        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 24;
            Item.damage = 75;
            Item.ArmorPenetration = ArmorPenetration;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 1;
            Item.useAnimation = 4;
            Item.useLimitPerAnimation = 4;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1f;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.UseSound = SoundID.Item31;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 6f;
            Item.useAmmo = AmmoID.Bullet;
            Item.consumeAmmoOnLastShotOnly = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
            position = position + (player.Calamity().mouseWorld - player.MountedCenter).SafeNormalize(Vector2.UnitX) * 65;
            float sine = (float)Math.Sin(SineCounter * 0.175f / MathHelper.Pi) * 3.5f;
            SineCounter++;
            if (SineCounter % 4 == 0)
            {
                for (int i = -1; i <= 1; i += 2)
                {
                    Vector2 helixVel = (velocity * Main.rand.NextFloat(0.9f, 1.1f)).RotatedBy(MathHelper.ToRadians(i * sine));
                    Projectile.NewProjectile(source, position, helixVel, ModContent.ProjectileType<ChargedBlast>(), damage, knockback, player.whoAmI, 0, 0, 1);
                }
            }
            Particle spark2 = new LineParticle(position + Main.rand.NextVector2Circular(6, 6), (velocity * 4).RotatedByRandom(0.2f) * Main.rand.NextFloat(0.8f, 1.2f), false, Main.rand.Next(15, 25 + 1), Main.rand.NextFloat(1.5f, 2f), new Color(229, 49, 39));
            GeneralParticleHandler.SpawnParticle(spark2);
            return false;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.Next(100) >= AmmoSavedPercent;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Shredder>().
                AddIngredient<CosmiliteBar>(8).
                AddIngredient<NightmareFuel>(20).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
