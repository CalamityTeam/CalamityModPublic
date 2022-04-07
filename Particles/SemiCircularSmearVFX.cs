using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class SemiCircularSmearVFX : Particle //Also check out Split mod!
    {
        public override string Texture => "CalamityMod/Particles/SemiCircularSmear";
        public override bool UseAdditiveBlend => true;
        public override bool UseCustomDraw => true;
        public override bool SetLifetime => true;
        public Vector2 Squish;

        public SemiCircularSmearVFX(Vector2 position, Color color, float rotation, float scale, Vector2 squish)
        {
            Position = position;
            Velocity = Vector2.Zero;
            Color = color;
            Scale = scale;
            Squish = squish;
            Rotation = rotation;
            Lifetime = 2;
        }

        //Use custom draw for the squish
        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(tex, Position - Main.screenPosition, null, Color, Rotation, tex.Size() / 2f, Squish * Scale, SpriteEffects.None, 0);
        }
    }
}
