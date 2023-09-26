using CalamityMod.CalPlayer;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    [LegacyName("PaintballBlaster")]
    public class SpeedBlaster : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public static readonly SoundStyle Shot = new("CalamityMod/Sounds/Item/Splatshot") { PitchVariance = 0.3f, Volume = 3.5f };
        public static readonly SoundStyle Dash = new("CalamityMod/Sounds/Item/SplatshotDash") { PitchVariance = 0.3f, Volume = 5f };
        public static readonly SoundStyle ShotBig = new("CalamityMod/Sounds/Item/SplatshotBig") { PitchVariance = 0.3f, Volume = 2f };
        public static readonly SoundStyle Empty = new("CalamityMod/Sounds/Item/DudFire") { PitchVariance = 0.3f, Volume = 0.7f };

        public static float DashShotDamageMult = 4.5f;
        public static int DashCooldown = 300; // Also dictates how long shots are boosted (frames)
        public static float FireRatePowerup = 1.15f; // Attack speed multiplier after dashing

        public float ColorValue = 0f; // What color is going to be shot - see projectile file for color hexes

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DashCooldown / 60);

        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 54;
            Item.height = 26;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.25f;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = Shot;
            Item.autoReuse = true;
            Item.shootSpeed = 20f;
            Item.shoot = ModContent.ProjectileType<SpeedBlasterShot>();
            Item.Calamity().canFirePointBlankShots = true;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool CanUseItem(Player player)
        {
            if (player.Calamity().SpeedBlasterDashDelayCooldown > 0 && player.altFunctionUse == 2)
            {
                SoundEngine.PlaySound(Empty, player.Center);
                return false;
            }
            Item.UseSound = player.altFunctionUse == 2 ? ShotBig : Shot;
            return base.CanUseItem(player);
        }

        public override Vector2? HoldoutOffset() => new Vector2(-1, -6);

        // Increased fire rate after dashing
        public override float UseSpeedMultiplier(Player player) => player.Calamity().SpeedBlasterDashDelayCooldown > 0 ? FireRatePowerup : 1f;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Reposition all shots to the gun's tip
            Vector2 newPos = position + new Vector2(38f, player.direction * (Math.Abs(velocity.SafeNormalize(Vector2.Zero).X) < 0.02f ? -2f : -8f)).RotatedBy(velocity.ToRotation());

            // Big dashing shot
            if (player.altFunctionUse == 2 && player.Calamity().SpeedBlasterDashDelayCooldown == 0)
            {
                // Switches the color of the next shots, including this one
                if (ColorValue >= 4f)
                    ColorValue = 0f;
                else
                    ColorValue++;

                Projectile.NewProjectile(source, newPos, velocity, type, (int)(damage * DashShotDamageMult), knockback, player.whoAmI, ColorValue, 3f);

                // Activate the dash and cooldown
                player.Calamity().SpeedBlasterDashDelayCooldown = DashCooldown;
                player.Calamity().sBlasterDashActivated = true;

                // If moving, emit particles to signal the dash
                if (player.velocity != Vector2.Zero)
                {
                    Color ColorUsed = SpeedBlasterShot.GetColor(ColorValue);
                    for (int i = 0; i <= 8; i++)
                    {
                        CritSpark spark = new CritSpark(player.Center, player.velocity.RotatedByRandom(MathHelper.ToRadians(13f)) * Main.rand.NextFloat(-2.1f, -4.5f), Color.White, ColorUsed, 2f, 45, 2f, 2.5f);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                }
                return false;
            }

            // Add inaccuracy to regular shots; powered up shots are more accurate
            Vector2 newVel = velocity.RotatedByRandom(MathHelper.ToRadians(player.Calamity().SpeedBlasterDashDelayCooldown > 0 ? 3f : 15f));
            float ShotMode = player.Calamity().SpeedBlasterDashDelayCooldown > 0 ? 2f : 0f;
            Projectile.NewProjectile(source, newPos, newVel, type, damage, knockback, player.whoAmI, ColorValue, ShotMode);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.PainterPaintballGun).
                AddRecipeGroup("AnyMythrilBar", 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
