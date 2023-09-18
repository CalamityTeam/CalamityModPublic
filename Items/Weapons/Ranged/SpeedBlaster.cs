using Terraria.DataStructures;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Ranged;
using Terraria.Audio;
using CalamityMod.Projectiles.Magic;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Particles;

namespace CalamityMod.Items.Weapons.Ranged
{
    [LegacyName("PaintballBlaster")]
    public class SpeedBlaster : ModItem, ILocalizedModType
    {
        public static readonly SoundStyle Shot = new("CalamityMod/Sounds/Item/Splatshot") { PitchVariance = 0.3f, Volume = 3.5f };
        public static readonly SoundStyle Dash = new("CalamityMod/Sounds/Item/SplatshotDash") { PitchVariance = 0.3f, Volume = 4f };
        public static readonly SoundStyle ShotBig = new("CalamityMod/Sounds/Item/SplatshotBig") { PitchVariance = 0.3f, Volume = 3f };
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public int ColorValue = 0;
        public int UseTime = 10;
        public float ShotBigMode = 0;
        public override void SetDefaults()
        {
            Item.damage = 55;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 54;
            Item.height = 26;
            Item.useTime = Item.useAnimation = UseTime;
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
            Item.UseSound = player.altFunctionUse == 2 && player.Calamity().SpeedBlasterDashDelayCooldown == 0 ? ShotBig : Shot;
            return base.CanUseItem(player);
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-1, -6);
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.Calamity().SpeedBlasterDashDelayCooldown == 0)
            {
                Item.useTime = Item.useAnimation = 10;
                ShotBigMode = 0;
            }

            if (player.altFunctionUse == 2 && player.Calamity().SpeedBlasterDashDelayCooldown == 0)
            {
                ShotBigMode = 3;
                if (ColorValue >= 4)
                    ColorValue = 0;
                else
                    ColorValue++;

                Item.useTime = Item.useAnimation = 8;
                player.Calamity().SpeedBlasterDashDelayCooldown = 300;

                player.Calamity().sBlasterDashActivated = true;
            }
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Reposition to the gun's tip + add some inaccuracy
            Vector2 newPos = position + new Vector2(38f, player.direction * (Math.Abs(velocity.SafeNormalize(Vector2.Zero).X) < 0.02f ? -2f : -8f)).RotatedBy(velocity.ToRotation());
            Vector2 newVel = velocity.RotatedByRandom(MathHelper.ToRadians(ShotBigMode == 3 ? 0f : 4f));
            Projectile.NewProjectile(source, newPos, newVel, ModContent.ProjectileType<SpeedBlasterShot>(), damage * (ShotBigMode == 3 ? 5 : 1), knockback, player.whoAmI, ColorValue, ShotBigMode);
            if (player.Calamity().SpeedBlasterDashDelayCooldown > 0)
            {
                ShotBigMode = 2;
            }
            player.Calamity().ColorValue = ColorValue;
            if (player.Calamity().SpeedBlasterDashDelayCooldown == 300 && player.velocity != Vector2.Zero)
            {
                Color ColorUsed = (ColorValue == 0 ? Color.Aqua : ColorValue == 1 ? Color.Blue : ColorValue == 2 ? Color.Fuchsia : ColorValue == 3 ? Color.Lime : ColorValue == 4 ? Color.Yellow : Color.White);
                for (int i = 0; i <= 8; i++)
                {
                    CritSpark spark = new CritSpark(player.Center, player.velocity.RotatedByRandom(MathHelper.ToRadians(13f)) * Main.rand.NextFloat(-2.1f, -4.5f), Color.White, ColorUsed, 2f, 45, 2f, 2.5f);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.PainterPaintballGun).
                AddIngredient(ItemID.MythrilBar, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
