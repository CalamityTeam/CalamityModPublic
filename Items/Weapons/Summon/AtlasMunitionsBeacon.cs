using Terraria.DataStructures;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class AtlasMunitionsBeacon : ModItem
    {
        public const float TargetRange = 2400f;

        public const float OverdriveModeRange = 720f;

        public const float PickupRange = 200f;

        public const int TurretShootRate = 9;

        public const int HeldCannonShootRate = 9;

        public const int HeldCannonFadeoutTime = 156;

        // How long the held cannon can exist on the ground before it starts to disappear.
        public const int HeldCannonMaxDropTime = 720;

        public const float OverdriveProjectileDamageFactor = 0.6f;

        // This shouldn't be too high, or it'll make using it as a held item a pain.
        public const int ShotsNeededToReachMaxHeat = 100;

        // How long it takes for the cannon to fully cool off from the maximum heat value.
        public const int HeatDissipationTime = 180;
        
        // This shouldn't be too high. If it is, the overdrive mode will be frustratingly inconsistent to use.
        public const float OverdriveProjectileAngularRandomness = 0.13f;

        public static readonly Color HeatGlowColor = Color.OrangeRed with { A = 64 };

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Atlas Munitions Beacon");
            Tooltip.SetDefault("Surprisingly easy to pick up");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 640;
            Item.mana = 10;
            Item.width = 40;
            Item.height = 38;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 4.75f;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item82;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AtlasMunitionsDropPod>();
            Item.shootSpeed = 10f;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Summon;
            Item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<AtlasMunitionsAutocannonHeld>()] >= 1)
                return false;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<AtlasMunitionsDropPod>()] >= 1)
                return false;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<AtlasMunitionsAutocannon>()] >= 1)
                return false;
            return true;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Summon/AtlasMunitionsBeaconGlow").Value);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                position = Main.MouseWorld - Vector2.UnitY.RotatedByRandom(0.47f) * 1020f;
                velocity = (Main.MouseWorld - position).SafeNormalize(Vector2.UnitY) * Main.rand.NextFloat(9f, 10f);
                int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, Main.MouseWorld.Y - 40f);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = Item.damage;
            }
            return false;
        }
    }
}
