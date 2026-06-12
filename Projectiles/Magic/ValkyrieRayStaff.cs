using System;
using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class ValkyrieRayStaff : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Items/Weapons/Magic/ValkyrieRay";

        private const float AimResponsiveness = 0.66f;
        public ref float Timer => ref Projectile.ai[0];
        public ref float Fired => ref Projectile.ai[1];
        public ref float TimeRate => ref Projectile.ai[2];

        public override void SetDefaults()
        {
            Projectile.width = 54;
            Projectile.height = 52;
            // This projectile has no hitboxes and no damage type.
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 900;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 rrp = player.RotatedRelativePoint(player.MountedCenter, true);

            // Calculate how quickly the staff should charge. Charge increases by some number close to 1 every frame.
            // Speed increasing reforges make this number greater than 1. Slowing reforges make it smaller than 1.
            if (TimeRate == 0f)
                TimeRate = MathF.Round((float)(ValkyrieRay.ChargeFrames + ValkyrieRay.CooldownFrames) / player.HeldItem.useTime, 3);

            // Increment the timer for the staff. If the timer has passed the total time, destroy it.
            Timer += TimeRate;
            int maxTime = ValkyrieRay.ChargeFrames + ValkyrieRay.CooldownFrames;
            if (Timer > maxTime)
            {
                Projectile.Kill();
                return;
            }
            // Compute the weapon's charge.
            float chargeLevel = MathHelper.Clamp(Timer / ValkyrieRay.ChargeFrames, 0f, 1f);

            // Common code among holdouts to keep the holdout projectile directly in the player's hand
            UpdatePlayerVisuals(player, rrp);

            // Compute the gem position, which is needed for visual effects
            float angle = Projectile.rotation - MathHelper.PiOver2;
            Vector2 gemOffset = Vector2.One * ValkyrieRay.GemDistance * 1.4142f; // distance to gem on staff
            Vector2 gemPos = Projectile.Center + gemOffset.RotatedBy(angle);

            // Firing or charging?
            if (chargeLevel >= 1f && Fired == 0f)
            {
                Fired = 1f; // so it never fires again
                FiringEffects(gemPos);
                if (Projectile.owner == Main.myPlayer)
                {
                    Projectile laser = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), gemPos, Projectile.velocity, ModContent.ProjectileType<ValkyrieRayBeam>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    laser.Center = gemPos;
                }
            }
            else if (Fired == 0f)
            {
                // The player can constantly re-aim the staff while it's charging, but once it fires it is locked in place.
                UpdateAim(rrp, Projectile.velocity.Length());
                ChargingEffects(gemPos, chargeLevel);
            }
        }

        private void UpdatePlayerVisuals(Player player, Vector2 rrp)
        {
            // Place the projectile directly into the player's hand at all times
            Projectile.Center = rrp;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            // The staff is a holdout projectile, so change the player's variables to reflect that
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            // Multiplying by projectile.direction is required due to vanilla spaghetti.
            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
        }

        // Adjusts the aim vector of the staff to point towards the mouse. This is Last Prism code.
        private void UpdateAim(Vector2 source, float speed)
        {
            // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
            Vector2 aimVector = Vector2.Normalize(Main.MouseWorld - source);
            if (aimVector.HasNaNs())
                aimVector = -Vector2.UnitY;
            aimVector = Vector2.Normalize(Vector2.Lerp(aimVector, Vector2.Normalize(Projectile.velocity), AimResponsiveness));
            aimVector *= 30f;

            if (aimVector != Projectile.velocity)
                Projectile.netUpdate = true;
            Projectile.velocity = aimVector;
        }

        private void ChargingEffects(Vector2 center, float chargeLevel)
        {
            Lighting.AddLight(center, ValkyrieRay.LightColor.ToVector3() * chargeLevel);

            int numDust = 2;
            int dustID = 73;
            float incomingRadius = 9f;
            for (int i = 0; i < numDust; ++i)
            {
                Vector2 offsetUnit = Main.rand.NextVector2Unit();
                Vector2 dustPos = center + offsetUnit * incomingRadius;
                Dust d = Dust.NewDustDirect(dustPos, 0, 0, dustID, 0f, 0f);
                d.velocity = offsetUnit * -Main.rand.NextFloat(2f, 3.5f);
                d.scale = Main.rand.NextFloat(0.4f, 1f);
                d.noGravity = true;
            }
        }

        private void FiringEffects(Vector2 center)
        {
            SoundEngine.PlaySound(SoundID.Item28 with { Volume = 0.7f }, center);
            SoundEngine.PlaySound(SoundID.Item60 with { Volume = 0.7f }, center);
            int numDust = 36;
            int dustID = 73;
            for (int i = 0; i < numDust; ++i)
            {
                Dust d = Dust.NewDustDirect(center, 0, 0, dustID, 0f, 0f);
                d.velocity = (i * MathHelper.TwoPi / numDust).ToRotationVector2() * 2.2f;
                d.scale = 1.4f;
                d.noGravity = true;
            }
        }
    }
}
