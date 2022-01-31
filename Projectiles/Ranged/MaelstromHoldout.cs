using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class MaelstromHoldout : ModProjectile
    {
        private Player Owner => Main.player[projectile.owner];
        private bool OwnerCanShoot => Owner.channel && Owner.HasAmmo(Owner.ActiveItem(), true) && !Owner.noItems && !Owner.CCed;
        private ref float CurrentChargingFrames => ref projectile.ai[0];
        private ref float ArrowsLoaded => ref projectile.ai[1];
        private ref float FramesToLoadNextArrow => ref projectile.localAI[0];

        public override string Texture => "CalamityMod/Items/Weapons/Ranged/TheMaelstrom";
        public override void SetStaticDefaults() => DisplayName.SetDefault("The Maelstrom");

        public override void SetDefaults()
        {
            projectile.width = 78;
            projectile.height = 137;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.ranged = true;
        }

        public override void AI()
        {
            Vector2 armPosition = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            Vector2 shootPosition = armPosition + projectile.velocity * projectile.width * 0.5f;

            // Destroy the holdout projectile if the owner is no longer eligible to hold it.
            if (!OwnerCanShoot)
            {
                projectile.Kill();
                return;
            }

            // Frame 1 effects: Record how fast the hold item being used is, to determine how fast to load arrows.
            if (FramesToLoadNextArrow == 0f)
            {
                Main.PlaySound(SoundID.Item20, projectile.Center);
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
            Main.PlaySound(SoundID.Item66, projectile.Center);
            Main.PlaySound(SoundID.Item96, projectile.Center);

            if (Main.myPlayer != projectile.owner)
                return;

            Item heldItem = Owner.ActiveItem();

            // Calculate damage at the instant the arrow is fired
            int arrowDamage = (int)(heldItem.damage * Owner.RangedDamage());
            float shootSpeed = heldItem.shootSpeed;
            float knockback = heldItem.knockBack;

            bool uselessFuckYou = OwnerCanShoot;
            int projectileType = 0;
            Owner.PickAmmo(heldItem, ref projectileType, ref shootSpeed, ref uselessFuckYou, ref arrowDamage, ref knockback, false);
            projectileType = ModContent.ProjectileType<TheMaelstromShark>();

            knockback = Owner.GetWeaponKnockback(heldItem, knockback);
            Vector2 shootVelocity = projectile.velocity.SafeNormalize(Vector2.UnitY) * shootSpeed;

            Projectile.NewProjectile(shootPosition, shootVelocity, projectileType, arrowDamage, knockback, projectile.owner, 0f, 0f);
        }

        private void UpdateProjectileHeldVariables(Vector2 armPosition)
        {
            if (Main.myPlayer == projectile.owner)
            {
                float aimInterpolant = Utils.InverseLerp(5f, 25f, projectile.Distance(Main.MouseWorld), true);
                Vector2 oldVelocity = projectile.velocity;
                projectile.velocity = Vector2.Lerp(projectile.velocity, projectile.SafeDirectionTo(Main.MouseWorld), aimInterpolant);
                if (projectile.velocity != oldVelocity)
                {
                    projectile.netSpam = 0;
                    projectile.netUpdate = true;
                }
            }

            projectile.position = armPosition - projectile.Size * 0.5f + projectile.velocity * 24f;
            projectile.rotation = projectile.velocity.ToRotation();
            if (projectile.spriteDirection == -1)
                projectile.rotation += MathHelper.Pi;
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

        public override bool CanDamage() => false;
    }
}
