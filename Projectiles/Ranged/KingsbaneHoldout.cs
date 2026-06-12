using CalamityMod.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
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
        public override string Texture => "CalamityMod/Projectiles/Ranged/KingsbaneWindUp";
        private Player Owner => Main.player[Projectile.owner];

        public int Time = 0;
        public int revTimer = 0; //revving timer
        public int framesBetweenShots = 0;
        public bool fullRev = false;
        public int fullRevShots = 50;
        public int windupAnim = 11;
        public int soundTimer = 0;
        public bool discharging = false;

        public override void SetDefaults()
        {
            Projectile.width = 112;
            Projectile.height = 44;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
        }
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 10;
        }
        public override void AI()
        {
            Time++;

            if (Time == 3)
                Projectile.alpha = 0;

            if (Time % 2 == 0)
                soundTimer++;
            Projectile.frameCounter++;

            if (Projectile.frameCounter > windupAnim && !Owner.CantUseHoldout())
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
            else if (Owner.CantUseHoldout())
            {
                Projectile.frame++;
            }
            if (Projectile.frame >= Main.projFrames[Type])
            {
                Projectile.frame = 2;
            }
            if (Owner.dead) // destroy the holdout if the player dies
            {
                Projectile.Kill();
                return;
            }

            Vector2 armPosition = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            Vector2 tipPosition = armPosition + Projectile.velocity * Projectile.width * 0.85f + new Vector2(0, 3.8f);
            Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 15;

            int bulletAMMO = ProjectileID.Bullet;
            Owner.PickAmmo(Owner.HeldItem, out bulletAMMO, out float SpeedNoUse, out int bulletDamage, out float kBackNoUse, out int _);

            // Fire Auric Bullets if the owner stops channeling or otherwise cannot use the weapon.
            if (Owner.CantUseHoldout() || discharging)
            {
                discharging = true;
                if (fullRev && fullRevShots > 0)
                {
                    Projectile.timeLeft = 2;
                    Dust dust2 = Dust.NewDustPerfect(tipPosition - Projectile.velocity * 68, DustID.GemTopaz, Projectile.velocity.RotatedBy((8.6f * Main.rand.NextFloat(0.975f, 1.025f)) * -Projectile.direction) * Main.rand.NextFloat(5.5f, 7f) + Owner.velocity * 0.5f);
                    dust2.noGravity = false;
                    dust2.scale = Main.rand.NextFloat(0.8f, 0.9f);
                    Dust dust3 = Dust.NewDustPerfect(tipPosition - Projectile.velocity * 5, Main.rand.NextBool(4) ? 169 : 162, (Projectile.velocity * Main.rand.NextFloat(4f, 15.5f)).RotatedByRandom(0.3f));
                    dust3.noGravity = true;
                    dust3.scale = Main.rand.NextFloat(1.3f, 2.2f);
                    Owner.SetScreenshake(1.85f);
                    //recoil
                    Owner.velocity += -Projectile.velocity * fullRevShots * (Main.zenithWorld ? 0.028f : 0.013f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), tipPosition + Projectile.velocity * 5 + Main.rand.NextVector2Circular(7, 7), shootVelocity.RotatedByRandom(MathHelper.ToRadians(4f)), ModContent.ProjectileType<AuricBullet>(), (int)(Projectile.damage), Projectile.knockBack, Projectile.owner);
                    SoundStyle fire = new("CalamityMod/Sounds/Item/GunShotSmall");
                    if (fullRevShots % 2 == 0)
                        SoundEngine.PlaySound(fire with { Volume = 0.7f }, Projectile.Center);
                    //SoundEngine.PlaySound(SoundID.Item40 with { PitchVariance = 0.4f }, Projectile.Center);
                    Owner.channel = true;

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
                        Dust dust2 = Dust.NewDustPerfect(tipPosition - Projectile.velocity * 68, DustID.GemTopaz, Projectile.velocity.RotatedBy((8.6f * Main.rand.NextFloat(0.985f, 1.015f)) * -Projectile.direction) * Main.rand.NextFloat(4, 5) + Owner.velocity * 0.5f);
                        dust2.noGravity = false;
                        dust2.scale = Main.rand.NextFloat(0.8f, 0.9f);
                        for (int i = 0; i <= 2; i++)
                        {
                            Dust dust3 = Dust.NewDustPerfect(tipPosition - Projectile.velocity * 6, Main.rand.NextBool(3) ? 263 : 247, (Projectile.velocity * Main.rand.NextFloat(4f, 15.5f)).RotatedByRandom(0.2f));
                            dust3.noGravity = true;
                            dust3.scale = Main.rand.NextFloat(0.9f, 1.6f);
                        }
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, shootVelocity.RotatedByRandom(MathHelper.ToRadians(1.5f)), bulletAMMO, Projectile.damage, Projectile.knockBack, Projectile.owner);
                        SoundStyle fire = new("CalamityMod/Sounds/Item/GunShotMid");
                        SoundEngine.PlaySound(fire with { Volume = 0.4f }, Projectile.Center);
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
                // 15NOV2024: Ozzatron: clamped mouse position unnecessary, this is only used to aim the Kingsbane itself

                float interpolant = Utils.GetLerpValue(0f, 55f, Owner.Distance(Main.MouseWorld), true);
                Vector2 oldVelocity = Projectile.velocity;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(Main.MouseWorld), (discharging ? 0.2f : 0.45f)).SafeNormalize(Vector2.UnitY);
                if (Projectile.velocity != oldVelocity)
                    Projectile.ForceNetUpdate();
            }
            Projectile.Center = armPosition + Projectile.velocity * MathHelper.Clamp(47f - (framesBetweenShots * 2), 0f, 47f) + new Vector2(0, 5);
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
            Projectile.spriteDirection = Projectile.direction;

            // Rumble
            if (Owner.CantUseHoldout() || discharging)
            {
                Projectile.Center += Main.rand.NextVector2Circular(4.5f, 4.5f);
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
