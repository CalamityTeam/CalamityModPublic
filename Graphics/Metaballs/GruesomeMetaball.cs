using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Graphics.Metaballs
{
    public class GruesomeMetaball : Metaball
    {
        public class GruesomeParticle
        {
            public float Size;

            public Vector2 Velocity;

            public Vector2 Center;

            public GruesomeParticle(Vector2 center, Vector2 velocity, float size)
            {
                Center = center;
                Velocity = velocity;
                Size = size;
            }

            public void Update()
            {
                Size *= 0.94f;
                Center += Velocity;
                Velocity *= 0.96f;
            }
        }

        private static List<Asset<Texture2D>> layerAssets;

        public static List<GruesomeParticle> Particles
        {
            get;
            private set;
        } = new();

        // Check if there are any extraneous particles or if the Gruesome Eminence projectile is present when deciding if this particle should be drawn.
        public override bool AnythingToDraw => Particles.Any() || CalamityUtils.AnyProjectiles(ModContent.ProjectileType<SpiritCongregation>());

        public override IEnumerable<Texture2D> Layers
        {
            get
            {
                for (int i = 0; i < layerAssets.Count; i++)
                    yield return layerAssets[i].Value;
            }
        }

        public override MetaballDrawLayer DrawContext => MetaballDrawLayer.AfterProjectiles;

        public override Color EdgeColor => new(61, 6, 2);

        public override void Load()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            // Load layer assets.
            layerAssets = new();

            for (int i = 1; i <= 5; i++)
                layerAssets.Add(ModContent.Request<Texture2D>($"CalamityMod/Graphics/Metaballs/GruesomeEminence_Ghost_Layer{i}", AssetRequestMode.ImmediateLoad));
        }

        public override void ClearInstances() => Particles.Clear();

        public override void Update()
        {
            // Update all particle instances.
            // Once sufficiently small, they vanish.
            for (int i = 0; i < Particles.Count; i++)
                Particles[i].Update();
            Particles.RemoveAll(p => p.Size <= 2.5f);
        }

        public static void SpawnParticle(Vector2 position, Vector2 velocity, float size) =>
            Particles.Add(new(position, velocity, size));

        public override Vector2 CalculateManualOffsetForLayer(int layerIndex)
        {
            switch (layerIndex)
            {
                // Background.
                case 0:
                    return Vector2.UnitX * Main.GlobalTimeWrappedHourly * 0.03f;

                // Gaseous skulls.
                case 1:
                    Vector2 offset = Vector2.One * (float)Math.Cos(Main.GlobalTimeWrappedHourly * 0.041f) * 2f;
                    offset = offset.RotatedBy((float)Math.Cos(Main.GlobalTimeWrappedHourly * 0.08f) * 0.97f);
                    return offset;

                // Spooky faces 1.
                case 2:
                    offset = (Main.GlobalTimeWrappedHourly * 2.02f).ToRotationVector2() * 0.036f;
                    offset.Y += (float)Math.Cos(Main.GlobalTimeWrappedHourly * 0.161f) * 0.5f + 0.5f;
                    return offset;

                // Spooky faces 2.
                case 3:
                    offset = Vector2.UnitX * Main.GlobalTimeWrappedHourly * -0.04f + (Main.GlobalTimeWrappedHourly * 1.89f).ToRotationVector2() * 0.03f;
                    offset.Y += CalamityUtils.PerlinNoise2D(Main.GlobalTimeWrappedHourly * 0.187f, Main.GlobalTimeWrappedHourly * 0.193f, 2, 466920161) * 0.025f;
                    return offset;

                // Spooky faces 3.
                case 4:
                    offset = Vector2.UnitX * Main.GlobalTimeWrappedHourly * 0.037f + (Main.GlobalTimeWrappedHourly * 1.77f).ToRotationVector2() * 0.04725f;
                    offset.Y += CalamityUtils.PerlinNoise2D(Main.GlobalTimeWrappedHourly * 0.187f, Main.GlobalTimeWrappedHourly * 0.193f, 2, 577215664) * 0.05f;
                    return offset;
            }

            return Vector2.Zero;
        }

        public override void DrawInstances()
        {
            Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/BasicCircle").Value;

            // Draw all particles.
            foreach (GruesomeParticle particle in Particles)
            {
                Vector2 drawPosition = particle.Center - Main.screenPosition;
                Vector2 origin = tex.Size() * 0.5f;
                Vector2 scale = Vector2.One * particle.Size / tex.Size();
                Main.spriteBatch.Draw(tex, drawPosition, null, Color.White, 0f, origin, scale, 0, 0f);
            }

            // Draw gruesome eminence ghost heads.
            foreach (Projectile eminence in CalamityUtils.AllProjectilesByID(ModContent.ProjectileType<SpiritCongregation>()))
                eminence.ModProjectile<SpiritCongregation>().DrawHeadForMetaball();
        }
    }
}
