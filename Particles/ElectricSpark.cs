using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class ElectricSpark : Particle
    {
        public override string Texture => "CalamityMod/Particles/ElectricSpark";
        public override bool UseAdditiveBlend => true;
        public override bool UseCustomDraw => true;
        public override bool SetLifetime => true;
        public override int FrameVariants => 2;

        private float Spin;
        private float opacity;
        private Color Bloom;
        private Color LightColor => Bloom * opacity;
        private float BloomScale;
        private float MaxJumpRotation; //Max rotation that the particle can change velocity per "jump"
        private float JumpTime; //Time between jumps
        private Vector2 OriginalSpeed;

        public ElectricSpark(Vector2 position, Vector2 velocity, Color color, Color bloom, float scale, int lifeTime, float maxJumpRotation = MathHelper.PiOver4, float jumpTime = 10, float rotationSpeed = 1f, float bloomScale = 1f)
        {
            Position = position;
            Velocity = velocity;
            OriginalSpeed = velocity;
            Color = color;
            Bloom = bloom;
            Scale = scale;
            Lifetime = lifeTime;
            MaxJumpRotation = maxJumpRotation;
            JumpTime = jumpTime;
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            Spin = rotationSpeed;
            BloomScale = bloomScale;
            Variant = Main.rand.Next(2);
        }

        public override void Update()
        {
            opacity -= JumpTime * 0.25f / Lifetime;
            if (Time % JumpTime == 0)
            {
                opacity = 1f;
                Velocity = OriginalSpeed.RotatedByRandom(MaxJumpRotation);
            }
            Velocity *= 0.6f;
            Rotation += Spin * ((Velocity.X > 0) ? 1f : -1f) * (LifetimeCompletion > 0.5 ? 1f : 0.5f);

            Lighting.AddLight(Position, LightColor.R / 255f, LightColor.G / 255f, LightColor.B / 255f);
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D sparkTexture = GeneralParticleHandler.GetTexture(Type);
            Rectangle frame = new Rectangle(0, 6 * Variant, 6, 6);
            Texture2D bloomTexture = ModContent.GetTexture("CalamityMod/Particles/BloomCircle");
            //Ajust the bloom's texture to be the same size as the star's
            float properBloomSize = (float)frame.Height / (float)bloomTexture.Height;

            spriteBatch.Draw(bloomTexture, Position - Main.screenPosition, null, Bloom * opacity * 0.5f, 0, bloomTexture.Size() / 2f, Scale * BloomScale * properBloomSize, SpriteEffects.None, 0);
            spriteBatch.Draw(sparkTexture, Position - Main.screenPosition, frame, Color * opacity, Rotation, frame.Size() / 2f, Scale, SpriteEffects.None, 0);
        }
    }
}
