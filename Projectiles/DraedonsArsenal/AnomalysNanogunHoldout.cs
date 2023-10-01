using System;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Particles;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class AnomalysNanogunHoldout : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        #region Properties
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public static Texture2D ScopeTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/DraedonsArsenal/AnomalysNanogunScope", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        public ref float AITimer => ref Projectile.ai[1];
        public Player Owner => Main.player[Projectile.owner];
        public Vector2 MouseDistance => Owner.Calamity().mouseWorld - Owner.MountedCenter;
        public ref float ScopeRotation => ref Projectile.localAI[0];
        public ref float ScopeScale => ref Projectile.localAI[1];
        public ref float TargetRecoil => ref Projectile.localAI[0];
        public bool UseMPFBAI { get => Projectile.ai[0] == MPFBAIType; }
        #endregion

        #region Weapon Attributes
        public const float GunLength = 64f;
        public const float MPFBAIType = 2f;

        public const int MPFBFireTimer = 59;
        public const float MPFBRecoil = MathHelper.Pi - MathHelper.PiOver4;
        public const float MPFBScreenShakePower = 5f;
        public const float MPFBPushback = 15f;
        public const float MaxMPFBPropellableSpeed = 14f;

        public const int PlasmaChargeupTimer = 40;
        public const int PlasmaCooldownTimer = 24;
        public const int PlasmaShotCooldown = 20;
        public const int PlasmaShotCount = 5;
        public static int PlasmaFireTimer
        {
            get => PlasmaChargeupTimer + PlasmaCooldownTimer + (PlasmaShotCooldown * PlasmaShotCount);
        }
        #endregion

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            // Reset the holdout rotation and position
            Owner.Calamity().mouseWorldListener = true;
            Projectile.rotation = (Owner.Calamity().mouseWorld - Owner.MountedCenter).ToRotation();
            Projectile.Center = Owner.MountedCenter;
            Owner.heldProj = Projectile.whoAmI;

            if (UseMPFBAI)
                UpdateMPFB();
            else
                UpdatePlasmaBeam();

            AITimer++;
        }

        // AI for the right click
        public void UpdateMPFB()
        {
            // Make sure the player faces the right way
            if (Projectile.velocity.X != 0f)
                Owner.ChangeDir(Math.Sign(Projectile.velocity.X));
            else
                Owner.ChangeDir(1);

            if (AITimer == 0)
            {
                SoundEngine.PlaySound(TheAnomalysNanogun.MPFBShotSFX, Projectile.Center);

                // Shoot 3 MPFBs with varying speeds and randomized animation starting placement
                for (int i = 0; i < 3; i++)
                {
                    int proj = Projectile.NewProjectile(new EntitySource_ItemUse_WithAmmo(Owner, Owner.HeldItem, -1),
                        Projectile.Center + Projectile.rotation.ToRotationVector2() * GunLength,
                        Projectile.velocity * (1f - i * 0.18f),
                        ModContent.ProjectileType<AnomalysNanogunMPFBDevastator>(),
                        Projectile.damage,
                        Projectile.knockBack,
                        Projectile.owner);

                    if (Main.projectile.IndexInRange(proj))
                    {
                        Main.projectile[proj].frame = Main.rand.Next(0, 4);
                        Main.projectile[proj].frameCounter = Main.rand.Next(0, 3);
                    }
                }

                // Make it POWERFUL
                Owner.Calamity().GeneralScreenShakePower = MPFBScreenShakePower;
                float playerSpeed = Owner.velocity.Length();
                Vector2 pushback = Projectile.velocity.SafeNormalize(Vector2.UnitX) * -MPFBPushback;
                Vector2 newPlayerVelocity = Owner.velocity + pushback;
                float newPlayerSpeed = newPlayerVelocity.Length();

                if (playerSpeed < MaxMPFBPropellableSpeed || newPlayerSpeed < playerSpeed)
                    Owner.velocity = newPlayerVelocity;
                else
                    Owner.velocity = newPlayerVelocity.SafeNormalize(Vector2.UnitX) * playerSpeed;

                // Assign target recoil
                Vector2 direction = MouseDistance.SafeNormalize(Vector2.UnitX);
                float recoil = direction.RotatedBy(-MathHelper.PiOver2 * 1.3f * Owner.direction).ToRotation();
                TargetRecoil = recoil;

                // Shooty ring
                Particle pulse = new DirectionalPulseRing(Owner.MountedCenter + direction * GunLength, Vector2.Zero, Color.DeepSkyBlue, new Vector2(0.5f, 1f), direction.ToRotation(), 0.2f, 1f, 30);
                GeneralParticleHandler.SpawnParticle(pulse);
                return;
            }

            // Apply recoil for the first 22 frames
            if (AITimer < 22)
            {
                float newRotation = UpdateAimPostShotRecoil(TargetRecoil.ToRotationVector2());
                Owner.itemRotation = newRotation;
            }

            // Then play a reload sound
            else if (AITimer == 22)
            {
                SoundEngine.PlaySound(SoundID.Item149, Projectile.Center);
            }

            // And finally move the gun back down as the reload sound plays
            else if (MPFBFireTimer - AITimer < 30)
            {
                float newRotation = UpdateAimPostShotRecoil((TargetRecoil + MathHelper.PiOver2 * 1.3f * Owner.direction).ToRotationVector2());
                Owner.itemRotation = newRotation;
            }

            if (AITimer >= MPFBFireTimer)
            {
                Projectile.Kill();
            }
        }

        // VIOLENTLY adjusts the aim vector of the cannon to point towards the target recoil
        // Then point back down as it reloads
        private float UpdateAimPostShotRecoil(Vector2 target) => Vector2.Lerp(target * Owner.direction, Owner.itemRotation.ToRotationVector2(), 0.795f).ToRotation();

        // AI for the left click
        public void UpdatePlasmaBeam()
        {
            // Randomize the scope starting rotation
            if (AITimer == 0)
            {
                SoundEngine.PlaySound(TheAnomalysNanogun.PlasmaChargeSFX, Projectile.Center);
                ScopeRotation = Main.rand.NextFloat(MathHelper.Pi * 0.5f, MathHelper.Pi * 0.75f) * Main.rand.NextBool().ToDirectionInt();
                ScopeScale = 2.5f;
            }

            // Set rotation and player direction
            Owner.itemRotation = Projectile.rotation;
            int dir = Math.Sign(Projectile.rotation.ToRotationVector2().X);
            Owner.ChangeDir(dir == 0 ? 1 : dir);
            Owner.itemRotation = ((Owner.Calamity().mouseWorld - Owner.MountedCenter) * Owner.direction).ToRotation();

            if (AITimer - PlasmaChargeupTimer > 0)
            {
                // Fire a laser at the correct moments
                if (AITimer % PlasmaShotCooldown == 0 && (AITimer - PlasmaChargeupTimer) / PlasmaShotCooldown <= PlasmaShotCount)
                {
                    Projectile.NewProjectile(new EntitySource_ItemUse_WithAmmo(Owner, Owner.HeldItem, -1),
                        Projectile.Center + Projectile.rotation.ToRotationVector2() * GunLength,
                        Projectile.rotation.ToRotationVector2(),
                        ModContent.ProjectileType<AnomalysNanogunPlasmaBeam>(),
                        Projectile.damage,
                        Projectile.knockBack,
                        Projectile.owner);

                    SoundEngine.PlaySound(TheAnomalysNanogun.PlasmaShotSFX, Projectile.Center);
                }
            }

            // Shrink the scope
            else
            {
                ScopeRotation = MathHelper.Lerp(ScopeRotation, 0f, 0.08f);
                ScopeScale -= 0.0625f;
            }

            if (AITimer >= PlasmaFireTimer)
                Projectile.Kill();
        }

        public override void PostDraw(Color lightColor)
        {
            if (!UseMPFBAI && AITimer - PlasmaChargeupTimer <= 0)
            {
                Vector2 drawCenter = (Projectile.Center + Projectile.rotation.ToRotationVector2() * GunLength) - Main.screenPosition;
                Main.EntitySpriteDraw(ScopeTexture, drawCenter, null, Color.White, ScopeRotation, ScopeTexture.Bounds.Size() / 2f, ScopeScale, SpriteEffects.None, 0);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;
    }
}
