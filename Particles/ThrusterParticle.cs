using System;
using CalamityMod.Enums;
using CalamityMod.Items.Accessories;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rail;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class ThrusterParticle : Particle
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public override bool SetLifetime => true;
        public override bool UseCustomDraw => true;

        public Vector2[] Path = new Vector2[2];
        public Vector2 DashDirection;
        private VertexPositionColorTexture[] vertices;
        private BasicEffect effect;

        private Vector2 lastPlayerCenter;
        private Vector2[] pos = new Vector2[20];
        public float length;
        private int time;
        private Player Owner;
        private int ColorSelection;

        private static readonly Color[] V8Colors =
        {
            Color.White,
            Color.Yellow,
            Color.OrangeRed,
            Color.Red,
            Color.Transparent
        };

        private static readonly Color[] V8000Colors =
{
            Color.White,
            Color.LightBlue,
            Color.LightCyan,
            Color.Cyan,
            Color.Transparent
        };
        Vector2 GetPlayerDrawCenter(Player p)
        {
            return p.Center + new Vector2(0f, p.gfxOffY);
        }

        public ThrusterParticle(Player player, Vector2 direction, int lifetime, float scale, int colors)
        {
            Owner = player;

            Position = player.Center;
            Velocity = direction; 
            Scale = scale;
            Lifetime = lifetime;
            ColorSelection = colors;

            Vector2 start = GetPlayerDrawCenter(player);
            Vector2 dir = direction;

            if (dir == Vector2.Zero)
                dir = Vector2.UnitX;

            dir.Normalize();

            length = scale;
            Vector2 tip = start + dir * length;

            for (int i = 0; i < pos.Length; i++)
            {
                float t = i / (float)(pos.Length - 1);
                pos[i] = Vector2.Lerp(start, tip, t);
            }

            lastPlayerCenter = start;
        }

        public override void Update()
        {
            Path[0] = GetPlayerDrawCenter(Owner);
            Path[^1] = Position;

            float lifeProgress = 1f - (time / (float)Lifetime);
            if (lifeProgress < 0f)
                lifeProgress = 0f;
            if (lifeProgress > 1f)
                lifeProgress = 1f;

            time++;

        }

        private Color SampleGradient(float t)
        {
            Color[] ThrusterGradient = ColorSelection == 1 ? V8000Colors : V8Colors;

            t = MathHelper.Clamp(t, 0, 1);
            float scaled = t * (ThrusterGradient.Length - 1);

            int index = (int)scaled;
            int next = Math.Min(index + 1, ThrusterGradient.Length - 1);

            float interp = scaled - index;

            return Color.Lerp(ThrusterGradient[index], ThrusterGradient[next], interp);
        }

        private void BuildPrimitive()
        {
            int count = pos.Length - 1;

            if (vertices == null || vertices.Length != count * 2)
                vertices = new VertexPositionColorTexture[count * 2];

            float width = 18f;
            float lifeProgress = 1f - (time / (float)Lifetime);

            float widthFactor = MathF.Sin(lifeProgress);

            for (int i = 0; i < count; i++)
            {
                Vector2 current = pos[i];

                Vector2 direction;

                if (i == count - 1)
                    direction = pos[i] - pos[i - 1];
                else
                    direction = pos[i + 1] - pos[i];

                if (direction == Vector2.Zero)
                    direction = Vector2.UnitX;

                direction.Normalize();

                Vector2 normal = direction.RotatedBy(MathHelper.PiOver2);

                float progress = i / (float)(count - 1);
                float profile = 0;
                if (length < 100)
                    profile = (float)Math.Sinh(progress * MathHelper.PiOver2) * 1.8f + 0.2f;
                else
                    profile = (float)Math.Sin(progress * Math.PI) * 0.8f + 0.2f;

                float scaledWidth = width * profile * widthFactor;

                Vector2 left = current - normal * scaledWidth;
                Vector2 right = current + normal * scaledWidth;

                Color color = SampleGradient(progress);

                vertices[i * 2] = new VertexPositionColorTexture(
                    new Vector3(left - Main.screenPosition, 0),
                    color,
                    new Vector2(progress, 0));

                vertices[i * 2 + 1] = new VertexPositionColorTexture(
                    new Vector3(right - Main.screenPosition, 0),
                    color,
                    new Vector2(progress, 1));
            }
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            if (pos == null || pos.Length < 2)
                return;

            Vector2 drawCenter = GetPlayerDrawCenter(Owner);
            Vector2 delta = drawCenter - lastPlayerCenter;

            for (int i = 0; i < pos.Length; i++)
                pos[i] += delta;

            lastPlayerCenter = drawCenter;
            BuildPrimitive();

            GraphicsDevice device = Main.graphics.GraphicsDevice;

            RasterizerState rasterizer = new RasterizerState { CullMode = CullMode.None, FillMode = FillMode.Solid };

            effect ??= new BasicEffect(device)
            {
                VertexColorEnabled = true,
                TextureEnabled = false,
                Projection = Matrix.CreateOrthographicOffCenter(
                    0,
                    Main.screenWidth,
                    Main.screenHeight,
                    0,
                    -1,
                    1)
            };
            effect.World = Matrix.Identity;
            effect.View = Main.GameViewMatrix.ZoomMatrix;

            device.RasterizerState = rasterizer;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, default, device.RasterizerState, null, Main.GameViewMatrix.TransformationMatrix);
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                device.DrawUserPrimitives(
                    PrimitiveType.TriangleStrip,
                    vertices,
                    0,
                    vertices.Length - 2);
            }
        }
    }
}
