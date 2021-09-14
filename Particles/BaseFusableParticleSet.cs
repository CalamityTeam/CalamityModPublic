using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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

		internal static readonly List<FusableParticleRenderCollection> ParticleSets = new List<FusableParticleRenderCollection>();

		public FusableParticleRenderCollection RenderCollection => GetParticleSetByType(GetType());

		public List<FusableParticle> Particles = new List<FusableParticle>();

		public RenderTarget2D GetRenderTarget => RenderCollection.RenderTarget;

		public virtual float BorderSize => 0f;
		public virtual bool BorderShouldBeSolid => false;
		public virtual Color BorderColor => Color.Transparent;
		public abstract Effect BackgroundShader { get; }
		public abstract Effect EdgeShader { get; }
		public abstract Texture2D BackgroundTexture { get; }
		public abstract FusableParticle SpawnParticle(Vector2 center, float sizeStrength);
		public abstract void UpdateBehavior(FusableParticle particle);
		public abstract void DrawParticles();

		internal static void LoadParticleRenderTargets()
		{
			// Look through every type in the mod, and check if it's derived from BaseFusableParticleSet.
			// If it is, create a default instance of said particle, save it, and create a RenderTarget2D for it to use.
			foreach (Type type in typeof(CalamityMod).Assembly.GetTypes())
			{
				// Don't load abstract classes; they cannot have instances.
				if (type.IsAbstract)
					continue;

				if (type.IsSubclassOf(typeof(BaseFusableParticleSet)))
				{
					BaseFusableParticleSet instance = Activator.CreateInstance(type) as BaseFusableParticleSet;
					RenderTarget2D renderTarget = null;
					if (Main.netMode != NetmodeID.Server)
						renderTarget = new RenderTarget2D(Main.instance.GraphicsDevice, Main.screenWidth, Main.screenHeight, false, default, default, 0, RenderTargetUsage.PreserveContents);

					FusableParticleRenderCollection particleRenderCollection = new FusableParticleRenderCollection(instance, renderTarget);
					ParticleSets.Add(particleRenderCollection);
				}
			}
		}

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

		internal static void PrepareFusableParticleTargets()
		{
			// Don't attempt to draw anything serverside.
			if (Main.netMode == NetmodeID.Server)
				return;

			// Prepare the render target for all fusable particles.
			foreach (FusableParticleRenderCollection particleSet in ParticleSets)
				particleSet.ParticleSet.PrepareRenderTargetForDrawing();
		}

		internal static void RenderAllFusableParticles()
		{
			foreach (FusableParticleRenderCollection particleRenderSet in ParticleSets)
			{
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

				BaseFusableParticleSet particleSet = particleRenderSet.ParticleSet;
				
				// Draw the current target with the specified shader.
				particleSet.EdgeShader.Parameters["edgeBorderSize"].SetValue(particleSet.BorderSize);
				particleSet.EdgeShader.Parameters["borderShouldBeSolid"].SetValue(particleSet.BorderShouldBeSolid);
				particleSet.EdgeShader.Parameters["edgeBorderColor"].SetValue(particleSet.BorderColor.ToVector3());
				particleSet.EdgeShader.Parameters["screenArea"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight) / Main.GameViewMatrix.Zoom);
				particleSet.EdgeShader.Parameters["screenMoveOffset"].SetValue(Main.screenPosition - Main.screenLastPosition);
				particleSet.EdgeShader.Parameters["uWorldPosition"].SetValue(Main.screenPosition);
				particleSet.EdgeShader.Parameters["renderTargetArea"].SetValue(new Vector2(particleSet.GetRenderTarget.Width, particleSet.GetRenderTarget.Height));
				particleSet.EdgeShader.Parameters["invertedScreen"].SetValue(Main.LocalPlayer.gravDir == -1f);

				Main.graphics.GraphicsDevice.Textures[1] = particleSet.BackgroundTexture;
				particleSet.EdgeShader.Parameters["uImageSize1"].SetValue(particleSet.BackgroundTexture.Size());
				particleSet.EdgeShader.CurrentTechnique.Passes[0].Apply();

				Main.spriteBatch.Draw(particleSet.GetRenderTarget, Vector2.Zero, Color.White);

				Main.spriteBatch.End();
			}
		}

		public static FusableParticleRenderCollection GetParticleSetByType(Type type)
		{
			return ParticleSets.First(s => s.ParticleSet.GetType() == type);
		}
	}
}
