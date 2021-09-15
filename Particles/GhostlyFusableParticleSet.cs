using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
	public class GhostlyFusableParticleSet : BaseFusableParticleSet
	{
		public override float BorderSize => 5f;
		public override bool BorderShouldBeSolid => false;
		public override Color BorderColor => Color.Lerp(Color.LightBlue, Color.Red, 0.35f) * 1.4f;
		public override Effect BackgroundShader { get; }
		public override Effect EdgeShader => GameShaders.Misc["CalamityMod:BaseFusableParticleEdge"].Shader;
		public override Texture2D BackgroundTexture => ModContent.GetTexture("CalamityMod/ExtraTextures/PolterFusableParticleBG");
		public override FusableParticle SpawnParticle(Vector2 center, float sizeStrength)
		{
			Particles.Add(new FusableParticle(center, sizeStrength));
			return Particles.Last();
		}

		public override void UpdateBehavior(FusableParticle particle)
		{
			particle.Size = MathHelper.Clamp(particle.Size - 1.5f, 0f, 400f) * 0.975f;
		}

		public override void PrepareOptionalShaderData(Effect effect)
		{
			Vector2 offset = Vector2.UnitX * Main.GlobalTime * 0.03f;
			effect.Parameters["generalBackgroundOffset"].SetValue(offset);
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
