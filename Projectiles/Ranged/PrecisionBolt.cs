using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class PrecisionBolt : ModProjectile
    {
        NPC potentialTarget = null;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Precision Bolt");
        }

        public override void SetDefaults()
        {
            projectile.width = 72;
            projectile.height = 72;
            projectile.friendly = true;
            projectile.timeLeft = 119;
            projectile.penetrate = 1;
            projectile.MaxUpdates = 2;
            projectile.ranged = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;

        }
        public override string Texture => "CalamityMod/Projectiles/Ranged/PrecisionBolt";

        private Vector2 Recalibrate()
        {

            // Choose the angular turn speed of the bolt on recalibration.
            // This will overshoot significantly at first but will regress to finer movements as time goes on.
            // The exponent in the equation below serves to dampen the turn speed more quickly. It will not hit targets otherwise.
            float turnSpeedFactor = (float)Math.Pow(MathHelper.Clamp(projectile.timeLeft - 40, 0f, 120f) / 120f, 4D);
            float turnAngle = MathHelper.ToRadians(turnSpeedFactor * 75f);

            // Select the velocity which has less of a disparity in terms of angular distance compared to the ideal direction.
            Vector2 leftTurnVelocity = projectile.velocity.RotatedBy(-turnAngle);
            Vector2 righTurnVelocity = projectile.velocity.RotatedBy(turnAngle);
            float leftDirectionImprecision = leftTurnVelocity.AngleBetween(projectile.SafeDirectionTo(potentialTarget.Center));
            float rightDirectionImprecision = righTurnVelocity.AngleBetween(projectile.SafeDirectionTo(potentialTarget.Center));
            potentialTarget = projectile.Center.ClosestNPCAt(512f, true);

            if (leftDirectionImprecision < rightDirectionImprecision)
                return leftTurnVelocity;
            else
                return righTurnVelocity;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, Color.LightSteelBlue.ToVector3());
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (potentialTarget == null) //(Re)target 
                potentialTarget = projectile.Center.ClosestNPCAt(512f, true);

            if (potentialTarget != null)
            {
                //Do some funny slight homing just in case 
                float angularTurnSpeed = MathHelper.ToRadians(2.5f);
                float idealDirection = projectile.AngleTo(potentialTarget.Center);
                float updatedDirection = projectile.velocity.ToRotation().AngleTowards(idealDirection, angularTurnSpeed);
                projectile.velocity = updatedDirection.ToRotationVector2() * projectile.velocity.Length();

                if (projectile.timeLeft % 6 == 0) // Do the STRONG homing
                {
                    Main.PlaySound(SoundID.Item93, projectile.Center);
                    projectile.velocity = Recalibrate();
                }
            }

            Dust trail = Dust.NewDustPerfect(projectile.Center, 267); //Dust trail kinda poopy but idk how to make a cool trail :(
            trail.velocity = Vector2.Zero;
            trail.color = Color.Yellow;
            trail.scale = Main.rand.NextFloat(1f, 1.1f);
            trail.noGravity = true;

        }

        public override void Kill(int timeLeft)
        {
            // Release a burst of magic dust on death.
            if (Main.dedServ)
                return;
            Main.PlaySound(SoundID.Item94, projectile.Center);
            for (int i = 0; i < 10; i++)
            {
                Dust zap = Dust.NewDustPerfect(projectile.Center, 267);
                zap.velocity = projectile.velocity;
                zap.color = Color.Yellow;
                zap.scale = Main.rand.NextFloat(1f, 1.1f);
                zap.noGravity = true;
            }
        }
    }
}