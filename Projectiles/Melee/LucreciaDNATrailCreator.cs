using System;
using System.Collections.Generic;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class LucreciaDNATrailCreator : ModProjectile, ILocalizedModType
    {
        private List<Vector2> oldPositionsLeft = new List<Vector2>();
        private List<Vector2> oldPositionsRight = new List<Vector2>();
        private int trailTimer = 0;
        // The timer is initialized with a delay. It will reset to 6 after the first shot.
        private int middleStreakTimer = 40;
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 170;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.extraUpdates = 12;
            Projectile.timeLeft = 2400;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.alpha = 255;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float hitboxSize = Projectile.width * Projectile.scale;
            return CalamityUtils.CircularHitboxCollision(Projectile.Center, hitboxSize, targetHitbox);
        }
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            trailTimer++;

            // Very volatile values...
            float amplitude = 86f;
            float frequency = 0.039f;

            // Makes a perpendicular/mirrored trail, AKA a double helix
            Vector2 perpendicular = Vector2.Normalize(new Vector2(-Projectile.velocity.Y, Projectile.velocity.X));

            // Calculate the sine wave value.
            float sineWave = (float)Math.Cos(trailTimer * frequency);

            // Calculate offset based on the pattern of the sinewave and subtract the proj's dimensions to be accurate
            Vector2 offsetLeft = (perpendicular * amplitude * sineWave);
            Vector2 offsetRight = (-perpendicular * amplitude * sineWave);

            Vector2 adjustedOffsetLeft = offsetLeft - new Vector2(Projectile.width, Projectile.height);
            Vector2 adjustedOffsetRight = offsetRight - new Vector2(Projectile.width, Projectile.height);

            // Store newest trail positions as points to render later
            oldPositionsLeft.Add(Projectile.Center + adjustedOffsetLeft);
            oldPositionsRight.Add(Projectile.Center + adjustedOffsetRight);

            // The timer will now correctly decrease once per AI update.
            middleStreakTimer--;

            if (middleStreakTimer <= 0)
            {
                // Reset the timer for the next shot.
                middleStreakTimer = 6;

                // Spawn the main trail
                Particle spark = new CustomSpark(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedBy(MathHelper.ToRadians(-170f)), "CalamityMod/Particles/BloomCircle", false, 18, 0.24f * Projectile.scale, Color.MediumPurple * 1.3f, new Vector2(1f, 2.5f), true, true, shrinkSpeed: 0.25f, glowOpacity: 0.4f);
                GeneralParticleHandler.SpawnParticle(spark);
                Particle spark2 = new CustomSpark(Projectile.Center + perpendicular, Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedBy(MathHelper.ToRadians(170f)), "CalamityMod/Particles/BloomCircle", false, 18, 0.24f * Projectile.scale, Color.CornflowerBlue * 1.3f, new Vector2(1f, 2.5f), true, true, shrinkSpeed: 0.25f, glowOpacity: 0.4f);
                GeneralParticleHandler.SpawnParticle(spark2);
            }

            // Chance to spawn small light particles along the trail
            if (trailTimer % 7 == 0 && Main.rand.NextBool())
            {
                // Use the unmodified offsets to align particle positions w/ the prim trails
                Vector2 purpleTrailOrigin = Projectile.Center + offsetLeft;
                Vector2 blueTrailOrigin = Projectile.Center + offsetRight;

                Particle purpleLightEmission = new SquishyLightParticle(purpleTrailOrigin, -Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.5f) * Main.rand.NextFloat(2f, 4f), Main.rand.NextFloat(0.3f, 0.6f) * Projectile.scale, Color.MediumPurple, Main.rand.Next(18, 46), 1, 1.5f);
                Particle blueLightEmission = new SquishyLightParticle(blueTrailOrigin, -Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.5f) * Main.rand.NextFloat(2f, 4f), Main.rand.NextFloat(0.3f, 0.6f) * Projectile.scale, Color.CornflowerBlue, Main.rand.Next(18, 46), 1, 1.5f);

                GeneralParticleHandler.SpawnParticle(blueLightEmission);
                GeneralParticleHandler.SpawnParticle(purpleLightEmission);
            }

            // Remove old positions after 150
            int maxTrailLength = 150;
            if (oldPositionsLeft.Count > maxTrailLength)
                oldPositionsLeft.RemoveAt(0);
            if (oldPositionsRight.Count > maxTrailLength)
                oldPositionsRight.RemoveAt(0);
        }

        private float WidthFunction(float completionRatio, Vector2 vertexPos)
        {
            return MathHelper.Lerp(12f * Projectile.scale, 0f, completionRatio);
        }


        private Color LeftColorFunction(float completionRatio, Vector2 vertexPos)
        {
            Color baseColor = Color.MediumPurple * 1.3f;

            float alphaScaling = -4 * completionRatio * (completionRatio - 1);
            return baseColor * alphaScaling;
        }
        private Color RightColorFunction(float completionRatio, Vector2 vertexPos)
        {
            Color baseColor = Color.CornflowerBlue * 1.3f;

            float alphaScaling = -4 * completionRatio * (completionRatio - 1);
            return baseColor * alphaScaling;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            MiscShaderData trailShader = GameShaders.Misc["CalamityMod:TrailStreak"];
            trailShader.SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/SylvestaffStreak"));

            // Use the separate color functions for each trail
            PrimitiveRenderer.RenderTrail(oldPositionsLeft, new PrimitiveSettings(WidthFunction, LeftColorFunction, (_,_) => Projectile.Size, pixelate: false, shader: trailShader));
            PrimitiveRenderer.RenderTrail(oldPositionsRight, new PrimitiveSettings(WidthFunction, RightColorFunction, (_,_) => Projectile.Size, pixelate: false, shader: trailShader));
            return false;
        }
    }
}
