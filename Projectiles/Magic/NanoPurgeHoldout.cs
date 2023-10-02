using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class NanoPurgeHoldout : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<NanoPurge>();
        private const int FramesPerFireRateIncrease = 36;
        private static int[] LaserOffsetByAnimationFrame = { 4, 3, 0, 3 };

        private Player Owner => Main.player[Projectile.owner];
        private bool OwnerCanShoot => Owner.channel && Owner.HasAmmo(Owner.ActiveItem()) && !Owner.noItems && !Owner.CCed;
        private ref float DeployedFrames => ref Projectile.ai[0];
        private ref float ChargeTowardsNextShot => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 62;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Vector2 armPosition = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            Vector2 gunBarrelPos = armPosition + Projectile.velocity * Projectile.height * 0.5f;

            // If the player is unable to continue using the holdout, delete it.
            if (!OwnerCanShoot)
            {
                Projectile.Kill();
                return;
            }

            // Update damage based on curent magic damage stat (so Mana Sickness affects it)
            Item weaponItem = Owner.ActiveItem();
            Projectile.damage = weaponItem is null ? 0 : Owner.GetWeaponDamage(weaponItem);

            // Get the original weapon's use time.
            int itemUseTime = weaponItem?.useAnimation ?? NanoPurge.UseTime;

            // Update time.
            DeployedFrames += 1f;

            // Choose fire rate multiplier (1x, 2x, 3x, 4x) based on current time spent firing.
            int fireRate = (int)MathHelper.Clamp(DeployedFrames / FramesPerFireRateIncrease, 1f, 4f);

            // Increment counter towards the item's use time by an amount equal to the current fire rate.
            ChargeTowardsNextShot += fireRate;

            // If enough charging progress is made, perform a shoot event if enough mana is available.
            if (ChargeTowardsNextShot >= itemUseTime)
            {
                ChargeTowardsNextShot -= itemUseTime;

                // Update the animation.
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];

                bool manaCostPaid = Owner.CheckMana(Owner.ActiveItem(), -1, true, false);
                if (manaCostPaid)
                {
                    SoundEngine.PlaySound(SoundID.Item91, Projectile.Center);

                    int projID = ModContent.ProjectileType<NanoPurgeLaser>();
                    float shootSpeed = weaponItem.shootSpeed;
                    float inaccuracyRatio = 0.045f;
                    Vector2 shootDirection = Projectile.velocity.SafeNormalize(Vector2.UnitY);
                    Vector2 perp = shootDirection.RotatedBy(MathHelper.PiOver2);

                    // Fire a pair of lasers, one with a negative offset, one with a positive offset.
                    for (int i = -1; i <= 1; i += 2)
                    {
                        Vector2 spread = Main.rand.NextVector2CircularEdge(shootSpeed, shootSpeed);
                        Vector2 shootVelocity = shootDirection * shootSpeed + inaccuracyRatio * spread;
                        Vector2 splitBarrelPos = gunBarrelPos + i * LaserOffsetByAnimationFrame[Projectile.frame] * perp;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), splitBarrelPos, shootVelocity, projID, Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
                        SpawnFiringDust(splitBarrelPos, shootVelocity);
                    }
                }

                // Delete the laser gun if a mana cost cannot be paid.
                else
                {
                    Projectile.Kill();
                    return;
                }
            }

            UpdateProjectileHeldVariables(armPosition);
            ManipulatePlayerVariables();
        }

        private void UpdateProjectileHeldVariables(Vector2 armPosition)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                float interpolant = Utils.GetLerpValue(5f, 25f, Projectile.Distance(Main.MouseWorld), true);
                Vector2 oldVelocity = Projectile.velocity;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(Main.MouseWorld), interpolant);
                if (Projectile.velocity != oldVelocity)
                {
                    Projectile.netSpam = 0;
                    Projectile.netUpdate = true;
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.Center = armPosition + Projectile.velocity.SafeNormalize(Vector2.UnitX * Owner.direction) * 8f;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
        }

        private void ManipulatePlayerVariables()
        {
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
        }

        private void SpawnFiringDust(Vector2 gunBarrelPos, Vector2 laserVelocity)
        {
            int dustID = 107;
            int dustRadius = 5;
            int dustDiameter = 2 * dustRadius;
            Vector2 dustCorner = gunBarrelPos - Vector2.One * dustRadius;
            for (int i = 0; i < 2; i++)
            {
                Vector2 dustVel = laserVelocity + Main.rand.NextVector2Circular(7f, 7f);
                Dust d = Dust.NewDustDirect(dustCorner, dustDiameter, dustDiameter, dustID, dustVel.X, dustVel.Y);
                d.velocity *= 0.125f;
                d.noGravity = true;
                d.scale = 1.4f;
            }
        }

        public override bool? CanDamage() => false;

        // prevents the item from appearing backwards frame 1
        public override bool PreDraw(ref Color lightColor) => DeployedFrames > 0f;
    }
}
