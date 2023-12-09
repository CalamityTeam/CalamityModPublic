using System;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
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
        public Color sparkColor;
        public int Time = 0;
        public ref int PhotoTimer => ref Main.player[Projectile.owner].Calamity().PhotoTimer;
        public SlotId PhotoUseSound;

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
            Time++;
            if (PhotoTimer > 0)
                PhotoTimer--;

            if (Time == 1)
                Projectile.alpha = 255;
            else
                Projectile.alpha = 0;

            sparkColor = Main.rand.Next(4) switch
            {
                0 => Color.Red,
                1 => Color.MediumTurquoise,
                2 => Color.Orange,
                _ => Color.LawnGreen,
            };

            if (Owner.Calamity().mouseRight)
            {
                if (SoundEngine.TryGetActiveSound(PhotoUseSound, out var Sound2))
                    Sound2?.Stop();
            }

            if (SoundEngine.TryGetActiveSound(PhotoUseSound, out var Sound) && Sound.IsPlaying)
                Sound.Position = Projectile.Center;

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
                SquishyLightParticle exoEnergy = new(flamePosition - verticalOffset * 26f, flameAngle * Main.rand.NextFloat(0.8f, 3.6f), 0.25f, energyColor, 20);
                GeneralParticleHandler.SpawnParticle(exoEnergy);
                SquishyLightParticle exoEnergy2 = new(armPosition - verticalOffset * 22f, flameAngle * Main.rand.NextFloat(0.7f, 3.2f), 0.2f, energyColor, 12);
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
            if (Time == 1 || Time % 55 == 0)
            {
                PhotoUseSound = SoundEngine.PlaySound(Photoviscerator.UseSound, Projectile.Center);
            }

            if (SoundEngine.TryGetActiveSound(PhotoUseSound, out var Sound3))
                Sound3.Pitch = 0 - (PhotoTimer * 0.002f);

            // Consume ammo and retrieve projectile stats; has a chance to not consume ammo
            Owner.PickAmmo(Owner.ActiveItem(), out _, out float shootSpeed, out int damage, out float knockback, out _, Main.rand.NextFloat() <= AmmoNotConsumeChance);

            var source = Projectile.GetSource_FromThis();
            Vector2 position = armPosition + Projectile.velocity * 55f - verticalOffset * 10f;
            Vector2 velocity = Projectile.velocity * shootSpeed;
            
            if (PhotoTimer == 1)
            {
                for (int i = 0; i < 30; i++)
                {
                    sparkColor = Main.rand.Next(4) switch
                    {
                        0 => Color.Red,
                        1 => Color.MediumTurquoise,
                        2 => Color.Orange,
                        _ => Color.LawnGreen,
                    };
                    SquishyLightParticle exoEnergy = new(position, (Projectile.velocity * 3).RotatedByRandom(0.4f) * Main.rand.NextFloat(0.3f, 1.6f), 0.9f, sparkColor, 60);
                    GeneralParticleHandler.SpawnParticle(exoEnergy);
                }
                SoundEngine.PlaySound(DeadSunsWind.Shoot with { Volume = 1.9f}, Owner.MountedCenter);
            }

            Dust dust = Dust.NewDustPerfect(position, 263, (Projectile.velocity * 10).RotatedByRandom(0.6f) * Main.rand.NextFloat(0.3f, 1.6f));
            dust.noGravity = true;
            dust.scale = Main.rand.NextFloat(1.3f, 1.8f) - PhotoTimer * 0.02f;
            dust.color = sparkColor;
            Dust dust2 = Dust.NewDustPerfect(position, 263, (Projectile.velocity * 15).RotatedByRandom(0.25f) * Main.rand.NextFloat(0.3f, 1.6f));
            dust2.noGravity = true;
            dust2.scale = Main.rand.NextFloat(1.3f, 1.8f) - PhotoTimer * 0.02f;
            dust2.color = sparkColor;
            if (Main.rand.NextBool())
            {
                MediumMistParticle smoke = new MediumMistParticle(position + Main.rand.NextVector2Circular(5, 5), (Projectile.velocity * 15).RotatedByRandom(0.15f) * Main.rand.NextFloat(0.5f, 2.1f), sparkColor, Color.White, Main.rand.NextFloat(1.8f, 2.9f) - PhotoTimer * 0.026f, 160, Main.rand.NextFloat(-3f, 3f));
                GeneralParticleHandler.SpawnParticle(smoke);
                MediumMistParticle smoke2 = new MediumMistParticle(position + Main.rand.NextVector2Circular(5, 5), (Projectile.velocity * 18).RotatedByRandom(0.05f) * Main.rand.NextFloat(1.8f, 3.1f), sparkColor, Color.White, Main.rand.NextFloat(0.8f, 1.9f) - PhotoTimer * 0.02f, 160, Main.rand.NextFloat(-3f, 3f));
                GeneralParticleHandler.SpawnParticle(smoke2);
            }

            // Main fire stream
            Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.005f), ProjectileType<ExoFire>(), (int)(damage * (1 - PhotoTimer / 59.9f)), knockback, Projectile.owner, Main.rand.NextFloat(0f, 3f));

            // Shoots light bombs every once in a while, rate of which equals to the item's use time
            if (ShootTimer >= Owner.ActiveItem().useTime * 10 && PhotoTimer == 0)
            {
                ShootTimer = 0f;

                for (int i = 0; i < 2; i++)
                {
                    Vector2 bombPos = armPosition + Projectile.velocity * 108f + verticalOffset * 6f;
                    int yDirection = (i == 0).ToDirectionInt();
                    Vector2 bombVel = velocity.RotatedBy(0.2f * yDirection);

                    Projectile.NewProjectile(source, bombPos, bombVel, ProjectileType<ExoLight>(), damage, knockback, Projectile.owner, yDirection);
                }
                SoundEngine.PlaySound(HalleysInferno.Hit with { Volume = 0.5f }, Owner.MountedCenter);
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
            Vector2 position = armPosition + Projectile.velocity * 55f - verticalOffset * 10f;
            Vector2 velocity = Projectile.velocity * shootSpeed * RightClickVelocityMult;
            for (int i = 0; i <= 15; i++)
            {
                sparkColor = Main.rand.Next(4) switch
                {
                    0 => Color.Red,
                    1 => Color.MediumTurquoise,
                    2 => Color.Orange,
                    _ => Color.LawnGreen,
                };
                DirectionalPulseRing pulse = new DirectionalPulseRing(position, (Projectile.velocity * 10).RotatedByRandom(0.6f) * Main.rand.NextFloat(0.3f, 1.6f), sparkColor, new Vector2(1, 1), 0, Main.rand.NextFloat(0.2f, 0.35f), 0f, 40);
                GeneralParticleHandler.SpawnParticle(pulse);
                DirectionalPulseRing pulse2 = new DirectionalPulseRing(position, (Projectile.velocity * 10).RotatedByRandom(0.1f) * Main.rand.NextFloat(0.8f, 3.1f), sparkColor, new Vector2(1, 1), 0, Main.rand.NextFloat(0.2f, 0.35f), 0f, 40);
                GeneralParticleHandler.SpawnParticle(pulse2);
                Dust dust = Dust.NewDustPerfect(position, 263, (Projectile.velocity * 10).RotatedByRandom(0.6f) * Main.rand.NextFloat(0.3f, 1.6f));
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(1.3f, 1.8f);
                dust.color = sparkColor;
            }
            SoundEngine.PlaySound(HalleysInferno.Shoot with { Volume = 0.4f } , Owner.MountedCenter);

            int rightClickDamage = (int)(0.5f * damage);
            Projectile.NewProjectile(source, position, velocity, ProjectileType<ExoFlareCluster>(), rightClickDamage, knockback, Projectile.owner);
        }

        public void UpdateProjectileHeldVariables(Vector2 armPosition)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                float interpolant = Utils.GetLerpValue(5f, 90f, Projectile.Distance(Main.MouseWorld), true);
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
        public override void OnKill(int timeLeft)
        {
            if (SoundEngine.TryGetActiveSound(PhotoUseSound, out var Sound))
                Sound?.Stop();
            PhotoTimer = 90;
        }
    }
}
