using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class KingsbaneHoldout : ModProjectile
    {
        // Take the name and texture from the weapon
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Kingsbane>();
        private bool OwnerCanShoot => Owner.channel && !Owner.noItems && !Owner.CCed;
        public override string Texture => "CalamityMod/Projectiles/Ranged/KingsbaneWindUp";
        private Player Owner => Main.player[Projectile.owner];

        public int Time = 0;
        public int revTimer = 0; //revving timer
        public int framesBetweenShots = 0;
        public bool fullRev = false;
        public int fullRevShots = 50;
        public int windupAnim = 11;
        public int soundTimer = 0;

        public override void SetDefaults()
        {
            Projectile.width = 112;
            Projectile.height = 44;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
        }
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 10;
        }
        public override void AI()
        {
            Time++;
            if (Time % 2 == 0)
                soundTimer++;
            Projectile.frameCounter++;

            if (Projectile.frameCounter > windupAnim && OwnerCanShoot)
            {
                if (Projectile.frame == 1 && Time < 85)
                {
                    Projectile.frame = 0;
                }
                else
                    Projectile.frame++;
                if (windupAnim > 0)
                    windupAnim--;
                Projectile.frameCounter = 0;
            }
            else if (!OwnerCanShoot)
            {
                Projectile.frame++;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 2;
            }
            if (Owner.dead) // destroy the holdout if the player dies
            {
                Projectile.Kill();
                return;
            }

            Vector2 armPosition = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            Vector2 tipPosition = armPosition + Projectile.velocity * Projectile.width * 0.85f + new Vector2 (0, 3.8f);
            Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 15;

            int bulletAMMO = ProjectileID.Bullet;
            Owner.PickAmmo(Owner.ActiveItem(), out bulletAMMO, out float SpeedNoUse, out int bulletDamage, out float kBackNoUse, out int _);

            // Fire Auric Bullets if the owner stops channeling or otherwise cannot use the weapon.
            if (!OwnerCanShoot)
            {
                if (fullRev && fullRevShots > 0)
                {
                    Projectile.timeLeft = 2;
                    Dust dust2 = Dust.NewDustPerfect(tipPosition - Projectile.velocity * 68, 87, Projectile.velocity.RotatedBy((8.6f * Main.rand.NextFloat(0.975f, 1.025f)) * -Projectile.direction) * Main.rand.NextFloat(5.5f, 7f) + Owner.velocity * 0.5f);
                    dust2.noGravity = false;
                    dust2.scale = Main.rand.NextFloat(0.8f, 0.9f);
                    Dust dust3 = Dust.NewDustPerfect(tipPosition - Projectile.velocity * 5, Main.rand.NextBool(4) ? 169 : 162, (Projectile.velocity * Main.rand.NextFloat(4f, 15.5f)).RotatedByRandom(0.3f));
                    dust3.noGravity = true;
                    dust3.scale = Main.rand.NextFloat(1.3f, 2.2f);
                    Owner.Calamity().GeneralScreenShakePower = 1.85f;
                    //recoil
                    Owner.velocity += -Projectile.velocity * fullRevShots * (Main.zenithWorld ? 0.028f : 0.013f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), tipPosition + Projectile.velocity * 5 + Main.rand.NextVector2Circular(7, 7), shootVelocity.RotatedByRandom(MathHelper.ToRadians(4f)), ModContent.ProjectileType<AuricBullet>(), (int)(Projectile.damage * 0.9f), Projectile.knockBack, Projectile.owner);
                    SoundEngine.PlaySound(SoundID.Item40 with { PitchVariance = 0.4f }, Projectile.Center);
                    //SoundEngine.PlaySound(Kingsbane.AuricFire with { PitchVariance = 0.4f }, Projectile.Center);
                    fullRevShots--;
                }
            }
            else
            {
                if (Time < 90 && soundTimer > (windupAnim + 2))
                {
                    SoundEngine.PlaySound(SoundID.Item23 with { Pitch = (8 - windupAnim) * 0.15f }, Projectile.Center);
                    soundTimer = 0;
                }
                // While channeled, keep refreshing the projectile lifespan
                Projectile.timeLeft = 2;
                if (Time > 90)
                {
                    fullRev = true;
                    if (framesBetweenShots == 0)
                    {
                        Dust dust2 = Dust.NewDustPerfect(tipPosition - Projectile.velocity * 68, 87, Projectile.velocity.RotatedBy((8.6f * Main.rand.NextFloat(0.985f, 1.015f)) * -Projectile.direction) * Main.rand.NextFloat(4, 5) + Owner.velocity * 0.5f);
                        dust2.noGravity = false;
                        dust2.scale = Main.rand.NextFloat(0.8f, 0.9f);
                        for (int i = 0; i <= 2; i++)
                        {
                            Dust dust3 = Dust.NewDustPerfect(tipPosition - Projectile.velocity * 6, Main.rand.NextBool(3) ? 263 : 247, (Projectile.velocity * Main.rand.NextFloat(4f, 15.5f)).RotatedByRandom(0.2f));
                            dust3.noGravity = true;
                            dust3.scale = Main.rand.NextFloat(0.9f, 1.6f);
                        }
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, shootVelocity.RotatedByRandom(MathHelper.ToRadians(1.5f)), bulletAMMO, Projectile.damage, Projectile.knockBack, Projectile.owner);
                        SoundEngine.PlaySound(SoundID.Item41 with { Volume = 0.75f}, Projectile.Center);
                        framesBetweenShots = 3;
                    }
                    if (framesBetweenShots > 0)
                        framesBetweenShots--;
                }
                
            }
            UpdateProjectileHeldVariables(armPosition);
            ManipulatePlayerVariables();
        }

        private void UpdateProjectileHeldVariables(Vector2 armPosition)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                float interpolant = Utils.GetLerpValue(5f, 55f, Projectile.Distance(Main.MouseWorld), true);
                Vector2 oldVelocity = Projectile.velocity;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(Main.MouseWorld), interpolant);
                if (Projectile.velocity != oldVelocity)
                {
                    Projectile.netSpam = 0;
                    Projectile.netUpdate = true;
                }
            }
            Projectile.Center = armPosition + Projectile.velocity * MathHelper.Clamp(47f - (framesBetweenShots * 2), 0f, 47f) + new Vector2 (0, 5);
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
            Projectile.spriteDirection = Projectile.direction;

            // Rumble
            if (!OwnerCanShoot)
            {
                Projectile.position += Main.rand.NextVector2Circular(4.5f, 4.5f);
            }
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
