using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using System;

namespace CalamityMod.Particles
{
    public class ManaDrainStreak : Particle
    {
        public override string Texture => "CalamityMod/Particles/DrainLineBloom";
        public override bool UseAdditiveBlend => true;
        public override bool UseCustomDraw => true;
        public override bool SetLifetime => true;

        public Player Owner;
        public float StartDistanceFromPlayer;
        public float FinalDistanceFromPlayer;
        public Color StartColor;
        public Color EndColor;
        public Vector2 OverridePosition;

        public ManaDrainStreak(Player owner, float thickness, Vector2 startVector, float finalDistance, Color colorStart, Color colorEnd, int lifetime, Vector2 overridePosition = default)
        {
            Owner = owner;
            Scale = thickness;
            Velocity = Vector2.Zero;
            Rotation = startVector.ToRotation();
            StartDistanceFromPlayer = startVector.Length();
            FinalDistanceFromPlayer = finalDistance;
            StartColor = colorStart;
            EndColor = colorEnd;
            Color = colorStart;
            Lifetime = lifetime;
            OverridePosition = overridePosition;
        }

        public override void Update()
        {
            if (Owner == null || !Owner.active || Owner.dead)
                return;

            Vector2 setPosition = OverridePosition != default ? OverridePosition : Owner.MountedCenter;
            Position = setPosition + Rotation.ToRotationVector2() * MathHelper.Lerp(StartDistanceFromPlayer, FinalDistanceFromPlayer, (float)Math.Pow(LifetimeCompletion, 2));
            Color = Color.Lerp(StartColor, EndColor, LifetimeCompletion);
            Lighting.AddLight(Position, Color.ToVector3() * 0.2f);
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            float currentDisplace = MathHelper.Lerp(StartDistanceFromPlayer, FinalDistanceFromPlayer, (float)Math.Pow(LifetimeCompletion, 2));
            float earlierDisplace = MathHelper.Lerp(StartDistanceFromPlayer, FinalDistanceFromPlayer, (float)Math.Pow(Math.Clamp(LifetimeCompletion - 0.2f, 0f, 1f), 2));

            float Length = (currentDisplace - earlierDisplace) / tex.Height;
            Vector2 scale = new Vector2(Scale, Length);

            Vector2 origin = new Vector2(tex.Width / 2f, tex.Height);

            spriteBatch.Draw(tex, Position - Main.screenPosition, null, Color, Rotation - MathHelper.PiOver2, origin, scale, SpriteEffects.None, 0);

        }
    }
}
