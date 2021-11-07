using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class NanoPurgeHoldout : ModProjectile
    {
        private const int FramesPerFireRateIncrease = 36;
        private static int[] LaserOffsetByAnimationFrame = { 4, 3, 0, 3 };

        private Player Owner => Main.player[projectile.owner];
        private bool OwnerCanShoot => Owner.channel && Owner.HasAmmo(Owner.ActiveItem(), true) && !Owner.noItems && !Owner.CCed;
        private ref float DeployedFrames => ref projectile.ai[0];
        private ref float ChargeTowardsNextShot => ref projectile.ai[1];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nano Purge");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 62;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.magic = true;
        }

        public override void AI()
        {
            Vector2 armPosition = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            Vector2 gunBarrelPos = armPosition + projectile.velocity * projectile.height * 0.5f;

            // If the player is unable to continue using the holdout, delete it.
            if (!OwnerCanShoot)
            {
                projectile.Kill();
                return;
            }

            // Update damage based on curent magic damage stat (so Mana Sickness affects it)
            Item nanoPurge = Owner.ActiveItem();
            projectile.damage = (int)((nanoPurge?.damage ?? 0) * Owner.MagicDamage());

            // Get the original weapon's use time.
            int itemUseTime = nanoPurge?.useAnimation ?? Purge.UseTime;

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
                projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];

                bool manaCostPaid = Owner.CheckMana(Owner.ActiveItem(), -1, true, false);
                if (manaCostPaid)
                {
                    Main.PlaySound(SoundID.Item91, projectile.Center);

                    int projID = ModContent.ProjectileType<NanoPurgeLaser>();
                    float shootSpeed = nanoPurge.shootSpeed;
                    float inaccuracyRatio = 0.045f;
                    Vector2 shootDirection = projectile.velocity.SafeNormalize(Vector2.UnitY);
                    Vector2 perp = shootDirection.RotatedBy(MathHelper.PiOver2);

                    // Fire a pair of lasers, one with a negative offset, one with a positive offset.
                    for (int i = -1; i <= 1; i += 2)
                    {
                        Vector2 spread = Main.rand.NextVector2CircularEdge(shootSpeed, shootSpeed);
                        Vector2 shootVelocity = shootDirection * shootSpeed + inaccuracyRatio * spread;
                        Vector2 splitBarrelPos = gunBarrelPos + i * LaserOffsetByAnimationFrame[projectile.frame] * perp;
                        Projectile.NewProjectile(splitBarrelPos, shootVelocity, projID, projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                        SpawnFiringDust(splitBarrelPos, shootVelocity);
                    }
                }

                // Delete the laser gun if a mana cost cannot be paid.
                else
                {
                    projectile.Kill();
                    return;
                }
            }

            UpdateProjectileHeldVariables(armPosition);
            ManipulatePlayerVariables();
        }

        private void UpdateProjectileHeldVariables(Vector2 armPosition)
        {
            if (Main.myPlayer == projectile.owner)
            {
                float interpolant = Utils.InverseLerp(5f, 25f, projectile.Distance(Main.MouseWorld), true);
                Vector2 oldVelocity = projectile.velocity;
                projectile.velocity = Vector2.Lerp(projectile.velocity, projectile.SafeDirectionTo(Main.MouseWorld), interpolant);
                if (projectile.velocity != oldVelocity)
                {
                    projectile.netSpam = 0;
                    projectile.netUpdate = true;
                }
            }

            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            projectile.Center = armPosition + projectile.velocity.SafeNormalize(Vector2.UnitX * Owner.direction) * 8f;
            projectile.spriteDirection = projectile.direction;
            projectile.timeLeft = 2;
        }

        private void ManipulatePlayerVariables()
        {
            Owner.ChangeDir(projectile.direction);
            Owner.heldProj = projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = (projectile.velocity * projectile.direction).ToRotation();
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

        public override bool CanDamage() => false;

        // prevents the item from appearing backwards frame 1
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => projectile.ai[0] > 0f;
    }
}
