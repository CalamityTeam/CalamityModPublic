using CalamityMod.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class CondemnationHoldout : ModProjectile
    {
        public Player Owner => Main.player[projectile.owner];
        public bool OwnerCanShoot => Owner.channel && Owner.HasAmmo(Owner.ActiveItem(), true) && !Owner.noItems && !Owner.CCed;
        public ref float CurrentChargingFrames => ref projectile.ai[0];
        public ref float ArrowsLoaded => ref projectile.ai[1];
        public ref float FramesToLoadNextArrow => ref projectile.localAI[0];

        public override string Texture => "CalamityMod/Items/Weapons/Ranged/Condemnation";
        public override void SetStaticDefaults() => DisplayName.SetDefault("Condemnation");

        public override void SetDefaults()
        {
            projectile.width = 130;
            projectile.height = 42;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ranged = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Vector2 armPosition = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            Vector2 tipPosition = armPosition + projectile.velocity * projectile.width * 0.5f;

            // Fire arrows if the owner stops channeling or otherwise cannot use the weapon.
            if (!OwnerCanShoot)
            {
                // No arrows left to shoot? The bow disappears.
                if (ArrowsLoaded <= 0f)
                {
                    projectile.Kill();
                    return;
                }

                // Fire one charged arrow every frame until you're out of arrows.
                ShootProjectiles(tipPosition, 1f);
                --ArrowsLoaded;
                Main.PlaySound(SoundID.DD2_BallistaTowerShot);
            }
            else
            {

                // Frame 1 effects: Record how fast the Condemnation item being used is, to determine how fast to load arrows.
                if (FramesToLoadNextArrow == 0f)
                    FramesToLoadNextArrow = Owner.ActiveItem().useAnimation;

                // Actually make progress towards loading more arrows.
                ++CurrentChargingFrames;

                // If no arrows are loaded, spawn a bit of dust to indicate it's not ready yet.
                // Spawn the same dust if the max number of arrows have been loaded.
                if (ArrowsLoaded <= 0f || ArrowsLoaded >= Condemnation.MaxLoadedArrows)
                    SpawnCannotLoadArrowsDust(tipPosition);

                // If it is time to load an arrow, produce a pulse of dust and add an arrow.
                // Also accelerate charging, because it's fucking awesome.
                if (CurrentChargingFrames >= FramesToLoadNextArrow && ArrowsLoaded < Condemnation.MaxLoadedArrows)
                {
                    SpawnArrowLoadedDust(tipPosition);
                    CurrentChargingFrames = 0f;
                    ++ArrowsLoaded;
                    --FramesToLoadNextArrow;

                    // Play a sound for additional notification that an arrow has been loaded.
                    var loadSound = Main.PlaySound(SoundID.Item108);
                    if (loadSound != null)
                        loadSound.Volume *= 0.3f;

                    if (ArrowsLoaded >= Condemnation.MaxLoadedArrows)
                        Main.PlaySound(SoundID.MaxMana);
                }
            }

            UpdateProjectileHeldVariables(armPosition);
            ManipulatePlayerVariables();
        }

        public void SpawnArrowLoadedDust(Vector2 tipPosition)
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 36; i++)
            {
                Dust chargeMagic = Dust.NewDustPerfect(tipPosition, 267);
                chargeMagic.velocity = (MathHelper.TwoPi * i / 36f).ToRotationVector2() * 5f + Owner.velocity;
                chargeMagic.scale = Main.rand.NextFloat(1f, 1.5f);
                chargeMagic.color = Color.Violet;
                chargeMagic.noGravity = true;
            }
        }

        public void SpawnCannotLoadArrowsDust(Vector2 tipPosition)
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 2; i++)
            {
                Dust chargeMagic = Dust.NewDustPerfect(tipPosition + Main.rand.NextVector2Circular(20f, 20f), 267);
                chargeMagic.velocity = (tipPosition - chargeMagic.position) * 0.1f + Owner.velocity;
                chargeMagic.scale = Main.rand.NextFloat(1f, 1.5f);
                chargeMagic.color = projectile.GetAlpha(Color.White);
                chargeMagic.noGravity = true;
            }
        }

        public void ShootProjectiles(Vector2 tipPosition, float speedFactor)
        {
            if (Main.myPlayer != projectile.owner)
                return;

            Item heldItem = Owner.ActiveItem();
            // calculate damage at the instant this arrow is fired
            int arrowDamage = (int)(heldItem.damage * Owner.RangedDamage());
            float shootSpeed = heldItem.shootSpeed * speedFactor * 1.5f;
            float knockback = heldItem.knockBack;

            bool uselessFuckYou = OwnerCanShoot;
            int projectileType = 0;
            Owner.PickAmmo(heldItem, ref projectileType, ref shootSpeed, ref uselessFuckYou, ref arrowDamage, ref knockback, false);
            projectileType = ModContent.ProjectileType<CondemnationArrow>();

            knockback = Owner.GetWeaponKnockback(heldItem, knockback);
            Vector2 shootVelocity = projectile.velocity.SafeNormalize(Vector2.UnitY) * shootSpeed;

            Projectile.NewProjectile(tipPosition, shootVelocity, projectileType, arrowDamage, knockback, projectile.owner, 0f, 0f);
        }

        public void UpdateProjectileHeldVariables(Vector2 armPosition)
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

        public override bool CanDamage() => false;
    }
}
