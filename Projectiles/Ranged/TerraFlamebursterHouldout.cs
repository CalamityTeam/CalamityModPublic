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
    public class TerraFlamebursterHoldout : ModProjectile
    {
        // Take the name and texture from the weapon
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<TerraFlameburster>();
        public override string Texture => "CalamityMod/Items/Weapons/Ranged/TerraFlameburster";
        private Player Owner => Main.player[Projectile.owner];
        private ref float ShotsFired => ref Projectile.ai[1];
        private ref float ShootTimer => ref Projectile.ai[2];
        private ref int ShotDelay => ref Owner.ActiveItem().useTime; // 8
        public int ShotCooldown;
        public int FireBlobs = 0;
        public int Time = 0;
        private bool OwnerCanShoot => Owner.channel && !Owner.noItems && !Owner.CCed;

        public override void SetDefaults()
        {
            Projectile.width = 68;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Time++;
            if (Time == 1)
                Projectile.alpha = 255;
            else
                Projectile.alpha = 0;

            if (Owner.dead || ShotsFired >= 16) // destroy the holdout if the player dies or is out of shots
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
                // While channeled, keep refreshing the projectile lifespan
                Projectile.timeLeft = 2;

                ShootTimer++;
                if (ShootTimer >= 60)
                {
                    if (ShotCooldown == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item34, Projectile.Center);
                        Owner.PickAmmo(Owner.ActiveItem(), out _, out float shootSpeed, out int damage, out float knockback, out _, Main.rand.NextBool(2) ? true : false);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), tipPosition, (Projectile.velocity * 9).RotatedByRandom(0.08f), ModContent.ProjectileType<TerraFire>(), damage, knockback, Projectile.owner);
                        ShotsFired++;
                        ShotCooldown = ShotDelay;
                        if (FireBlobs == 0)
                        {
                            float randAngle = Main.rand.NextFloat(8f, 15f);
                            Vector2 newVel = (Projectile.velocity * 9).RotatedBy(MathHelper.ToRadians(randAngle)) * 2f;
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), tipPosition, newVel, ModContent.ProjectileType<TerraFlare>(), damage, knockback, Projectile.owner);
                            newVel = (Projectile.velocity * 9).RotatedBy(MathHelper.ToRadians(-randAngle)) * 2f;
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), tipPosition, newVel, ModContent.ProjectileType<TerraFlare>(), damage, knockback, Projectile.owner);
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
                        float rotMulti = Main.rand.NextFloat(0.3f, 1f);
                        Dust dust2 = Dust.NewDustPerfect(tipPosition, Main.rand.NextBool(5) ? 135 : 107);
                        dust2.noGravity = true;
                        dust2.velocity = new Vector2(0, -2).RotatedByRandom(rotMulti * 0.3f) * (Main.rand.NextFloat(1f, 2.9f) - rotMulti);
                        dust2.scale = Main.rand.NextFloat(1.2f, 1.8f) * (ShootTimer * 0.015f);
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
                float interpolant = Utils.GetLerpValue(5f, 40f, Projectile.Distance(Main.MouseWorld), true);
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
