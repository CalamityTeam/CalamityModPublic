using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static CalamityMod.Items.Weapons.Ranged.Photoviscerator;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Projectiles.Ranged
{
    public class PhotovisceratorHoldout : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Photoviscerator>();
        public Player Owner => Main.player[Projectile.owner];
        public bool OwnerCanShoot => (Owner.channel || Owner.Calamity().mouseRight) && !Owner.noItems && !Owner.CCed;
        public ref float ShootTimer => ref Projectile.ai[0];
        public ref float ForcedLifespan => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = 170;
            Projectile.height = 66;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            Vector2 armPosition = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            UpdateProjectileHeldVariables(armPosition);
            ManipulatePlayerVariables();

            // Orange exo energy as the back "flames"
            Color energyColor = Color.Orange;
            Vector2 flamePosition = armPosition + Projectile.velocity * Projectile.width * 0.12f;
            Vector2 verticalOffset = Vector2.UnitY.RotatedBy(Projectile.rotation);
            if (Math.Cos(Projectile.rotation) < 0f)
                verticalOffset *= -1f;

            if (Main.rand.NextBool(4))
            {
                Vector2 flameAngle = -Vector2.UnitY.RotatedBy(Projectile.rotation + MathHelper.ToRadians(Main.rand.NextFloat(270f, 300f) * Projectile.spriteDirection));
                SquishyLightParticle exoEnergy = new(flamePosition - verticalOffset * 26f, flameAngle * Main.rand.NextFloat(0.4f, 1.6f), 0.25f, energyColor, 20);
                GeneralParticleHandler.SpawnParticle(exoEnergy);
                SquishyLightParticle exoEnergy2 = new(armPosition - verticalOffset * 22f, flameAngle * Main.rand.NextFloat(0.3f, 1.2f), 0.2f, energyColor, 12);
                GeneralParticleHandler.SpawnParticle(exoEnergy2);
            }

            // Create light around the crystal
            Lighting.AddLight(armPosition + Projectile.velocity * 42f - verticalOffset * 10f, 0.8f, 0.8f, 0.8f);
            Lighting.AddLight(armPosition + Projectile.velocity * 96f + verticalOffset * 6f, 0.4f, 0.4f, 0.4f);

            // Despawn the projectile if the player can't shoot
            // Any forced lifespan (triggered by attacking right click to prevent click spamming) will have priority
            if (!OwnerCanShoot)
            {
                ForcedLifespan--;
                if (ForcedLifespan <= 0f)
                    Projectile.Kill();
                return;
            }

            // Don't try to perform attacks if it's not the holder
            if (Projectile.owner != Main.myPlayer)
                return;

            // Immediately killed if ammo is out            
            if (!Owner.HasAmmo(Owner.ActiveItem()))
            {
                Projectile.Kill();
                return;
            }

            // Right-click attacks
            if (Owner.Calamity().mouseRight && !Owner.channel)
            {
                // Can't shoot on frame 1 as it can't use ammo yet
                if (ShootTimer < 0f)
                    RightClickAttack(armPosition, verticalOffset);

                ShootTimer--;
            }
            else if (!Owner.Calamity().mouseRight && Owner.channel)
            {
                ShootTimer++;
                LeftClickAttack(armPosition, verticalOffset);
            }
        }

        public void LeftClickAttack(Vector2 armPosition, Vector2 verticalOffset)
        {
            // Consume ammo and retrieve projectile stats; has a chance to not consume ammo
            Owner.PickAmmo(Owner.ActiveItem(), out _, out float shootSpeed, out int damage, out float knockback, out _, Main.rand.NextFloat() <= AmmoNotConsumeChance);

            var source = Projectile.GetSource_FromThis();
            Vector2 position = armPosition + Projectile.velocity * 48f - verticalOffset * 10f;
            Vector2 velocity = Projectile.velocity * shootSpeed;

            // Main fire stream
            SoundEngine.PlaySound(SoundID.Item34, Owner.MountedCenter);
            Projectile.NewProjectile(source, position, velocity, ProjectileType<ExoFire>(), damage, knockback, Projectile.owner, Main.rand.NextFloat(0f, 3f));

            // Shoots light bombs every once in a while, rate of which equals to the item's use time
            if (ShootTimer >= Owner.ActiveItem().useTime)
            {
                ShootTimer = 0f;

                for (int i = 0; i < 2; i++)
                {
                    Vector2 bombPos = armPosition + Projectile.velocity * 108f + verticalOffset * 6f;
                    int yDirection = (i == 0).ToDirectionInt();
                    Vector2 bombVel = velocity.RotatedBy(0.2f * yDirection);

                    Projectile.NewProjectile(source, bombPos, bombVel, ProjectileType<ExoLight>(), damage, knockback, Projectile.owner, yDirection);
                }
            }
        }

        public void RightClickAttack(Vector2 armPosition, Vector2 verticalOffset)
        {
            // Multiplied by the ratio of attack speed gained from modifiers
            ShootTimer = (RightClickCooldown * Owner.ActiveItem().useTime / (float)LightBombCooldown) - 1f;
            ForcedLifespan = ShootTimer;

            // Consume ammo and retrieve projectile stats
            Owner.PickAmmo(Owner.ActiveItem(), out _, out float shootSpeed, out int damage, out float knockback, out _);

            var source = Projectile.GetSource_FromThis();
            Vector2 position = armPosition + Projectile.velocity * 140f + verticalOffset * 6f;
            Vector2 velocity = Projectile.velocity * shootSpeed * RightClickVelocityMult;

            SoundEngine.PlaySound(SoundID.Item34, Owner.MountedCenter);
            Projectile.NewProjectile(source, position, velocity, ProjectileType<ExoFlareCluster>(), damage, knockback, Projectile.owner);
        }

        public void UpdateProjectileHeldVariables(Vector2 armPosition)
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

            Projectile.position = armPosition - Projectile.Size * 0.5f + Projectile.velocity.SafeNormalize(Vector2.UnitY) * 28f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.spriteDirection == -1)
                Projectile.rotation += MathHelper.Pi;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
        }

        public void ManipulatePlayerVariables()
        {
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? CanDamage() => false;
    }
}
