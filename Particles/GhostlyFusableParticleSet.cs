using CalamityMod.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
	public class GhostlyFusableParticleSet : BaseFusableParticleSet
	{
		public static GhostlyFusableParticleSet Instance => GetParticleSetByType(typeof(GhostlyFusableParticleSet)).ParticleSet as GhostlyFusableParticleSet;
		public override Effect BackgroundShader { get; }
		public override Effect EdgeShader => CalamityShaders.BaseFusableParticleEdgeShader;
		public override FusableParticle SpawnParticle(Vector2 center)
		{
			Particles.Add(new FusableParticle(center, Main.rand.NextFloat(64f, 85f)));
			return Particles.Last();
		}

		public override void UpdateBehavior(FusableParticle particle)
		{
			particle.Size = MathHelper.Clamp(particle.Size - 0.7f, 0f, 100f);
		}

		public override void DrawParticles()
		{
			// Clear away any particles that shouldn't exist anymore.
			Particles.RemoveAll(p => p.Size <= 1f);

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
