using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class QuickSparkleParticle : Particle
    {
        public float StoredScale;

        public float LightStrength;

        public bool EmitLight;

        public Vector2 StretchFactor;

        public QuickSparkleParticle(Vector2 position, Vector2 velocity, Color color, float scale, int lifetime, Vector2? stretchFactor = null,bool emitLight = false, float lightStrength = 1.45f)
        {
            Position = position;
            Velocity = velocity;
            Color = color;
            StoredScale = scale;
            Lifetime = lifetime;
            StretchFactor = stretchFactor ?? new(0.75f, 1.75f);
            EmitLight = emitLight;
            LightStrength = lightStrength;
        }

        public override bool SetLifetime => true;

        public override bool UseAdditiveBlend => true;

        public override bool UseCustomDraw => true;

        public override string Texture => "CalamityMod/ExtraTextures/ShineFlare";

        public override void Update()
        {
            Velocity *= 0.92f;
            Scale = MathHelper.Lerp(0f, StoredScale, MathF.Sin(LifetimeCompletion * MathHelper.Pi));
            if (EmitLight)
                Lighting.AddLight(Position, Color.ToVector3() * Scale * LightStrength);
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GeneralParticleHandler.GetTexture(Type), Position - Main.screenPosition, null, Color.White, Rotation, GeneralParticleHandler.GetTexture(Type).Size() * 0.5f, Scale * StretchFactor * 0.1f, 0, 0f);
            spriteBatch.Draw(GeneralParticleHandler.GetTexture(Type), Position - Main.screenPosition, null, Color, Rotation, GeneralParticleHandler.GetTexture(Type).Size() * 0.5f, Scale * StretchFactor * 0.2f, 0, 0f);
        }
    }
}
