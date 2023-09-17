using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Ranged;
using Terraria.Audio;
using CalamityMod.Projectiles.Magic;

namespace CalamityMod.Items.Weapons.Ranged
{
    [LegacyName("PaintballBlaster")]
    public class SpeedBlaster : ModItem, ILocalizedModType
    {
        public static readonly SoundStyle Shot = new("CalamityMod/Sounds/Item/Splatshot") { PitchVariance = 0.3f, Volume = 2.0f };
        public static readonly SoundStyle Dash = new("CalamityMod/Sounds/Item/SplatshotDash") { PitchVariance = 0.3f, Volume = 2.0f };
        public static readonly SoundStyle ShotBig = new("CalamityMod/Sounds/Item/SplatshotBig") { PitchVariance = 0.3f, Volume = 2.0f };
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public int ColorValue = 0;
        public float ShotBigMode = 0;
        public override void SetDefaults()
        {
            Item.damage = 34;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 54;
            Item.height = 26;
            Item.useTime = Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.25f;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = Shot;
            Item.autoReuse = true;
            Item.shootSpeed = 20f;
            Item.shoot = ProjectileID.PainterPaintball;
            Item.Calamity().canFirePointBlankShots = true;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool CanUseItem(Player player)
        {
            Item.UseSound = player.altFunctionUse == 2 ? ShotBig : Shot;
            return base.CanUseItem(player);
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3, -6);
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                ShotBigMode = 3;
                if (ColorValue >= 4)
                    ColorValue = 0;
                else
                    ColorValue++;
            }
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Reposition to the gun's tip + add some inaccuracy
            Vector2 newPos = position + new Vector2(38f, player.direction * (Math.Abs(velocity.SafeNormalize(Vector2.Zero).X) < 0.02f ? -2f : -8f)).RotatedBy(velocity.ToRotation());
            Vector2 newVel = velocity.RotatedByRandom(MathHelper.ToRadians(ShotBigMode == 3 ? 0f : 4f));
            Projectile.NewProjectile(source, newPos, newVel, ModContent.ProjectileType<SpeedBlasterShot>(), damage, knockback, player.whoAmI, ColorValue, ShotBigMode);
            ShotBigMode = 0f;
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.PainterPaintballGun).
                AddIngredient(ItemID.SoulofSight, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
