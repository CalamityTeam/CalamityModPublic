using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class AetherfluxCannonHoldout : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<AetherfluxCannon>();
        private const int FramesPerFireRateIncrease = 36;

        private Player Owner => Main.player[Projectile.owner];
        private bool OwnerCanShoot => Owner.channel && Owner.HasAmmo(Owner.ActiveItem()) && !Owner.noItems && !Owner.CCed;

        private ref float DeployedFrames => ref Projectile.ai[0];
        private ref float AnimationRate => ref Projectile.ai[1];
        private ref float LastShootAttemptTime => ref Projectile.localAI[0];
        private ref float LastAnimationTime => ref Projectile.localAI[1];

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.width = 58;
            Projectile.height = 94;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Vector2 armPosition = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            Vector2 gunBarrelPos = armPosition + Projectile.velocity * Projectile.height * 0.4f;

            // If the player is unable to continue using the holdout, delete it.
            if (!OwnerCanShoot)
            {
                Projectile.Kill();
                return;
            }

            // Play a sound frame 1.
            if (DeployedFrames <= 0f)
            {
                SoundEngine.PlaySound(SoundID.DD2_DarkMageCastHeal with { Volume = SoundID.DD2_DarkMageCastHeal.Volume * 1.5f}, Projectile.Center);
            }

            // Update damage based on curent magic damage stat (so Mana Sickness affects it)
            Item weaponItem = Owner.ActiveItem();
            Projectile.damage = weaponItem is null ? 0 : Owner.GetWeaponDamage(weaponItem);

            // Get the original weapon's use time.
            int itemUseTime = weaponItem?.useAnimation ?? AetherfluxCannon.UseTime;
            // 36, base use time, will result in 5. Speed increasing reforges push it to 4.
            int framesPerShot = itemUseTime / 7;

            // Update time.
            DeployedFrames += 1f;

            // Determine animation rate. If the gun is spinning up, it increases linearly. Otherwise it's maxed.
            AnimationRate = DeployedFrames >= itemUseTime ? 2f : MathHelper.Lerp(7f, 2f, DeployedFrames / itemUseTime);

            // Update the animation. This occurs even when firing is not occurring.
            if (DeployedFrames - LastAnimationTime >= AnimationRate)
            {
                LastAnimationTime = DeployedFrames;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
            }

            // Once past the initial spin-up time, fire constantly as long as mana is available.
            // Before the spin-up is done, sparks are produced but no lasers come out (and no mana is consumed).
            if (DeployedFrames - LastShootAttemptTime >= framesPerShot)
            {
                LastShootAttemptTime = DeployedFrames;
                bool actuallyShoot = DeployedFrames >= itemUseTime;
                bool manaOK = !actuallyShoot || Owner.CheckMana(Owner.ActiveItem(), -1, true, false);
                if (manaOK)
                {
                    if (actuallyShoot)
                        SoundEngine.PlaySound(SoundID.Item91, Projectile.Center);

                    int projID = ModContent.ProjectileType<PhasedGodRay>();
                    float shootSpeed = weaponItem.shootSpeed;
                    Vector2 shootDirection = Projectile.velocity.SafeNormalize(Vector2.UnitY);
                    Vector2 shootVelocity = shootDirection * shootSpeed;

                    // Waving beams need to start offset so they cross each other neatly.
                    float waveSideOffset = Main.rand.NextFloat(18f, 28f);
                    Vector2 perp = shootDirection.RotatedBy(-MathHelper.PiOver2) * waveSideOffset;

                    // Dust chaotically sheds off the crystal while charging or firing.
                    float dustInaccuracy = 0.045f;

                    for (int i = -1; i <= 1 ; i += 2)
                    {
                        Vector2 laserStartPos = gunBarrelPos + i * perp + Main.rand.NextVector2CircularEdge(6f, 6f);
                        Vector2 dustOnlySpread = Main.rand.NextVector2Circular(shootSpeed, shootSpeed);
                        Vector2 dustVelocity = shootVelocity + dustInaccuracy * dustOnlySpread;
                        if (actuallyShoot)
                        {
                            Projectile godRay = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), laserStartPos, shootVelocity, projID, Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
                            // Tell this Phased God Ray exactly which way it should be waving.
                            godRay.localAI[1] = i * 0.5f;
                        }
                        SpawnFiringDust(gunBarrelPos, dustVelocity);
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
            int dustID = 133;
            int dustRadius = 12;
            float dustRandomness = 11f;
            int dustDiameter = 2 * dustRadius;
            Vector2 dustCorner = gunBarrelPos - Vector2.One * dustRadius;
            for (int i = 0; i < 2; i++)
            {
                Vector2 dustVel = laserVelocity + Main.rand.NextVector2Circular(dustRandomness, dustRandomness);
                Dust d = Dust.NewDustDirect(dustCorner, dustDiameter, dustDiameter, dustID, dustVel.X, dustVel.Y);
                d.velocity *= 0.18f;
                d.noGravity = true;
                d.scale = 0.6f;
            }
        }

        public override bool? CanDamage() => false;

        // prevents the item from appearing backwards frame 1
        public override bool PreDraw(ref Color lightColor) => FramesPerFireRateIncrease > 0f;
    }
}
