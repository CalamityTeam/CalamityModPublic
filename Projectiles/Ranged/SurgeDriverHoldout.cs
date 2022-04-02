using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class SurgeDriverHoldout : ModProjectile
    {
        public Player Owner => Main.player[projectile.owner];
        public bool OwnerCanShoot => Owner.channel && Owner.HasAmmo(Owner.ActiveItem(), true) && !Owner.noItems && !Owner.CCed;
        public ref float ShootCountdown => ref projectile.ai[0];
        public override void SetStaticDefaults() => DisplayName.SetDefault("Surge Driver");

        public override void SetDefaults()
        {
            projectile.width = 192;
            projectile.height = 52;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ranged = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (!OwnerCanShoot)
                projectile.Kill();

            Vector2 armPosition = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            armPosition += projectile.velocity.SafeNormalize(Owner.direction * Vector2.UnitX) * 32f;
            armPosition.Y -= 12f;

            UpdateProjectileHeldVariables(armPosition);
            ManipulatePlayerVariables();

            ShootCountdown--;
            if (ShootCountdown <= 0f)
            {
                if (Main.myPlayer == projectile.owner)
                {
                    ShootProjectiles(armPosition);
                    ShootCountdown = 28f;
                    projectile.netUpdate = true;
                }

                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon"), projectile.Center);
            }
        }

        public void ShootProjectiles(Vector2 armPosition)
        {
            if (Main.myPlayer != projectile.owner)
                return;

            Item heldItem = Owner.ActiveItem();
            int projectileType = ModContent.ProjectileType<PrismaticEnergyBlast>();
            float shootSpeed = heldItem.shootSpeed * projectile.scale * 0.64f;
            int damage = (int)(Owner.GetWeaponDamage(heldItem) * 6.66);
            float knockback = heldItem.knockBack;

            bool uselessFuckYou = OwnerCanShoot;
            Owner.PickAmmo(heldItem, ref projectileType, ref shootSpeed, ref uselessFuckYou, ref damage, ref knockback, false);
            projectileType = ModContent.ProjectileType<PrismaticEnergyBlast>();

            knockback = Owner.GetWeaponKnockback(heldItem, knockback);
            Vector2 shootDirection = (Main.MouseWorld - projectile.Center).SafeNormalize(-Vector2.UnitY);
            Vector2 shootVelocity = shootDirection * shootSpeed;

            // Sync if the shoot direction changes.
            if (shootDirection.X != projectile.velocity.X || shootDirection.Y != projectile.velocity.Y)
                projectile.netUpdate = true;

            Vector2 gunTip = armPosition + shootDirection * heldItem.scale * 130f;
            Projectile.NewProjectile(gunTip, shootVelocity, projectileType, damage, knockback, projectile.owner, 0f, 0f);

            projectile.velocity = shootDirection;
        }

        public void UpdateProjectileHeldVariables(Vector2 armPosition)
        {
            projectile.position = armPosition - projectile.Size * 0.5f;
            projectile.rotation = projectile.velocity.ToRotation();
            if (projectile.spriteDirection == -1)
                projectile.rotation += MathHelper.Pi;
            projectile.spriteDirection = projectile.direction;
            projectile.timeLeft = 2;
        }

        public void ManipulatePlayerVariables()
        {
            Owner.ChangeDir(projectile.direction);
            Owner.heldProj = projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = (projectile.velocity * projectile.direction).ToRotation();
        }
    }
}
