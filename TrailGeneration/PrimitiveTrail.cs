using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using Terraria;

namespace CalamityMod
{
	public class PrimitiveTrail
	{
		public struct VertexPosition2DColor : IVertexType
		{
			public Vector2 Position;
			public Color Color;
			public VertexDeclaration VertexDeclaration => _vertexDeclaration;

			private static readonly VertexDeclaration _vertexDeclaration = new VertexDeclaration(new VertexElement[]
			{
				new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
				new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0)
			});
			public VertexPosition2DColor(Vector2 position, Color color)
			{
				Position = position;
				Color = color;
			}
		}

		public delegate float VertexWidthFunction(float completionRatio);
		public delegate Color VertexColorFunction(float completionRatio);

		public VertexWidthFunction WidthFunction;
		public VertexColorFunction ColorFunction;

		// NOTE: Beziers can be laggy when a lot of control points are used, since our implementation
		// uses a recursive Lerp that gets more computationally expensive the more original indices.
		// n(n - 1)/2 linear interpolations to be precise, where n is the amount of original indices.
		public bool UsesSmoothening;
		public Effect EffectToUse;

		public PrimitiveTrail(VertexWidthFunction widthFunction, VertexColorFunction colorFunction, bool useSmoothening = true, Effect specialShader = null)
		{
			if (widthFunction is null || colorFunction is null)
				throw new NullReferenceException($"In order to create a primitive trail, a non-null {(widthFunction is null ? "width" : "color")} function must be specified.");
			WidthFunction = widthFunction;
			ColorFunction = colorFunction;

			UsesSmoothening = useSmoothening;

			EffectToUse = specialShader ?? new BasicEffect(Main.instance.GraphicsDevice);
			if (EffectToUse is BasicEffect)
			{
				(EffectToUse as BasicEffect).VertexColorEnabled = true;
				(EffectToUse as BasicEffect).TextureEnabled = false;
				UpdateBasicEffect(EffectToUse as BasicEffect);
			}
		}

		public void UpdateBasicEffect(BasicEffect effect)
		{
			// The screen width and height.
			int width = Main.instance.GraphicsDevice.Viewport.Width;
			int height = Main.instance.GraphicsDevice.Viewport.Height;

			Vector2 zoom = Main.GameViewMatrix.Zoom;

			// Get a matrix that aims towards the Z axis (these calculations are relative to a 2D world).
			Matrix effectView = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up);

			// Offset the matrix to the appropriate position.
			effectView *= Matrix.CreateTranslation(width / 2, height / -2, 0f);

			// Flip the matrix around 180 degrees.
			effectView *= Matrix.CreateRotationZ(MathHelper.Pi);

			// And account for the current zoom.
			effectView *= Matrix.CreateScale(zoom.X, zoom.Y, 1f);

			Matrix effectProjection = Matrix.CreateOrthographic(width, height, 0f, 1000f);
			effect.View = effectView;
			effect.Projection = effectProjection;
		}

		[Pure]
		public List<Vector2> GetTrailPoints(IEnumerable<Vector2> originalPositions, Vector2 generalOffset, int totalTrailPoints)
		{
			if (EffectToUse is BasicEffect)
				UpdateBasicEffect((BasicEffect)EffectToUse);

			// Don't smoothen the points unless explicitly told do so.
			if (!UsesSmoothening)
			{
				List<Vector2> basePoints = originalPositions.Where(originalPosition => originalPosition != Vector2.Zero).ToList();
				List<Vector2> endPoints = new List<Vector2>();

				if (basePoints.Count < 3)
					return endPoints;

				// Remap the original positions across a certain length.
				for (int i = 0; i < totalTrailPoints; i++)
				{
					float completionRatio = i / (float)totalTrailPoints;
					int currentColorIndex = (int)(completionRatio * (basePoints.Count - 1));
					Vector2 currentColor = basePoints[currentColorIndex];
					Vector2 nextColor = basePoints[(currentColorIndex + 1) % basePoints.Count];
					endPoints.Add(Vector2.Lerp(currentColor, nextColor, completionRatio * (basePoints.Count - 1) % 0.999f) + generalOffset);
				}
				return endPoints;
			}

			List<Vector2> controlPoints = new List<Vector2>();
			for (int i = 0; i < originalPositions.Count(); i++)
			{
				// Don't incorporate points that are zeroed out.
				// They are almost certainly a result of incomplete oldPos arrays.
				if (originalPositions.ElementAt(i) == Vector2.Zero)
					continue;
				controlPoints.Add(originalPositions.ElementAt(i) + generalOffset);
			}
			BezierCurve bezierCurve = new BezierCurve(controlPoints.ToArray());
			return controlPoints.Count <= 1 ? controlPoints : bezierCurve.GetPoints(totalTrailPoints);
		}

		public VertexPosition2DColor[] GetVerticesFromTrailPoints(List<Vector2> trailPoints)
		{
			VertexPosition2DColor[] vertices = new VertexPosition2DColor[trailPoints.Count * 6];
			for (int i = 0; i < trailPoints.Count - 1; i++)
			{
				float completionRatio = i / (float)trailPoints.Count;
				float widthAtVertex = WidthFunction(completionRatio);
				Color vertexColor = ColorFunction(completionRatio);

				Vector2 currentPosition = trailPoints[i];
				Vector2 positionAhead = trailPoints[i + 1];
				Vector2 directionToAhead = (positionAhead - trailPoints[i]).SafeNormalize(Vector2.Zero);

				// Point 90 degrees away from the direction towards the next point, and use it to mark the edges of the rectangle.
				// This doesn't use RotatedBy for the sake of performance (there can potentially be a lot of trail points).
				Vector2 sideDirection = new Vector2(-directionToAhead.Y, directionToAhead.X);

				// The bounds of the rectangle.
				Vector2 firstUp = currentPosition - sideDirection * widthAtVertex;
				Vector2 firstDown = currentPosition + sideDirection * widthAtVertex;

				Vector2 secondUp = positionAhead - sideDirection * widthAtVertex;
				Vector2 secondDown = positionAhead + sideDirection * widthAtVertex;

				// What this is doing, at its core, is defining a rectangle based on two triangles.
				// These triangles are defined based on the width of the strip at that point.
				// The resulting rectangles combined are what make the trail itself.
				vertices[i * 6] = new VertexPosition2DColor(firstUp, vertexColor);
				vertices[i * 6 + 1] = new VertexPosition2DColor(secondUp, vertexColor);
				vertices[i * 6 + 2] = new VertexPosition2DColor(firstDown, vertexColor);

				vertices[i * 6 + 3] = new VertexPosition2DColor(secondUp, vertexColor);
				vertices[i * 6 + 4] = new VertexPosition2DColor(secondDown, vertexColor);
				vertices[i * 6 + 5] = new VertexPosition2DColor(firstDown, vertexColor);
			}
			return vertices;
		}

		public void Draw(IEnumerable<Vector2> originalPositions, Vector2 generalOffset, int totalTrailPoints)
		{
			Main.instance.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

			List<Vector2> trailPoints = GetTrailPoints(originalPositions, generalOffset, totalTrailPoints);

			// A trail with only one point or less has nothing to connect to, and therefore, can't make a trail.
			if (trailPoints.Count <= 1)
				return;

			VertexPosition2DColor[] vertices = GetVerticesFromTrailPoints(trailPoints);

			foreach (var pass in EffectToUse.CurrentTechnique.Passes)
			{
				pass.Apply();

				// The division by 3 here is because, vertices is a list of vertices.
				// That parameter, however, wants the amount of primitives (in this case triangles), it should draw.
				// Getting the amount of triangles in this case is simply a matter of dividing the total vertices by 3.
				Main.instance.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length / 3);
			}
		}
	}
}