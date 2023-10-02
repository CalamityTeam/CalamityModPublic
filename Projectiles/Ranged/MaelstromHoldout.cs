using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class MaelstromHoldout : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<TheMaelstrom>();
        private Player Owner => Main.player[Projectile.owner];
        private bool OwnerCanShoot => Owner.channel && Owner.HasAmmo(Owner.ActiveItem()) && !Owner.noItems && !Owner.CCed;
        private ref float CurrentChargingFrames => ref Projectile.ai[0];
        private ref float ArrowsLoaded => ref Projectile.ai[1];
        private ref float FramesToLoadNextArrow => ref Projectile.localAI[0];

        public override string Texture => "CalamityMod/Items/Weapons/Ranged/TheMaelstrom";

        public override void SetDefaults()
        {
            Projectile.width = 78;
            Projectile.height = 137;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            Vector2 armPosition = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            Vector2 shootPosition = armPosition + Projectile.velocity * Projectile.width * 0.5f;

            // Destroy the holdout projectile if the owner is no longer eligible to hold it.
            if (!OwnerCanShoot)
            {
                Projectile.Kill();
                return;
            }

            // Frame 1 effects: Record how fast the hold item being used is, to determine how fast to load arrows.
            if (FramesToLoadNextArrow == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
                FramesToLoadNextArrow = Owner.ActiveItem().useAnimation;
            }

            // Actually make progress towards loading more arrows.
            CurrentChargingFrames++;

            // If it is time to do so, produce a pulse of particles and shoot an arrow.
            if (CurrentChargingFrames % FramesToLoadNextArrow == FramesToLoadNextArrow - 1)
                ShootProjectiles(shootPosition);

            UpdateProjectileHeldVariables(armPosition);
            ManipulatePlayerVariables();
        }

        public void ShootProjectiles(Vector2 shootPosition)
        {
            // Create electric particles.
            if (Main.netMode != NetmodeID.Server)
            {
                for (int i = 0; i < 16; i++)
                {
                    int sparkLifetime = Main.rand.Next(22, 36);
                    float sparkScale = Main.rand.NextFloat(1f, 1.3f);
                    Color sparkColor = Color.Lerp(Color.Cyan, Color.AliceBlue, Main.rand.NextFloat(0.35f));
                    Vector2 sparkVelocity = (MathHelper.TwoPi * i / 16f + Main.rand.NextFloat(0.09f)).ToRotationVector2() * Main.rand.NextFloat(6f, 14f);
                    sparkVelocity.Y -= Owner.gravDir * 4f;

                    SparkParticle spark = new SparkParticle(shootPosition, sparkVelocity, false, sparkLifetime, sparkScale, sparkColor);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }

            // Play a shoot sound.
            SoundEngine.PlaySound(SoundID.Item66, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item96, Projectile.Center);

            if (Main.myPlayer != Projectile.owner)
                return;

            Item heldItem = Owner.ActiveItem();

            // Calculate damage at the instant the arrow is fired
            int arrowDamage = heldItem is null ? 0 : Owner.GetWeaponDamage(heldItem);
            float shootSpeed = heldItem.shootSpeed;
            float knockback = heldItem.knockBack;

            bool uselessFuckYou = OwnerCanShoot;
            int projectileType = 0;
            Owner.PickAmmo(heldItem, out projectileType, out shootSpeed, out arrowDamage, out knockback, out _);
            projectileType = ModContent.ProjectileType<TheMaelstromShark>();

            knockback = Owner.GetWeaponKnockback(heldItem, knockback);
            Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * shootSpeed;

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), shootPosition, shootVelocity, projectileType, arrowDamage, knockback, Projectile.owner, 0f, 0f);
        }

        private void UpdateProjectileHeldVariables(Vector2 armPosition)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                float aimInterpolant = Utils.GetLerpValue(5f, 25f, Projectile.Distance(Main.MouseWorld), true);
                Vector2 oldVelocity = Projectile.velocity;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(Main.MouseWorld), aimInterpolant);
                if (Projectile.velocity != oldVelocity)
                {
                    Projectile.netSpam = 0;
                    Projectile.netUpdate = true;
                }
            }

            Projectile.position = armPosition - Projectile.Size * 0.5f + Projectile.velocity * 24f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.spriteDirection == -1)
                Projectile.rotation += MathHelper.Pi;
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

        public override bool? CanDamage() => false;
    }
}
