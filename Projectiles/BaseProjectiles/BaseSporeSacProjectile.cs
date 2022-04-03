using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.BaseProjectiles
{
    public abstract class BaseSporeSacProjectile : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];
        public bool FadingOut => GeneralTimer > 5400f;
        public bool HomesInStronglyOnEnemies => Projectile.ai[1] == 1f;
        public ref float GeneralTimer => ref Projectile.ai[0];
        public ref float PulseIncrement => ref Projectile.localAI[0];
        public ref float MoveTimer => ref Projectile.localAI[1];

        public virtual Color? LightColor => new Color(0.25f, 0.025f, 0.275f);

        public virtual float HomeDistance => 600f;
        public virtual float HomeSpeed => 0.75f;

        public override void AI()
        {
            // Emit light if applicable.
            if (LightColor.HasValue)
            {
                float lightFactor = Projectile.Opacity * Projectile.scale;
                Lighting.AddLight(Projectile.Center, LightColor.Value.ToVector3() * lightFactor);
            }

            PulseIncrement++;

            // Wrap back to once the increment has reached its apex.
            if (PulseIncrement >= 90f)
                PulseIncrement *= -1f;

            // Pulse in and out based on the increment in a way analogous to a triangle wave.
            Projectile.scale += (PulseIncrement >= 0f).ToDirectionInt() * 0.003f;

            // Rotate with an angular velocity proportional to the scale of the projectile.
            Projectile.rotation += Projectile.scale * 0.0025f;

            // Determine the move direction.
            Vector2 moveDirection = Vector2.One;
            switch (Projectile.identity % 6)
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
            Projectile.velocity += moveDirection * (MoveTimer >= -60f).ToDirectionInt() * 0.002f;

            // Fade out and die after enough time.
            // This process is accelerated the farther away the projectile is from its owner.
            GeneralTimer++;
            if (FadingOut)
            {
                Projectile.damage = 0;
                if (Projectile.alpha < 255)
                    Projectile.alpha = Utils.Clamp(Projectile.alpha + 5, 0, 255);

                else if (Projectile.owner == Main.myPlayer)
                    Projectile.Kill();
            }
            else
            {
                float ownerDistanceIncrement = Projectile.Distance(Owner.Center);
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
                Projectile.alpha = Utils.Clamp(Projectile.alpha - 10, 50, 255);
            }
            if (HomesInStronglyOnEnemies || Projectile.timeLeft < (3300 + PulseIncrement))
            {
                NPC potentialTarget = Projectile.Center.ClosestNPCAt(HomeDistance);

                if (potentialTarget != null)
                {
                    if (HomesInStronglyOnEnemies)
                        Projectile.extraUpdates = 5;

                    Projectile.velocity = (Projectile.velocity * 10f + Projectile.SafeDirectionTo(potentialTarget.Center) * HomeSpeed) / 11f;
                    return;
                }
            }
            if (Projectile.velocity.Length() > 0.2f)
            {
                Projectile.velocity *= 0.98f;
            }
        }
    }
}
