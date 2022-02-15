using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class TrientCircularSmear : Particle //Trient aka semi but for thirds
    {
        public override string Texture => "CalamityMod/Particles/TrientCircularSmear";
        public override bool UseAdditiveBlend => true;
        public override bool SetLifetime => true;

        public TrientCircularSmear(Vector2 position, Color color, float rotation, float scale)
        {
            Position = position;
            Velocity = Vector2.Zero;
            Color = color;
            Scale = scale;
            Rotation = rotation;
            Lifetime = 3;
        }
    }
}
