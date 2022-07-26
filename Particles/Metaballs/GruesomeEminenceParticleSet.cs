using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.Particles.Metaballs
{
    public class GruesomeEminenceParticleSet : BaseFusableParticleSet
    {
        public override float BorderSize => 3f;
        public override bool BorderShouldBeSolid => false;
        public override Color BorderColor => Color.Lerp(Color.Fuchsia, Color.Black, 0.55f) * 0.85f;

        public override List<Effect> BackgroundShaders => new()
        {
            GameShaders.Misc["CalamityMod:BaseFusableParticleEdge"].Shader,
            GameShaders.Misc["CalamityMod:BaseFusableParticleEdge"].Shader,
            GameShaders.Misc["CalamityMod:BaseFusableParticleEdge"].Shader,
            GameShaders.Misc["CalamityMod:BaseFusableParticleEdge"].Shader,
            GameShaders.Misc["CalamityMod:BaseFusableParticleEdge"].Shader,
        };
        public override List<Texture2D> BackgroundTextures => new()
        {
            ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/ParticleBackgrounds/GruesomeEminence_Ghost_Layer1").Value,
            ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/ParticleBackgrounds/GruesomeEminence_Ghost_Layer2").Value,
            ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/ParticleBackgrounds/GruesomeEminence_Ghost_Layer3").Value,
            ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/ParticleBackgrounds/GruesomeEminence_Ghost_Layer4").Value,
            ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/ParticleBackgrounds/GruesomeEminence_Ghost_Layer5").Value,
        };
        public override FusableParticle SpawnParticle(Vector2 center, float sizeStrength)
        {
            Particles.Add(new FusableParticle(center, sizeStrength));
            return Particles.Last();
        }

        public override void UpdateBehavior(FusableParticle particle)
        {
            particle.Size = MathHelper.Clamp(particle.Size - 2.5f, 0f, 400f) * 0.97f;
        }

        public override void PrepareOptionalShaderData(Effect effect, int index)
        {
            switch (index)
            {
                // Background.
                case 0:
                    Vector2 offset = Vector2.UnitX * Main.GlobalTimeWrappedHourly * 0.03f;
                    effect.Parameters["generalBackgroundOffset"].SetValue(offset);
                    break;

                // Gaseous skulls.
                case 1:
                    offset = Vector2.One * (float)Math.Cos(Main.GlobalTimeWrappedHourly * 0.041f) * 2f;
                    offset = offset.RotatedBy((float)Math.Cos(Main.GlobalTimeWrappedHourly * 0.08f) * 0.97f);
                    effect.Parameters["generalBackgroundOffset"].SetValue(offset);
                    break;

                // Spooky faces 1.
                case 2:
                    offset = (Main.GlobalTimeWrappedHourly * 2.02f).ToRotationVector2() * 0.036f;
                    offset.Y += (float)Math.Cos(Main.GlobalTimeWrappedHourly * 0.161f) * 0.5f + 0.5f;
                    effect.Parameters["generalBackgroundOffset"].SetValue(offset);
                    break;

                // Spooky faces 2.
                case 3:
                    offset = Vector2.UnitX * Main.GlobalTimeWrappedHourly * -0.04f + (Main.GlobalTimeWrappedHourly * 1.89f).ToRotationVector2() * 0.03f;
                    offset.Y += CalamityUtils.PerlinNoise2D(Main.GlobalTimeWrappedHourly * 0.187f, Main.GlobalTimeWrappedHourly * 0.193f, 2, 466920161) * 0.025f;
                    effect.Parameters["generalBackgroundOffset"].SetValue(offset);
                    break;

                // Spooky faces 3.
                case 4:
                    offset = Vector2.UnitX * Main.GlobalTimeWrappedHourly * 0.037f + (Main.GlobalTimeWrappedHourly * 1.77f).ToRotationVector2() * 0.04725f;
                    offset.Y += CalamityUtils.PerlinNoise2D(Main.GlobalTimeWrappedHourly * 0.187f, Main.GlobalTimeWrappedHourly * 0.193f, 2, 577215664) * 0.05f;
                    effect.Parameters["generalBackgroundOffset"].SetValue(offset);
                    break;
            }
        }

        public override void DrawParticles()
        {
            Texture2D fusableParticleBase = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/FusableParticleBase").Value;
            foreach (FusableParticle particle in Particles)
            {
                Vector2 drawPosition = particle.Center - Main.screenPosition;
                Vector2 origin = fusableParticleBase.Size() * 0.5f;
                Vector2 scale = Vector2.One * particle.Size / fusableParticleBase.Size();
                Main.spriteBatch.Draw(fusableParticleBase, drawPosition, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
            }
        }
    }
}
