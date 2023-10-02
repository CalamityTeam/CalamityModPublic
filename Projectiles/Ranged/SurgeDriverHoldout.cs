using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class SurgeDriverHoldout : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<SurgeDriver>();
        public Player Owner => Main.player[Projectile.owner];
        public bool OwnerCanShoot => Owner.channel && !Owner.noItems && !Owner.CCed;
        public ref float ShootCountdown => ref Projectile.ai[0];

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
            Vector2 armPosition = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            armPosition += Projectile.velocity.SafeNormalize(Owner.direction * Vector2.UnitX) * 32f;
            armPosition.Y -= 12f;

            UpdateProjectileHeldVariables(armPosition);
            ManipulatePlayerVariables();

            if (!OwnerCanShoot)
            {
                // Prevent spam clicking by letting the animation run if the player just stops holding
                if (Owner.noItems || Owner.CCed || Owner.dead || ShootCountdown < 0f)
                    Projectile.Kill();
            }
            // Can't shoot on frame 1 as it can't use ammo yet
            else if (ShootCountdown < 0f && Owner.HasAmmo(Owner.ActiveItem()))
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    ShootProjectiles(armPosition);
                    ShootCountdown = Owner.ActiveItem().useAnimation - 1;
                    Projectile.netUpdate = true;
                }

                SoundEngine.PlaySound(CommonCalamitySounds.LaserCannonSound, Projectile.Center);
            }
            ShootCountdown--;
        }

        public void ShootProjectiles(Vector2 armPosition)
        {
            if (Main.myPlayer != Projectile.owner)
                return;

            Item heldItem = Owner.ActiveItem();
            Owner.PickAmmo(heldItem, out int projectileType, out float shootSpeed, out int damage, out float knockback, out _);
            damage *= 4;
            shootSpeed = heldItem.shootSpeed * Projectile.scale * 0.64f;
            projectileType = ModContent.ProjectileType<PrismaticEnergyBlast>();

            knockback = Owner.GetWeaponKnockback(heldItem, knockback);
            Vector2 shootDirection = (Main.MouseWorld - Projectile.Center).SafeNormalize(-Vector2.UnitY);
            Vector2 shootVelocity = shootDirection * shootSpeed;

            // Sync if the shoot direction changes.
            if (shootDirection.X != Projectile.velocity.X || shootDirection.Y != Projectile.velocity.Y)
                Projectile.netUpdate = true;

            Vector2 gunTip = armPosition + shootDirection * heldItem.scale * 130f;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), gunTip, shootVelocity, projectileType, damage, knockback, Projectile.owner, 0f, 0f);

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

        public override bool? CanDamage() => false;
    }
}
