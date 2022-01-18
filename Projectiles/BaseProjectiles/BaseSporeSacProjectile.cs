using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.BaseProjectiles
{
    public abstract class BaseSporeSacProjectile : ModProjectile
    {
        public Player Owner => Main.player[projectile.owner];
        public bool FadingOut => GeneralTimer > 5400f;
        public bool HomesInStronglyOnEnemies => projectile.ai[1] == 1f;
        public ref float GeneralTimer => ref projectile.ai[0];
        public ref float PulseIncrement => ref projectile.localAI[0];
        public ref float MoveTimer => ref projectile.localAI[1];

        public virtual Color? LightColor => new Color(0.25f, 0.025f, 0.275f);

        public virtual float HomeDistance => 600f;
        public virtual float HomeSpeed => 0.75f;

        public override void AI()
        {
            // Emit light if applicable.
            if (LightColor.HasValue)
            {
                float lightFactor = projectile.Opacity * projectile.scale;
                Lighting.AddLight(projectile.Center, LightColor.Value.ToVector3() * lightFactor);
            }

            PulseIncrement++;

            // Wrap back to once the increment has reached its apex.
            if (PulseIncrement >= 90f)
                PulseIncrement *= -1f;

            // Pulse in and out based on the increment in a way analogous to a triangle wave.
            projectile.scale += (PulseIncrement >= 0f).ToDirectionInt() * 0.003f;

            // Rotate with an angular velocity proportional to the scale of the projectile.
            projectile.rotation += projectile.scale * 0.0025f;

            // Determine the move direction.
            Vector2 moveDirection = Vector2.One;
            switch (projectile.identity % 6)
            {
                case 0:
                    moveDirection.X *= -1f;
                    break;
                case 1:
                    moveDirection.Y *= -1f;
                    break;
                case 2:
                    moveDirection *= -1f;
                    break;
                case 3:
                    moveDirection.X = 0f;
                    break;
                case 4:
                    moveDirection.Y = 0f;
                    break;
            }

            // Determine movement with a very, very slow acceleration.
            MoveTimer += 1f;
            if (MoveTimer > 60f)
                MoveTimer = -180f;
            projectile.velocity += moveDirection * (MoveTimer >= -60f).ToDirectionInt() * 0.002f;

            // Fade out and die after enough time.
            // This process is accelerated the farther away the projectile is from its owner.
            GeneralTimer++;
            if (FadingOut)
            {
                projectile.damage = 0;
                if (projectile.alpha < 255)
                    projectile.alpha = Utils.Clamp(projectile.alpha + 5, 0, 255);

                else if (projectile.owner == Main.myPlayer)
                    projectile.Kill();
            }
            else
            {
                float ownerDistanceIncrement = projectile.Distance(Owner.Center);
                if (ownerDistanceIncrement > 400f)
                    ownerDistanceIncrement *= 1.1f;

                if (ownerDistanceIncrement > 500f)
                    ownerDistanceIncrement *= 1.2f;

                if (ownerDistanceIncrement > 600f)
                    ownerDistanceIncrement *= 1.3f;

                if (ownerDistanceIncrement > 700f)
                    ownerDistanceIncrement *= 1.4f;

                if (ownerDistanceIncrement > 800f)
                    ownerDistanceIncrement *= 1.5f;

                if (ownerDistanceIncrement > 900f)
                    ownerDistanceIncrement *= 1.6f;

                if (ownerDistanceIncrement > 1000f)
                    ownerDistanceIncrement *= 1.7f;

                GeneralTimer += ownerDistanceIncrement * 0.01f;

                // Fade in.
                projectile.alpha = Utils.Clamp(projectile.alpha - 10, 50, 255);
            }
            if (HomesInStronglyOnEnemies || projectile.timeLeft < (3300 + PulseIncrement))
            {
                NPC potentialTarget = projectile.Center.ClosestNPCAt(HomeDistance);

                if (potentialTarget != null)
                {
                    if (HomesInStronglyOnEnemies)
                        projectile.extraUpdates = 5;

                    projectile.velocity = (projectile.velocity * 10f + projectile.SafeDirectionTo(potentialTarget.Center) * HomeSpeed) / 11f;
                    return;
                }
            }
            if (projectile.velocity.Length() > 0.2f)
            {
                projectile.velocity *= 0.98f;
            }
        }
    }
}
