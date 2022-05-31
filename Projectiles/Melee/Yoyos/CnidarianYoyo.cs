using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using System.Linq;
using System;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class VerletSimulatedSegment
    {
        public Vector2 position, oldPosition;
        public bool locked;

        public VerletSimulatedSegment(Vector2 _position, bool _locked = false)
        {
            position = _position;
            oldPosition = _position;
            locked = _locked;
        }
    }

    public class CnidarianYoyo : ModProjectile
    {
        public const int SegmentCount = 10;
        public const float SegmentDistance = 20;
        public const int FadeoutTime = 20;

        internal PrimitiveTrail TrailRenderer;

        public List<VerletSimulatedSegment> Segments;
        public Player Owner => Main.player[Projectile.owner];
        public ref float Initialized => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cnidarian");
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 7f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 240f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 11f;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.scale = 1.15f;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public void SetOrigin(Vector2 position)
        {
            Projectile.Center = position;
            if ((Projectile.Center - Owner.Center).Length() > 380f)
                Projectile.Center = Owner.Center + (Projectile.Center - Owner.Center).SafeNormalize(Vector2.One) * 380f;
        }

        public void Initialize()
        {
            //Initialize the segments
            SetOrigin(Owner.Calamity().mouseWorld);

            Segments = new List<VerletSimulatedSegment>(SegmentCount);
            for (int i = 0; i < SegmentCount; i++)
            {
                VerletSimulatedSegment segment = new VerletSimulatedSegment(Projectile.Center + Vector2.UnitY * SegmentDistance * i);
                Segments.Add(segment);
            }

            Segments[0].locked = true;

            foreach (VerletSimulatedSegment segment in Segments)
            {
                Particles.CritSpark particle = new Particles.CritSpark(segment.position, Vector2.Zero, Color.White, Color.Cyan, 1f, 10);
                Particles.GeneralParticleHandler.SpawnParticle(particle);
            }


            Initialized = 1f;
            return;
        }

        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;

            if (Initialized == 0f)
                Initialize();

            if (Owner.channel)
                Projectile.timeLeft = FadeoutTime;

            SetOrigin(Projectile.Center.MoveTowards(Owner.Calamity().mouseWorld), 30f));


            SimulateSegments();

            CalamityGlobalProjectile.MagnetSphereHitscan(Projectile, 300f, 6f, 180f, 5, ModContent.ProjectileType<Seashell>(), 0.75);

            if ((Projectile.Center - Owner.Center).Length() > 3200f) //200 blocks
                Projectile.Kill();
        }

        public void SimulateSegments()
        {
            Segments[0].oldPosition = Segments[0].position;
            Segments[0].position = Projectile.Center;

            //https://youtu.be/PGk0rnyTa1U?t=400 we use verlet integration chains here
            float movementLenght = Projectile.velocity.Length();
            foreach (VerletSimulatedSegment segment in Segments)
            {
                if (!segment.locked)
                {
                    Vector2 positionBeforeUpdate = segment.position;

                    segment.position += (segment.position - segment.oldPosition); // This adds conservation of energy to the segments. This makes it super bouncy and shouldnt be used but it's really funny
                    segment.position += Vector2.UnitY * 0.1f; //=> This adds gravity to the segments. 

                    segment.oldPosition = positionBeforeUpdate;
                }
            }

            for (int k = 0; k < 10; k++)
            {
                for (int j = 0; j < SegmentCount - 1; j++)
                {
                    VerletSimulatedSegment pointA = Segments[j];
                    VerletSimulatedSegment pointB = Segments[j + 1];
                    Vector2 segmentCenter = (pointA.position + pointB.position) / 2f;
                    Vector2 segmentDirection = Utils.SafeNormalize(pointA.position - pointB.position, Vector2.UnitY);

                    if (!pointA.locked)
                        pointA.position = segmentCenter + segmentDirection * SegmentDistance / 2f;

                    if (!pointB.locked)
                        pointB.position = segmentCenter - segmentDirection * SegmentDistance / 2f;

                    Segments[j] = pointA;
                    Segments[j + 1] = pointB;
                }
            }
        }

        public float PrimWidthFunction(float completionRatio)
        {
            return 2f;
        }

        public Color PrimColorFunction(float completionRatio)
        {
            Color startingColor = Color.Cyan * (Projectile.timeLeft / (float)FadeoutTime);
            Color endColor = Color.DarkCyan * 0f;
            return Color.Lerp(endColor, startingColor, (float)Math.Pow(completionRatio, 1.5D)) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (TrailRenderer is null)
                TrailRenderer = new PrimitiveTrail(PrimWidthFunction, PrimColorFunction);

            Vector2[] segmentPositions = Segments.Select(x => x.position).ToArray();

            TrailRenderer.Draw(segmentPositions, -Main.screenPosition, 66);


            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            float rotation = (Segments[SegmentCount - 1].position - Segments[SegmentCount - 2].position).ToRotation() - MathHelper.PiOver2;


            Main.EntitySpriteDraw(tex, Segments[SegmentCount - 1].position - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * (Projectile.timeLeft / (float)FadeoutTime), rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
