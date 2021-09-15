using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Particles
{
	public abstract class BaseFusableParticleSet
	{
		public class FusableParticleRenderCollection
		{
			public BaseFusableParticleSet ParticleSet;
			public RenderTarget2D RenderTarget;
			public FusableParticleRenderCollection(BaseFusableParticleSet set, RenderTarget2D renderTarget)
			{
				ParticleSet = set;
				RenderTarget = renderTarget;
			}
		}

		public class FusableParticle
		{
			public Vector2 Center;
			public float Size;
			public Vector2 TopLeft => Center + new Vector2(-1f, -1f) * Size * 0.5f;
			public Vector2 TopRight => Center + new Vector2(1f, -1f) * Size * 0.5f;
			public Vector2 BottomLeft => Center + new Vector2(-1f, 1f) * Size * 0.5f;
			public Vector2 BottomRight => Center + new Vector2(1f, 1f) * Size * 0.5f;

			public FusableParticle(Vector2 center, float size)
			{
				Center = center;
				Size = size;
			}
		}

		public FusableParticleRenderCollection RenderCollection => FusableParticleManager.GetParticleRenderCollectionByType(GetType());

		public List<FusableParticle> Particles = new List<FusableParticle>();

		public RenderTarget2D GetRenderTarget => RenderCollection.RenderTarget;

		public virtual float BorderSize => 0f;
		public virtual bool BorderShouldBeSolid => false;
		public virtual Color BorderColor => Color.Transparent;
		public virtual void PrepareOptionalShaderData(Effect effect) { }
		public abstract Effect BackgroundShader { get; }
		public abstract Effect EdgeShader { get; }
		public abstract Texture2D BackgroundTexture { get; }
		public abstract FusableParticle SpawnParticle(Vector2 center, float sizeStrength);
		public abstract void UpdateBehavior(FusableParticle particle);
		public abstract void DrawParticles();

		internal void PrepareRenderTargetForDrawing()
		{
			// Don't bother doing anything if this method is called serverside.
			if (Main.netMode == NetmodeID.Server)
				return;

			// Go to the specialized render target for this particle set and clear the entire thing to use a base of transparent pixels.
			Main.instance.GraphicsDevice.SetRenderTarget(GetRenderTarget);
			Main.instance.GraphicsDevice.Clear(Color.Transparent);

			// Draw the particles.
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

			// Clear away any particles that shouldn't exist anymore.
			Particles.RemoveAll(p => p.Size <= 1f);

			DrawParticles();
			Main.spriteBatch.End();

			// Return to using the previous render targets after done drawing everything to this target.
			Main.instance.GraphicsDevice.SetRenderTarget(null);
		}
	}
}
