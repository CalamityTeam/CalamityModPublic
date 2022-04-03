using CalamityMod.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    public class CondemnationHoldout : ModProjectile
    {
        private Player Owner => Main.player[Projectile.owner];
        private bool OwnerCanShoot => Owner.channel && Owner.HasAmmo(Owner.ActiveItem(), true) && !Owner.noItems && !Owner.CCed;
        private ref float CurrentChargingFrames => ref Projectile.ai[0];
        private ref float ArrowsLoaded => ref Projectile.ai[1];
        private ref float FramesToLoadNextArrow => ref Projectile.localAI[0];

        public override string Texture => "CalamityMod/Items/Weapons/Ranged/Condemnation";
        public override void SetStaticDefaults() => DisplayName.SetDefault("Condemnation");

        public override void SetDefaults()
        {
            Projectile.width = 130;
            Projectile.height = 42;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Vector2 armPosition = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            Vector2 tipPosition = armPosition + Projectile.velocity * Projectile.width * 0.5f;

            // Fire arrows if the owner stops channeling or otherwise cannot use the weapon.
            if (!OwnerCanShoot)
            {
                // No arrows left to shoot? The bow disappears.
                if (ArrowsLoaded <= 0f)
                {
                    Projectile.Kill();
                    return;
                }

                // Fire one charged arrow every frame until you're out of arrows.
                ShootProjectiles(tipPosition);
                --ArrowsLoaded;
            }
            else
            {

                // Frame 1 effects: Record how fast the Condemnation item being used is, to determine how fast to load arrows.
                if (FramesToLoadNextArrow == 0f)
                {
                    SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
                    FramesToLoadNextArrow = Owner.ActiveItem().useAnimation;
                }

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
                    var loadSound = SoundEngine.PlaySound(SoundID.Item108);
                    if (loadSound != null)
                        loadSound.Volume *= 0.3f;

                    if (ArrowsLoaded >= Condemnation.MaxLoadedArrows)
                        SoundEngine.PlaySound(SoundID.MaxMana);
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
                chargeMagic.color = Projectile.GetAlpha(Color.White);
                chargeMagic.noGravity = true;
            }
        }

        public void ShootProjectiles(Vector2 tipPosition)
        {
            if (Main.myPlayer != Projectile.owner)
                return;

            Item heldItem = Owner.ActiveItem();
            // calculate damage at the instant this arrow is fired
            int arrowDamage = (int)(heldItem.damage * Owner.RangedDamage());
            float shootSpeed = heldItem.shootSpeed * 1.5f; // Loaded arrows move faster than normal.
            float knockback = heldItem.knockBack;

            bool uselessFuckYou = OwnerCanShoot;
            int projectileType = 0;
            Owner.PickAmmo(heldItem, ref projectileType, ref shootSpeed, ref uselessFuckYou, ref arrowDamage, ref knockback, false);
            projectileType = ModContent.ProjectileType<CondemnationArrow>();

            knockback = Owner.GetWeaponKnockback(heldItem, knockback);
            Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * shootSpeed;

            Projectile.NewProjectile(tipPosition, shootVelocity, projectileType, arrowDamage, knockback, Projectile.owner, 0f, 0f);
        }

        private void UpdateProjectileHeldVariables(Vector2 armPosition)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                float interpolant = Utils.InverseLerp(5f, 25f, Projectile.Distance(Main.MouseWorld), true);
                Vector2 oldVelocity = Projectile.velocity;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(Main.MouseWorld), interpolant);
                if (Projectile.velocity != oldVelocity)
                {
                    Projectile.netSpam = 0;
                    Projectile.netUpdate = true;
                }
            }

            Projectile.position = armPosition - Projectile.Size * 0.5f;
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

        public override bool CanDamage() => false;
    }
}
