using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    public class SurgeDriverHoldout : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];
        public bool OwnerCanShoot => Owner.channel && Owner.HasAmmo(Owner.ActiveItem(), true) && !Owner.noItems && !Owner.CCed;
        public ref float ShootCountdown => ref Projectile.ai[0];
        public override void SetStaticDefaults() => DisplayName.SetDefault("Surge Driver");

        public override void SetDefaults()
        {
            Projectile.width = 192;
            Projectile.height = 52;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (!OwnerCanShoot)
                Projectile.Kill();

            Vector2 armPosition = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            armPosition += Projectile.velocity.SafeNormalize(Owner.direction * Vector2.UnitX) * 32f;
            armPosition.Y -= 12f;

            UpdateProjectileHeldVariables(armPosition);
            ManipulatePlayerVariables();

            ShootCountdown--;
            if (ShootCountdown <= 0f)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    ShootProjectiles(armPosition);
                    ShootCountdown = 28f;
                    Projectile.netUpdate = true;
                }

                SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Item/LaserCannon"), Projectile.Center);
            }
        }

        public void ShootProjectiles(Vector2 armPosition)
        {
            if (Main.myPlayer != Projectile.owner)
                return;

            Item heldItem = Owner.ActiveItem();
            int projectileType = ModContent.ProjectileType<PrismaticEnergyBlast>();
            float shootSpeed = heldItem.shootSpeed * Projectile.scale * 0.64f;
            int damage = (int)(Owner.GetWeaponDamage(heldItem) * 6.66);
            float knockback = heldItem.knockBack;

            bool uselessFuckYou = OwnerCanShoot;
            Owner.PickAmmo(heldItem, ref projectileType, ref shootSpeed, ref uselessFuckYou, ref damage, ref knockback, out _);
            projectileType = ModContent.ProjectileType<PrismaticEnergyBlast>();

            knockback = Owner.GetWeaponKnockback(heldItem, knockback);
            Vector2 shootDirection = (Main.MouseWorld - Projectile.Center).SafeNormalize(-Vector2.UnitY);
            Vector2 shootVelocity = shootDirection * shootSpeed;

            // Sync if the shoot direction changes.
            if (shootDirection.X != Projectile.velocity.X || shootDirection.Y != Projectile.velocity.Y)
                Projectile.netUpdate = true;

            Vector2 gunTip = armPosition + shootDirection * heldItem.scale * 130f;
            Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), gunTip, shootVelocity, projectileType, damage, knockback, Projectile.owner, 0f, 0f);

            Projectile.velocity = shootDirection;
        }

        public void UpdateProjectileHeldVariables(Vector2 armPosition)
        {
            Projectile.position = armPosition - Projectile.Size * 0.5f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.spriteDirection == -1)
                Projectile.rotation += MathHelper.Pi;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
        }

        public void ManipulatePlayerVariables()
        {
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
        }
    }
}
