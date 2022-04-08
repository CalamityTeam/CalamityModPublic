using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class ImpactParticle : Particle
    {
        public float AngularVelocity;
        public override bool SetLifetime => true;
        public override bool UseCustomDraw => true;
        public override bool UseAdditiveBlend => true;

        public override string Texture => "CalamityMod/Projectiles/StarProj";

        public ImpactParticle(Vector2 relativePosition, float angularVelocity, int lifetime, float scale, Color color)
        {
            Position = relativePosition;
            Scale = scale;
            AngularVelocity = angularVelocity;
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            Lifetime = lifetime;
            Color = color;
        }

        public override void Update() => Rotation += AngularVelocity;

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            float scaleFactor = CalamityUtils.Convert01To010(LifetimeCompletion) * 1.3f;
            Vector2 scale = new Vector2(0.3f, 1f) * scaleFactor;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color, 0f, texture.Size() * 0.5f, scale, 0, 0f);
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color, Rotation, texture.Size() * 0.5f, scale, 0, 0f);
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color, -Rotation, texture.Size() * 0.5f, scale, 0, 0f);
        }
    }
}
