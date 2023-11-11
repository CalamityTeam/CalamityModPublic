using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using log4net.Core;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class ElementalEruptionHoldout : ModProjectile
    {
        // Take the name and texture from the weapon
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<ElementalEruption>();
        public override string Texture => "CalamityMod/Items/Weapons/Ranged/ElementalEruption";
        private Player Owner => Main.player[Projectile.owner];
        private ref float ShotsFired => ref Projectile.ai[1];
        private ref float ShootTimer => ref Projectile.ai[2];
        private ref int ShotDelay => ref Owner.ActiveItem().useTime; // 6
        public int ShotCooldown;
        public int FireBlobs = 0;
        private bool OwnerCanShoot => Owner.channel && !Owner.noItems && !Owner.CCed;

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (Owner.dead || ShotsFired >= 24) // destroy the holdout if the player dies or is out of shots
            {
                Projectile.Kill();
                return;
            }

            Vector2 armPosition = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            Vector2 tipPosition = armPosition + Projectile.velocity * Projectile.width * 0.8f;

            // Kill if the owner stops channeling or otherwise cannot use the weapon.
            if (!OwnerCanShoot)
            {
                Projectile.Kill();
            }
            else
            {
                var effectcolor = Main.rand.Next(4) switch
                {
                    0 => Color.DeepSkyBlue,
                    1 => Color.MediumSpringGreen,
                    2 => Color.DarkOrange,
                    _ => Color.Violet,
                };

                // While channeled, keep refreshing the projectile lifespan
                Projectile.timeLeft = 2;

                ShootTimer++;
                if (ShootTimer >= 60)
                {
                    if (ShotCooldown == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item34, Projectile.Center);
                        Owner.PickAmmo(Owner.ActiveItem(), out _, out float shootSpeed, out int damage, out float knockback, out _, Main.rand.NextFloat() < 0.70f);
                        for (int i = 0; i < 2; i++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), tipPosition, (Projectile.velocity * 10).RotatedByRandom(0.12f), ModContent.ProjectileType<ElementalFire>(), damage, knockback, Projectile.owner);
                        }
                        ShotsFired++;
                        ShotCooldown = ShotDelay;
                        if (FireBlobs == 0)
                        {
                            Vector2 newVel = (Projectile.velocity * 9);
                            Vector2 newPos = tipPosition + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 36f;
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), newPos, newVel, ModContent.ProjectileType<ElementalFlare>(), damage, knockback, Projectile.owner, newVel.Length(), -1f);
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), newPos, newVel, ModContent.ProjectileType<ElementalFlare>(), damage, knockback, Projectile.owner, newVel.Length(), 1f);
                            FireBlobs = 3;
                        }
                        else
                            FireBlobs--;
                    }
                    else
                        ShotCooldown--;
                }
                else
                {
                    if (ShootTimer == 1)
                        SoundEngine.PlaySound(SoundID.Item73 with { Volume = 0.7f}, Projectile.Center);
                    ShotsFired = 0;
                    for (int i = 0; i < 2; i++)
                    {
                        int dustType = Main.rand.NextBool() ? 66 : 247;
                        float rotMulti = Main.rand.NextFloat(0.3f, 1f);
                        Dust dust = Dust.NewDustPerfect(tipPosition, dustType);
                        dust.scale = Main.rand.NextFloat(1.2f, 1.8f) * (ShootTimer * 0.025f) - rotMulti * 0.1f;
                        dust.noGravity = true;
                        dust.velocity = new Vector2(0, -2).RotatedByRandom(rotMulti * 0.3f) * (Main.rand.NextFloat(1f, 3.2f) - rotMulti) * (ShootTimer * 0.025f);
                        dust.alpha = Main.rand.Next(90, 150);
                        dust.color = effectcolor;
                    }
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
            }
            Projectile.Center = armPosition + Projectile.velocity * 20;
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
            Projectile.spriteDirection = Projectile.direction;
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
