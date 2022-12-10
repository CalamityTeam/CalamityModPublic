using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent.Shaders;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class FreedomStarBeam : ModProjectile
    {
        private const int Lifetime = 600;
        private const int TimeToReachMaxSize = 240;
        private const int TimeToShrink = 80;
        private const float MaxBeamScale = 3f;
        private const int DustType = 226;

        private const float MaxBeamLength = 2400f;
        private const float BeamTileCollisionWidth = 1f;
        private const float BeamHitboxCollisionWidth = 15f;
        private const int NumSamplePoints = 3;
        private const float BeamLengthChangeFactor = 0.5f;

        private const float OuterBeamOpacityMultiplier = 0.82f;
        private const float InnerBeamOpacityMultiplier = 0.2f;
        private const float MaxBeamBrightness = 0.75f;

        private const float MainDustBeamEndOffset = 14.5f;
        private const float SidewaysDustBeamEndOffset = 4f;
        private const float BeamRenderTileOffset = 10.5f;
        private const float BeamLengthReductionFactor = 14.5f;

        public Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Freedom Star Beam");
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hide = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.alpha = 0;
            // The beam itself still stops on tiles, but its invisible "source" projectile ignores them.
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
            Projectile.timeLeft = Lifetime;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
                Projectile.velocity = -Vector2.UnitY;

            if (Main.projectile[(int)Projectile.ai[1]].active && Main.projectile[(int)Projectile.ai[1]].type == ModContent.ProjectileType<FreedomStarHoldout>())
            {
                Vector2 value27 = Vector2.Normalize(Main.projectile[(int)Projectile.ai[1]].velocity);
                Projectile.position = Main.projectile[(int)Projectile.ai[1]].Center + value27 * 16f - new Vector2(Projectile.width, Projectile.height) / 2f + new Vector2(0f, 0f - Main.projectile[(int)Projectile.ai[1]].gfxOffY);
                Projectile.velocity = Vector2.Normalize(Main.projectile[(int)Projectile.ai[1]].velocity);
            }
            else
            {
                Projectile.Kill();
                return;
            }

            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
                Projectile.velocity = -Vector2.UnitY;

            float rotation = Projectile.velocity.ToRotation();
            Projectile.rotation = rotation - (float)Math.PI / 2f;
            Projectile.velocity = rotation.ToRotationVector2();

            // Figure out the scale.
            if (Projectile.timeLeft > Lifetime - TimeToReachMaxSize)
                Projectile.localAI[0]++;
            else if (Projectile.timeLeft < TimeToShrink)
                Projectile.localAI[0] -= TimeToReachMaxSize / TimeToShrink;

            // Set initial damage.
            if (Projectile.localAI[1] == 0f)
                Projectile.localAI[1] = Projectile.damage;

            // Reduce the "power" and thus scale of the projectile over its lifetime.
            float power = MathHelper.Clamp(Projectile.localAI[0] / TimeToReachMaxSize, 0.1f, 1f);
            Projectile.scale = MaxBeamScale * power;
            Projectile.damage = (int)MathHelper.Lerp(Projectile.localAI[1], Projectile.localAI[1] * 3f, power);

            // Perform a laser scan to calculate the correct length of the beam.
            float[] laserScanResults = new float[NumSamplePoints];

            // A minimum width is forced for the beam scan to prevent massive lag when fired into open areas.
            float scanWidth = Projectile.scale < 1f ? 1f : Projectile.scale;
            Collision.LaserScan(Projectile.Center, Projectile.velocity, BeamTileCollisionWidth * scanWidth, MaxBeamLength, laserScanResults);
            float avg = 0f;
            for (int i = 0; i < laserScanResults.Length; i++)
                avg += laserScanResults[i];
            avg /= NumSamplePoints;
            Projectile.ai[0] = MathHelper.Lerp(Projectile.ai[0], avg, BeamLengthChangeFactor);

            // X = beam length.
            // Y = beam width.
            Vector2 beamDims = new Vector2(Projectile.velocity.Length() * Projectile.ai[0], Projectile.width * Projectile.scale);
            Color beamColor = Color.Cyan;
            ProduceBeamDust(beamColor);

            // If the game is rendering (i.e. isn't a dedicated server), make the beam disturb water.
            if (Main.netMode != NetmodeID.Server)
            {
                WaterShaderData wsd = (WaterShaderData)Filters.Scene["WaterDistortion"].GetShader();

                // A universal time-based sinusoid which updates extremely rapidly.
                // GlobalTimeWrappedHourly is 0 to 3600, measured in seconds.
                float waveSine = 0.1f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 20f);
                Vector2 ripplePos = Projectile.position + new Vector2(beamDims.X * 0.5f, 0f).RotatedBy(Projectile.rotation);

                // WaveData is encoded as a Color.
                // Not sure why, considering Vector3 exists.
                Color waveData = new Color(0.5f, 0.1f * Math.Sign(waveSine) + 0.5f, 0f, 1f) * Math.Abs(waveSine);
                wsd.QueueRipple(ripplePos, waveData, beamDims, RippleShape.Square, Projectile.rotation);
            }

            // Make the beam cast light along its length.
            // v3_1 is an unnamed decompiled variable which is the color of the light cast by DelegateMethods.CastLight.
            DelegateMethods.v3_1 = beamColor.ToVector3() * power * MaxBeamBrightness;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.ai[0], beamDims.Y, DelegateMethods.CastLight);

            // Also add some fast moving dust.
            if (Projectile.timeLeft == Lifetime)
            {
                int dustCount = Main.rand.Next(10, 30);
                for (int i = 0; i < dustCount; i++)
                {
                    float dustProgressAlongBeam = Projectile.ai[0] * Main.rand.NextFloat(0f, 0.7f);
                    Vector2 dustPosition = Projectile.Center + dustProgressAlongBeam * Projectile.velocity + Projectile.velocity.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-6f, 6f) * Projectile.scale;

                    Dust dust = Dust.NewDustPerfect(dustPosition, DustType, Projectile.velocity * Main.rand.NextFloat(2f, 16f), 0, beamColor);
                    dust.color = beamColor;
                    dust.noGravity = true;
                    dust.scale = 0.7f;
                }
            }
        }

        // Determines whether the specified target hitbox is intersecting with the beam.
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // If the target is touching the beam's hitbox (which is a small rectangle vaguely overlapping the host crystal), that's good enough.
            if (projHitbox.Intersects(targetHitbox))
                return true;

            // Otherwise, perform an AABB line collision check to check the whole beam.
            float _ = float.NaN;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.ai[0], BeamHitboxCollisionWidth * Projectile.scale, ref _);
        }

        // Spawn lunar flare explosions on hit.
        /*public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            // Ensure that the hit direction is correct when hitting enemies.
            hitDirection = (Projectile.Center.X < target.Center.X).ToDirectionInt();
            float targetPolarity = target.PolarityNPC().CurPolarity;
            //If a polarity beam hits the target with the opposite polarity, the damage dealt increases by 20%
            if (polarity * targetPolarity < 0)
            {
                double newDamage = damage * 1.2;
                damage = (int)newDamage;

                for (int i = 0; i < 4; i++)
                {
                    Color sparkColor = AdamantiteParticleAccelerator.LightColors[polarity < 0 ? 1 : 0];

                    Vector2 sparkSpeed = Owner.DirectionTo(target.Center).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2)) * Main.rand.NextFloat(8f, 17f);
                    Particle Spark = new CritSpark(target.Center, sparkSpeed, Color.White, sparkColor, 0.7f + Main.rand.NextFloat(0, 0.6f), 30, 0.4f, 0.6f);
                    GeneralParticleHandler.SpawnParticle(Spark);
                }
            }
        }*/

        public override bool PreDraw(ref Color lightColor)
        {
            // If the beam has its velocity set to zero, don't draw anything.
            if (Projectile.velocity == Vector2.Zero)
                return false;

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            float beamLength = Projectile.ai[0];
            Vector2 centerFloored = Projectile.Center.Floor() + Projectile.velocity * Projectile.scale * BeamRenderTileOffset;
            Vector2 scaleVec = new Vector2(Projectile.scale);

            // Reduce the beam length proportional to its square area to reduce block penetration.
            beamLength -= BeamLengthReductionFactor * Projectile.scale * Projectile.scale;

            // f_1 is an unnamed decompiled variable whose function is unknown. Leave it at 1.
            DelegateMethods.f_1 = 1f;

            Vector2 beamStartPos = centerFloored - Main.screenPosition;
            Vector2 beamEndPos = beamStartPos + Projectile.velocity * beamLength;
            Utils.LaserLineFraming llf = new Utils.LaserLineFraming(DelegateMethods.RainbowLaserDraw);

            // Draw the outer beam.
            // c_1 is an unnamed decompiled variable which is the render color of the beam drawn by DelegateMethods.RainbowLaserDraw.
            Color beamColor = Color.Cyan;
            DelegateMethods.c_1 = beamColor * OuterBeamOpacityMultiplier * Projectile.Opacity;
            Utils.DrawLaser(Main.spriteBatch, tex, beamStartPos, beamEndPos, scaleVec, llf);

            // Draw the inner beams, each with reduced size and whiter color
            for (int i = 0; i < 5; i++)
            {
                beamColor = Color.Lerp(beamColor, Color.White, 0.4f);
                scaleVec *= 0.85f - i * 0.15f;
                DelegateMethods.c_1 = beamColor * InnerBeamOpacityMultiplier * Projectile.Opacity;
                Utils.DrawLaser(Main.spriteBatch, tex, beamStartPos, beamEndPos, scaleVec, llf);
            }

            return false;
        }

        private void ProduceBeamDust(Color beamColor)
        {
            // Create a few dust per frame a small distance from where the beam ends.
            Vector2 laserEndPos = Projectile.Center + Projectile.velocity * (Projectile.ai[0] - MainDustBeamEndOffset * Projectile.scale);

            for (int i = 0; i < 2; i++)
            {
                // 50% chance for the dust to come off on either side of the beam.
                float dustAngle = Projectile.rotation + (Main.rand.NextBool() ? 1f : -1f) * MathHelper.PiOver2;
                float dustStartDist = Main.rand.NextFloat(1f, 1.8f);
                Vector2 dustVel = dustAngle.ToRotationVector2() * dustStartDist;
                int d = Dust.NewDust(laserEndPos, 0, 0, DustType, dustVel.X, dustVel.Y, 0, beamColor);
                Main.dust[d].color = beamColor;
                Main.dust[d].noGravity = true;
                Main.dust[d].scale = 0.7f;

                // Scale up dust with the projectile if it's large.
                if (Projectile.scale > 1f)
                {
                    Main.dust[d].velocity *= Projectile.scale;
                    Main.dust[d].scale *= Projectile.scale;
                }

                // If the beam isn't at max scale, then make additional smaller dust.
                if (Projectile.scale != MaxBeamScale)
                {
                    Dust smallDust = Dust.CloneDust(d);
                    smallDust.scale /= 2f;
                }
            }

            // Low chance every frame to spawn a large "directly sideways" dust which doesn't move.
            if (Main.rand.NextBool(5))
            {
                // Velocity, flipped sideways, times -50% to 50% of beam width.
                Vector2 dustOffset = Projectile.velocity.RotatedBy(MathHelper.PiOver2) * (Main.rand.NextFloat() - 0.5f) * Projectile.width;
                Vector2 dustPos = laserEndPos + dustOffset - Vector2.One * SidewaysDustBeamEndOffset;
                int d = Dust.NewDust(dustPos, 8, 8, DustType, 0f, 0f, 100, beamColor, 1.2f);
                Main.dust[d].velocity *= 0.5f;

                // Force the dust to always move downwards, never upwards.
                Main.dust[d].velocity.Y = -Math.Abs(Main.dust[d].velocity.Y);
            }
        }

        // Automatically iterates through every tile the laser is overlapping to cut grass at all those locations.
        public override void CutTiles()
        {
            // tilecut_0 is an unnamed decompiled variable which tells CutTiles how the tiles are being cut (in this case, via a projectile).
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Utils.TileActionAttempt cut = DelegateMethods.CutTiles;
            Vector2 beamStartPos = Projectile.Center;
            Vector2 beamEndPos = beamStartPos + Projectile.velocity * Projectile.ai[0];
            Utils.PlotTileLine(beamStartPos, beamEndPos, Projectile.width * Projectile.scale, cut);
        }
    }
}
