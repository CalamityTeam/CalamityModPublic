using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;

using static CalamityMod.Particles.BaseFusableParticleSet;

namespace CalamityMod.Particles
{
	public static class FusableParticleManager
	{
		internal static readonly List<FusableParticleRenderCollection> ParticleSets = new List<FusableParticleRenderCollection>();

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
				// Restart the sprite batch. This must be done with an immediate sort mode since a shader is going to be applied.
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

				BaseFusableParticleSet particleSet = particleRenderSet.ParticleSet;
				Effect edgeShader = particleSet.EdgeShader;

				// Draw the current target with the specified shader.
				edgeShader.Parameters["edgeBorderSize"].SetValue(particleSet.BorderSize);
				edgeShader.Parameters["borderShouldBeSolid"].SetValue(particleSet.BorderShouldBeSolid);
				edgeShader.Parameters["edgeBorderColor"].SetValue(particleSet.BorderColor.ToVector3());
				edgeShader.Parameters["screenArea"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight) / Main.GameViewMatrix.Zoom);
				edgeShader.Parameters["screenMoveOffset"].SetValue(Main.screenPosition - Main.screenLastPosition);
				edgeShader.Parameters["uWorldPosition"].SetValue(Main.screenPosition);
				edgeShader.Parameters["renderTargetArea"].SetValue(new Vector2(particleSet.GetRenderTarget.Width, particleSet.GetRenderTarget.Height));
				edgeShader.Parameters["invertedScreen"].SetValue(Main.LocalPlayer.gravDir == -1f);

				// Prepare the background texture for loading.
				Main.graphics.GraphicsDevice.Textures[1] = particleSet.BackgroundTexture;
				edgeShader.Parameters["uImageSize1"].SetValue(particleSet.BackgroundTexture.Size());

				// Do any optional shader operations prior to applying the shader.
				particleSet.PrepareOptionalShaderData(edgeShader);

				// Apply the shader. This is what is used to connect separated blobs together.
				edgeShader.CurrentTechnique.Passes[0].Apply();

				// And draw the particle set's render target with said shader.
				Main.spriteBatch.Draw(particleSet.GetRenderTarget, Vector2.Zero, Color.White);

				Main.spriteBatch.End();
			}
		}

		internal static FusableParticleRenderCollection GetParticleRenderCollectionByType(Type type)
		{
			return ParticleSets.First(s => s.ParticleSet.GetType() == type);
		}

		public static T GetParticleSetByType<T>() where T : BaseFusableParticleSet
		{
			return ParticleSets.First(s => s.ParticleSet.GetType() == typeof(T)).ParticleSet as T;
		}
	}
}
