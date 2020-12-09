using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Graphics.Shaders;

namespace CalamityMod
{
	public class PrimitiveTrail
	{
		public struct VertexPosition2DColor : IVertexType
		{
			public Vector2 Position;
			public Color Color;
			public Vector2 TextureCoordinates;
			public VertexDeclaration VertexDeclaration => _vertexDeclaration;

			private static readonly VertexDeclaration _vertexDeclaration = new VertexDeclaration(new VertexElement[]
			{
				new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
				new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
				new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
			});
			public VertexPosition2DColor(Vector2 position, Color color, Vector2 textureCoordinates)
			{
				Position = position;
				Color = color;
				TextureCoordinates = textureCoordinates;
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
		public BasicEffect BaseEffect;
		public MiscShaderData SpecialShader;

		public PrimitiveTrail(VertexWidthFunction widthFunction, VertexColorFunction colorFunction, bool useSmoothening = true, MiscShaderData specialShader = null)
		{
			if (widthFunction is null || colorFunction is null)
				throw new NullReferenceException($"In order to create a primitive trail, a non-null {(widthFunction is null ? "width" : "color")} function must be specified.");
			WidthFunction = widthFunction;
			ColorFunction = colorFunction;

			UsesSmoothening = useSmoothening;

			if (specialShader != null)
				SpecialShader = specialShader;

			BaseEffect = new BasicEffect(Main.instance.GraphicsDevice)
			{
				VertexColorEnabled = true,
				TextureEnabled = false
			};
			UpdateBaseEffect(out _, out _);
		}

		public void UpdateBaseEffect(out Matrix effectProjection, out Matrix effectView)
		{
			// The screen width and height.
			int width = Main.instance.GraphicsDevice.Viewport.Width;
			int height = Main.instance.GraphicsDevice.Viewport.Height;

			Vector2 zoom = Main.GameViewMatrix.Zoom;

			// Get a matrix that aims towards the Z axis (these calculations are relative to a 2D world).
			effectView = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up);

			// Offset the matrix to the appropriate position.
			effectView *= Matrix.CreateTranslation(width / 2, height / -2, 0f);

			// Flip the matrix around 180 degrees.
			effectView *= Matrix.CreateRotationZ(MathHelper.Pi);

			// And account for the current zoom.
			effectView *= Matrix.CreateScale(zoom.X, zoom.Y, 1f);

			effectProjection = Matrix.CreateOrthographic(width, height, 0f, 1000f);
			BaseEffect.View = effectView;
			BaseEffect.Projection = effectProjection;
		}

		public List<Vector2> GetTrailPoints(IEnumerable<Vector2> originalPositions, Vector2 generalOffset, int totalTrailPoints)
		{
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
			List<VertexPosition2DColor> vertices = new List<VertexPosition2DColor>();
			for (int i = 0; i < trailPoints.Count - 1; i++)
			{
				float completionRatio = i / (float)trailPoints.Count;
				float widthAtVertex = WidthFunction(completionRatio);
				Color vertexColor = ColorFunction(completionRatio);

				Vector2 currentPosition = trailPoints[i];
				Vector2 positionAhead = trailPoints[i + 1];
				Vector2 directionToAhead = (positionAhead - trailPoints[i]).SafeNormalize(Vector2.Zero);

				Vector2 leftCurrentTextureCoord = new Vector2(completionRatio, 0f);
				Vector2 rightCurrentTextureCoord = new Vector2(completionRatio, 1f);

				// Point 90 degrees away from the direction towards the next point, and use it to mark the edges of the rectangle.
				// This doesn't use RotatedBy for the sake of performance (there can potentially be a lot of trail points).
				Vector2 sideDirection = new Vector2(-directionToAhead.Y, directionToAhead.X);

				// What this is doing, at its core, is defining a rectangle based on two triangles.
				// These triangles are defined based on the width of the strip at that point.
				// The resulting rectangles combined are what make the trail itself.
				vertices.Add(new VertexPosition2DColor(currentPosition - sideDirection * widthAtVertex, vertexColor, leftCurrentTextureCoord));
				vertices.Add(new VertexPosition2DColor(currentPosition + sideDirection * widthAtVertex, vertexColor, rightCurrentTextureCoord));
			}

			return vertices.ToArray();
		}

		public short[] GetIndicesFromTrailPoints(int pointCount)
		{
			// What this is doing is basically representing each point on the vertices list as
			// indices. These indices should come together to create a tiny rectangle that acts
			// as a segment on the trail. This is achieved here by splitting the indices (or rather, points)
			// into 2 triangles, which requires 6 points.
			// The logic here basically determines which indices are connected together.
			int totalIndices = (pointCount - 1) * 6;
			short[] indices = new short[totalIndices];

			for (int i = 0; i < pointCount - 2; i++)
			{
				int startingTriangleIndex = i * 6;
				int connectToIndex = i * 2;
				indices[startingTriangleIndex] = (short)connectToIndex;
				indices[startingTriangleIndex + 1] = (short)(connectToIndex + 1);
				indices[startingTriangleIndex + 2] = (short)(connectToIndex + 2);
				indices[startingTriangleIndex + 3] = (short)(connectToIndex + 2);
				indices[startingTriangleIndex + 4] = (short)(connectToIndex + 1);
				indices[startingTriangleIndex + 5] = (short)(connectToIndex + 3);
			}

			return indices;
		}

		public void Draw(IEnumerable<Vector2> originalPositions, Vector2 generalOffset, int totalTrailPoints)
		{
			Main.instance.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
			List<Vector2> trailPoints = GetTrailPoints(originalPositions, generalOffset, totalTrailPoints);

			// A trail with only one point or less has nothing to connect to, and therefore, can't make a trail.
			if (originalPositions.Count() <= 1 || trailPoints.Count <= 1)
				return;

			UpdateBaseEffect(out Matrix projection, out Matrix view);
			VertexPosition2DColor[] vertices = GetVerticesFromTrailPoints(trailPoints);
			short[] triangleIndices = GetIndicesFromTrailPoints(trailPoints.Count);

			if (SpecialShader != null)
			{
				SpecialShader.Shader.Parameters["uWorldViewProjection"].SetValue(view * projection);
				SpecialShader.Apply();
			}
			else
				BaseEffect.CurrentTechnique.Passes[0].Apply();

			Main.instance.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, triangleIndices, 0, triangleIndices.Length / 3);

			Main.pixelShader.CurrentTechnique.Passes[0].Apply();
		}
	}
}