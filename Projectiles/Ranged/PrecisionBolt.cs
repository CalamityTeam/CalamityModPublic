using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    public class PrecisionBolt : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        NPC potentialTarget = null;
        public override void SetDefaults()
        {
            Projectile.width = 72;
            Projectile.height = 72;
            Projectile.friendly = true;
            Projectile.timeLeft = 119;
            Projectile.penetrate = 1;
            Projectile.MaxUpdates = 2;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

        }
        public override string Texture => "CalamityMod/Projectiles/Ranged/PrecisionBolt";

        private Vector2 Recalibrate()
        {

            // Choose the angular turn speed of the bolt on recalibration.
            // This will overshoot significantly at first but will regress to finer movements as time goes on.
            // The exponent in the equation below serves to dampen the turn speed more quickly. It will not hit targets otherwise.
            float turnSpeedFactor = (float)Math.Pow(MathHelper.Clamp(Projectile.timeLeft - 40, 0f, 120f) / 120f, 4D);
            float turnAngle = MathHelper.ToRadians(turnSpeedFactor * 75f);

            // Select the velocity which has less of a disparity in terms of angular distance compared to the ideal direction.
            Vector2 leftTurnVelocity = Projectile.velocity.RotatedBy(-turnAngle);
            Vector2 righTurnVelocity = Projectile.velocity.RotatedBy(turnAngle);
            float leftDirectionImprecision = leftTurnVelocity.AngleBetween(Projectile.SafeDirectionTo(potentialTarget.Center));
            float rightDirectionImprecision = righTurnVelocity.AngleBetween(Projectile.SafeDirectionTo(potentialTarget.Center));
            potentialTarget = Projectile.Center.ClosestNPCAt(512f, true);

            if (leftDirectionImprecision < rightDirectionImprecision)
                return leftTurnVelocity;
            else
                return righTurnVelocity;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.LightSteelBlue.ToVector3());
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (potentialTarget == null) //(Re)target
                potentialTarget = Projectile.Center.ClosestNPCAt(512f, true);

            if (potentialTarget != null)
            {
                //Do some funny slight homing just in case
                float angularTurnSpeed = MathHelper.ToRadians(2.5f);
                float idealDirection = Projectile.AngleTo(potentialTarget.Center);
                float updatedDirection = Projectile.velocity.ToRotation().AngleTowards(idealDirection, angularTurnSpeed);
                Projectile.velocity = updatedDirection.ToRotationVector2() * Projectile.velocity.Length();

                if (Projectile.timeLeft % 6 == 0) // Do the STRONG homing
                {
                    SoundEngine.PlaySound(SoundID.Item93, Projectile.Center);
                    Projectile.velocity = Recalibrate();
                }
            }

            Dust trail = Dust.NewDustPerfect(Projectile.Center, 267); //Dust trail kinda poopy but idk how to make a cool trail :(
            trail.velocity = Vector2.Zero;
            trail.color = Color.Yellow;
            trail.scale = Main.rand.NextFloat(1f, 1.1f);
            trail.noGravity = true;

        }

        public override void OnKill(int timeLeft)
        {
            // Release a burst of magic dust on death.
            if (Main.dedServ)
                return;
            SoundEngine.PlaySound(SoundID.Item94, Projectile.Center);
            for (int i = 0; i < 10; i++)
            {
                Dust zap = Dust.NewDustPerfect(Projectile.Center, 267);
                zap.velocity = Projectile.velocity;
                zap.color = Color.Yellow;
                zap.scale = Main.rand.NextFloat(1f, 1.1f);
                zap.noGravity = true;
            }
        }
    }
}
