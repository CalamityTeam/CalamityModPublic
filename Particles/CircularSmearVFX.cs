using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class CircularSmearVFX : Particle //Also check out Split mod!
    {
        public override string Texture => "CalamityMod/Particles/CircularSmear";
        public override bool UseAdditiveBlend => true;
        public override bool UseCustomDraw => false;

        public override bool SetLifetime => true;
        public float opacity;

        public CircularSmearVFX(Vector2 position, Color color, float rotation, float scale)
        {
            Position = position;
            Velocity = Vector2.Zero;
            Color = color;
            Scale = scale;
            Rotation = rotation;
            Lifetime = 2;
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = GeneralParticleHandler.GetTexture(Type);

            spriteBatch.Draw(tex, Position - Main.screenPosition, null, Color * opacity * 0.5f, 0, tex.Size()/2f, Scale, SpriteEffects.None, 0);
        }
    }
}
