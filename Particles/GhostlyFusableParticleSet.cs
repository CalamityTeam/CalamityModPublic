using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
	public class GhostlyFusableParticleSet : BaseFusableParticleSet
	{
		public override float BorderSize => 3f;
		public override bool BorderShouldBeSolid => false;
		public override Color BorderColor => Color.Lerp(Color.Fuchsia, Color.Black, 0.55f) * 0.85f;
		public override List<Effect> BackgroundShaders
		{
			get
			{
				List<Effect> shaders = new List<Effect>()
				{
					GameShaders.Misc["CalamityMod:BaseFusableParticleEdge"].Shader,
					GameShaders.Misc["CalamityMod:BaseFusableParticleEdge"].Shader,
				};
				return shaders;
			}
		}
		public override List<Texture2D> BackgroundTextures
		{
			get
			{
				List<Texture2D> textures = new List<Texture2D>()
				{
					ModContent.GetTexture("CalamityMod/ExtraTextures/PolterFusableParticleBG"),
					ModContent.GetTexture("CalamityMod/ExtraTextures/PolterFusableParticleBG2"),
				};
				return textures;
			}
		}
		public override FusableParticle SpawnParticle(Vector2 center, float sizeStrength)
		{
			Particles.Add(new FusableParticle(center, sizeStrength));
			return Particles.Last();
		}

		public override void UpdateBehavior(FusableParticle particle)
		{
			particle.Size = MathHelper.Clamp(particle.Size - 1.5f, 0f, 400f) * 0.975f;
		}

		public override void PrepareOptionalShaderData(Effect effect, int index)
		{
			switch (index)
			{
				// Background.
				case 0:
					Vector2 offset = Vector2.UnitX * Main.GlobalTime * 0.03f;
					effect.Parameters["generalBackgroundOffset"].SetValue(offset);
					break;

				// Spooky faces.
				case 1:
					offset = Vector2.UnitX * Main.GlobalTime * -0.04f + (Main.GlobalTime * 1.89f).ToRotationVector2() * 0.03f;
					offset.Y += CalamityUtils.PerlinNoise2D(Main.GlobalTime * 0.187f, Main.GlobalTime * 0.193f, 2, 466920161) * 0.025f;
					effect.Parameters["generalBackgroundOffset"].SetValue(offset);
					break;
			}
		}

		public override void DrawParticles()
		{
			foreach (FusableParticle particle in Particles)
			{
				Texture2D fusableParticleBase = ModContent.GetTexture("CalamityMod/ExtraTextures/FusableParticleBase");
				Vector2 drawPosition = particle.Center - Main.screenPosition;
				Vector2 origin = fusableParticleBase.Size() * 0.5f;
				Vector2 scale = Vector2.One * particle.Size / fusableParticleBase.Size();
				Main.spriteBatch.Draw(fusableParticleBase, drawPosition, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
			}
		}
	}
}
