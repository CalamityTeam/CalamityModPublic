using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using System;

namespace CalamityMod.Particles
{
    public class ManaDrainBlob : Particle
    {
        public override bool SetLifetime => false;
        public override string Texture => "CalamityMod/Particles/MicroBloom";
        public override bool UseAdditiveBlend => true;
        public override int FrameVariants => 2;

        public Player Owner;

        public ManaDrainBlob(Player owner, Vector2 position, Vector2 speed, float scale, Color color)
        {
            Owner = owner;
            Position = position;
            Scale = Math.Min(scale, 1f);
            Color = color;
            Velocity = speed;
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            Variant = Main.rand.Next(2);
        }

        public override void Update()
        {
            Scale -= 0.02f;
            if (Scale < 0.1f)
                Kill();

            Scale += 0.007f;

            if (Owner != null && Owner.active)
            {
                Vector2 speedDirection = Position - Owner.Center;
                float distanceToOwner = speedDirection.Length();
                float distanceToOwnerTwisted = 100f - distanceToOwner;

                if (distanceToOwnerTwisted > 0f)
                    Scale -= distanceToOwnerTwisted * 0.0015f;

                speedDirection.Normalize();

                float dustAcceleration = (1f - Scale) * -20f;
                speedDirection *= dustAcceleration;
                Velocity = (Velocity * 4f + speedDirection) / 5f;


                if (distanceToOwner > 16)
                {


                    if (Main.rand.NextBool(7))
                    {
                        float velocityMultiplier = MathHelper.Lerp(0.05f, 1f, MathHelper.Clamp((distanceToOwner - 10f) / 40f, 0f, 1f));
                        Dust chust = Dust.NewDustPerfect(Position, 15, Velocity * Main.rand.NextFloat(0.7f, 1.2f) * velocityMultiplier, Alpha: 100, Scale: Main.rand.NextFloat(1.2f, 1.8f));
                        chust.noGravity = true;
                        chust.noLight = true;
                    }
                }
            }

            Rotation += 0.04f * ((Velocity.X > 0) ? 1f : -1f);
        }
    }
}
