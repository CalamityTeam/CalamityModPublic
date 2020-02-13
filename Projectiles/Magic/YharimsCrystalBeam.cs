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

namespace CalamityMod.Projectiles.Magic
{
    public class YharimsCrystalBeam : ModProjectile
    {
        private const float PiOver6 = MathHelper.Pi / 6f;

        private const float FullDamageMultiplier = 3f;

        private const float BeamPosOffset = 16f;
        private const float MaxBeamScale = 1.8f;

        private const float MaxBeamLength = 2400f;
        private const float BeamTileCollisionWidth = 0f;
        private const float BeamHitboxCollisionWidth = 22f;
        private const int NumSamplePoints = 2;
        private const float LaserLerpRatio = 0.75f;

        private const float OuterBeamOpacityMultiplier = 0.75f;
        private const float InnerBeamOpacityMultiplier = 0.1f;
        private const float BeamLightBrightness = 0.75f;

        private const float MainDustBeamEndOffset = 14.5f;
        private const float SidewaysDustBeamEndOffset = 4f;
        private const float BeamRenderTileOffset = 10.5f;
        private const float BeamLengthReductionFactor = 14.5f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yermes Christal");
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[1] = reader.ReadSingle();
        }

        private Color GetBeamColor()
        {
            float customHue = GetHue(projectile.ai[0]);
            float sat = 0.66f;
            Color c = Main.hslToRgb(customHue, sat, 0.53f);
            c.A = 64; // 127
            return c;
        }

        // indexing = 0, 1, 2, 3, 4, or 5 (it's beam ID, stored in projectile.ai[0])
        private float GetHue(float indexing)
        {
            string name = Main.player[projectile.owner].name ?? "";
            if(Main.player[projectile.owner].active)
            {
                switch (name)
                {
                    case "Fabsol":
                    case "Ziggums":
                        return 2f;
                    case "Poly":
                        return 0.83f;
                    case "Zach":
                        return 1.5f + (float)Math.Cos(Main.time / 180.0 * Math.PI * 2.0) * 0.1f;
                    case "Grox the Great":
                        return 1.27f;
                    case "Jenosis":
                        return 0.65f + (float)Math.Cos(Main.time / 180.0 * Math.PI * 2.0) * 0.1f;
                    case "DM DOKURO":
                        return 0f;
                    case "Uncle Danny":
                    case "Phoenix":
                        return 1.7f + (float)Math.Cos(Main.time / 180.0 * Math.PI * 2.0) * 0.07f;
                    case "Minecat":
                        return 0.15f + (float)Math.Cos(Main.time / 180.0 * Math.PI * 2.0) * 0.07f;
                    case "Khaelis":
                        return 1.15f + (float)Math.Cos(Main.time / 180.0 * Math.PI * 2.0) * 0.18f;
                    case "Purple Necromancer":
                        return 1.7f + (float)Math.Cos(Main.time / 120.0 * Math.PI * 2.0) * 0.05f;
                    case "gamagamer64":
                        return 0.83f + (float)Math.Cos(Main.time / 120.0 * Math.PI * 2.0) * 0.03f;
                    case "Svante":
                        return 1.4f + (float)Math.Cos(Main.time / 180.0 * Math.PI * 2.0) * 0.06f;
                    case "Puff":
                        return 0.31f + (float)Math.Cos(Main.time / 120.0 * Math.PI * 2.0) * 0.13f;
                    case "Leviathan":
                        return 1.9f + (float)Math.Cos(Main.time / 180.0 * Math.PI * 2.0) * 0.1f;
                    case "Testdude":
                        return Main.rand.NextFloat();
                }
            }
            return (indexing / 6f) % 0.12f;
        }

        // projectile.ai[0] = 0, 1, 2, 3, 4 or 5, chosen at spawn = Which beam ID this is (they all have different rotations and colors)
        // projectile.ai[1] = Index of the crystal projectile "hosting" this beam.
        public override void AI()
        {
            // Sanity check velocity to prevent crashes
            if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
                projectile.velocity = -Vector2.UnitY;

            // If something has gone wrong with either the beam or the host crystal, destroy the beam.
            Projectile hostCrystal = Main.projectile[(int)projectile.ai[1]];
            if (projectile.type != ModContent.ProjectileType<YharimsCrystalBeam>() || !hostCrystal.active || hostCrystal.type != ModContent.ProjectileType<YharimsCrystalPrism>())
            {
                projectile.Kill();
                return;
            }

            Vector2 hostCrystalDir = Vector2.Normalize(hostCrystal.velocity);

            // -2.5, -1.5, 0.5, 1.5, or 2.5 based on the beam ID. Used to make the beams orient differently.
            float beamIdOffset = (int)projectile.ai[0] - 2.5f;
            float projScale;
            float beamSpread;
            float spinRate;
            float beamStartSidewaysOffset;
            float beamStartForwardsOffset;
            if (hostCrystal.ai[0] < 180f)
            {
                float chargeRatio = hostCrystal.ai[0] / 180f;
                projScale = MathHelper.Lerp(0f, MaxBeamScale, chargeRatio);
                beamSpread = MathHelper.Lerp(1.22f, 0f, chargeRatio);
                beamStartSidewaysOffset = MathHelper.Lerp(20f, 6f, chargeRatio);
                beamStartForwardsOffset = MathHelper.Lerp(-17f, -13f, chargeRatio);

                // From 0 to 120 frames, the opacity scales up from 0% to 40%.
                // Spin rate increases slowly for the first 120 frames.
                if (hostCrystal.ai[0] < 120f)
                {
                    projectile.Opacity = hostCrystal.ai[0] / 120f * 0.4f;
                    spinRate = 20f - 4f * (hostCrystal.ai[0] / 120f);
                }

                // Between 120 and 180 frames, the opacity scales up from 40% to 100%.
                // Spin rate increases dramatically during these frames.
                else
                {
                    projectile.Opacity = 0.4f + (hostCrystal.ai[0] - 120f) / 60f * 0.6f;
                    spinRate = 16f - 10f * ((hostCrystal.ai[0] - 120f) / 60f);
                }
            }
            // Past 180 frames, these variables are fixed in place.
            else
            {
                projScale = MaxBeamScale;
                projectile.Opacity = 1f;
                beamSpread = 0f;
                spinRate = 6f;
                beamStartSidewaysOffset = 6f;
                beamStartForwardsOffset = -13f;
            }

            // The amount to which the angle changes reduces over time so that the beams look like they are focusing.
            float beamAngle = (hostCrystal.ai[0] + beamIdOffset * spinRate) / (spinRate * 6f) * MathHelper.TwoPi;
            Vector2 unitRot = Vector2.UnitY.RotatedBy(beamAngle);
            float sinusoidYOffset = unitRot.Y * PiOver6 * beamSpread;

            float hostCrystalAngle = hostCrystal.velocity.ToRotation();
            Vector2 yVec = new Vector2(4f, beamStartSidewaysOffset);
            Vector2 beamSpanVector = (unitRot * yVec).RotatedBy(hostCrystalAngle);

            // Calculate the beam's emanating position. Start with the crystal center (offset because we're setting projectile position, not center)
            projectile.Center = hostCrystal.Center;
            // projectile.position = hostCrystal.Center - projectile.Size / 2f;
            // Not sure what this step does.
            projectile.position += hostCrystalDir * BeamPosOffset + new Vector2(0f, -hostCrystal.gfxOffY);
            // Not sure what this step does.
            projectile.position += hostCrystal.velocity.ToRotation().ToRotationVector2() * beamStartForwardsOffset;
            // Not sure what this step does.
            projectile.position += beamSpanVector;
            // Not sure what this step does.
            projectile.velocity = hostCrystalDir.RotatedBy(sinusoidYOffset);
            // Scale the projectile up as the beam continues to be used.
            projectile.scale = projScale;

            // Update the damage based on the host crystal damage.
            // The host crystal itself updates its damage every frame based on the player's magic damage to account for Mana Sickness.
            projectile.damage = hostCrystal.damage;

            // If at full power, the beam deals significantly more damage.
            // Additionally, its interpolation starts exactly at the host crystal's center.
            if (hostCrystal.ai[0] >= 180f)
                projectile.damage = (int)(projectile.damage * FullDamageMultiplier);

            // The beam does not deal damage for the first 30 frames.
            projectile.friendly = hostCrystal.ai[0] > 30f;
            
            // Sanity check the projectile's velocity.
            if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
                projectile.velocity = -Vector2.UnitY;

            // Rotate the projectile's velocity by 90 degrees. Why this couldn't have been done earlier, I do not know.
            float velAngle = projectile.velocity.ToRotation();
            projectile.rotation = velAngle - MathHelper.PiOver2;
            projectile.velocity = velAngle.ToRotationVector2();

            // By default, the interpolation starts at the projectile's center.
            // However, if the host crystal is at full power, the interpolation starts at the host crystal's center.
            // Overriding that, if the player shoves the crystal into or through a wall, the interpolation starts at the player's center.
            Vector2 samplingPoint = projectile.Center;
            if(hostCrystal.ai[0] >= 180f)
                samplingPoint = hostCrystal.Center;
            if (!Collision.CanHitLine(Main.player[projectile.owner].Center, 0, 0, hostCrystal.Center, 0, 0))
                samplingPoint = Main.player[projectile.owner].Center;

            // Perform a laser scan and sum the results (not sure what that means...)
            float[] laserScanResults = new float[NumSamplePoints];
            Collision.LaserScan(samplingPoint, projectile.velocity, BeamTileCollisionWidth * projectile.scale, MaxBeamLength, laserScanResults);
            float sum = 0f;
            for (int i = 0; i < laserScanResults.Length; ++i)
                sum += laserScanResults[i];
            sum /= NumSamplePoints;

            // Pretty sure this sets the endpoint/length of the beam?
            projectile.localAI[1] = MathHelper.Lerp(projectile.localAI[1], sum, LaserLerpRatio);

            // Not sure what this check is, but if it fails the beam doesn't even attempt to draw.
            if (Math.Abs(projectile.localAI[1] - sum) < 100f && projectile.scale > 0.15f)
            {
                Color beamColor = GetBeamColor();

                // Create a few dust per frame a small distance from where the beam ends.
                Vector2 laserEndPos = projectile.Center + projectile.velocity * (projectile.localAI[1] - MainDustBeamEndOffset * projectile.scale);
                for (int i = 0; i < 2; ++i)
                {
                    // 50% chance for the dust to come off on either side of the beam.
                    float dustAngle = velAngle + (Main.rand.NextBool() ? 1f : -1f) * MathHelper.PiOver2;
                    float dustStartDist = Main.rand.NextFloat(1f, 1.8f);
                    Vector2 dustVel = dustAngle.ToRotationVector2() * dustStartDist;
                    int d = Dust.NewDust(laserEndPos, 0, 0, 244, dustVel.X, dustVel.Y, 0, new Color(255, Main.DiscoG, 53), 3.3f);
                    Main.dust[d].color = beamColor;
                    Main.dust[d].noGravity = true;
                    Main.dust[d].scale = 1.2f;

                    // Scale up dust with the projectile if it's large.
                    if (projectile.scale > 1f)
                    {
                        Main.dust[d].velocity *= projectile.scale;
                        Main.dust[d].scale *= projectile.scale;
                    }

                    // If the beam isn't at max scale, then make additional smaller dust.
                    if (projectile.scale != MaxBeamScale)
                    {
                        Dust smallDust = Dust.CloneDust(d);
                        smallDust.scale /= 2f;
                    }
                }

                // Low chance every frame to spawn a large "directly sideways" dust which doesn't move.
                if (Main.rand.NextBool(5))
                {
                    // Velocity, flipped sideways, times -50% to 50% of beam width.
                    Vector2 dustOffset = projectile.velocity.RotatedBy(MathHelper.PiOver2) * (Main.rand.NextFloat() - 0.5f) * projectile.width;
                    Vector2 dustPos = laserEndPos + dustOffset - Vector2.One * SidewaysDustBeamEndOffset;
                    int dustID = 244;
                    int d = Dust.NewDust(dustPos, 8, 8, dustID, 0f, 0f, 100, beamColor, 5f);
                    Main.dust[d].velocity *= 0.5f;
                    // Force the dust to always move downwards, never upwards.
                    Main.dust[d].velocity.Y = -Math.Abs(Main.dust[d].velocity.Y);
                }

                // X = beam length. Y = beam width.
                Vector2 beamDims = new Vector2(projectile.velocity.Length() * projectile.localAI[1], projectile.width * projectile.scale);

                // If the game is rendering (i.e. isn't a dedicated server), make the beam disturb water.
                if (Main.netMode != NetmodeID.Server)
                {
                    WaterShaderData wsd = (WaterShaderData)Filters.Scene["WaterDistortion"].GetShader();
                    // A universal time-based sinusoid which updates extremely rapidly. GlobalTime is 0 to 3600, measured in seconds.
                    float waveSine = 0.1f * (float)Math.Sin(Main.GlobalTime * 20f);
                    Vector2 ripplePos = projectile.position + new Vector2(beamDims.X * 0.5f, 0f).RotatedBy(velAngle);
                    // Yes, wave data is encoded as a color. I guess someone forgot about Vector3?
                    Color waveData = new Color(0.5f, 0.1f * Math.Sign(waveSine) + 0.5f, 0f, 1f) * Math.Abs(waveSine);
                    wsd.QueueRipple(ripplePos, waveData, beamDims, RippleShape.Square, velAngle);
                }

                // Make the beam cast light along its length.
                // v3_1 is an unnamed decompiled variable which is the color of the light cast by DelegateMethods.CastLight
                DelegateMethods.v3_1 = beamColor.ToVector3() * BeamLightBrightness;
                Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], beamDims.Y, new Utils.PerLinePoint(DelegateMethods.CastLight));
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            // If the beam isn't "moving" or "pointed" anywhere, don't do anything.
            if (projectile.velocity == Vector2.Zero)
                return false;

            Texture2D tex = Main.projectileTexture[projectile.type];
            float beamLength = projectile.localAI[1];
            Vector2 projCenterTile = projectile.Center.Floor() + projectile.velocity * projectile.scale * BeamRenderTileOffset;
            Vector2 scaleVec = new Vector2(projectile.scale);

            // Reduce the beam length proportional to its square area to reduce block penetration.
            beamLength -= BeamLengthReductionFactor * projectile.scale * projectile.scale;

            // Draw the outer beam
            // f_1 is an unnamed decompiled variable which is probably the opacity of the beam
            DelegateMethods.f_1 = 1f;
            Color outerBeamColor = GetBeamColor();
            // c_1 is an unnamed decompiled variable which is the render color of the beam drawn by DelegateMethods.RainbowLaserDraw
            DelegateMethods.c_1 = outerBeamColor * OuterBeamOpacityMultiplier * projectile.Opacity;
            Utils.LaserLineFraming llf = new Utils.LaserLineFraming(DelegateMethods.RainbowLaserDraw);
            Utils.DrawLaser(Main.spriteBatch, tex, projCenterTile - Main.screenPosition, projCenterTile + projectile.velocity * beamLength - Main.screenPosition, scaleVec, llf);

            // Draw the inner beam (half size)
            Color innerBeamColor = Color.White;
            DelegateMethods.c_1 = innerBeamColor * InnerBeamOpacityMultiplier * projectile.Opacity;
            llf = new Utils.LaserLineFraming(DelegateMethods.RainbowLaserDraw);
            Utils.DrawLaser(Main.spriteBatch, tex, projCenterTile - Main.screenPosition, projCenterTile + projectile.velocity * beamLength - Main.screenPosition, scaleVec / 2f, llf);
            return false;
        }

        // Automatically iterates through every tile the laser is overlapping to cut grass at all those locations.
        public override void CutTiles()
        {
            // tilecut_0 is an unnamed decompiled variable which tells CutTiles how the tiles are being cut (in this case, via a projectile).
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Utils.PerLinePoint cut = new Utils.PerLinePoint(DelegateMethods.CutTiles);
            Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], projectile.width * projectile.scale, cut);
        }

        // Determines whether the specified target hitbox is intersecting with the beam.
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // If the target is touching the beam's hitbox (which is a small rectangle vaguely overlapping the host crystal), that's good enough.
            if (projHitbox.Intersects(targetHitbox))
                return true;
            // TODO -- can replace this with a discard
            float intersectDist = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], BeamHitboxCollisionWidth * projectile.scale, ref intersectDist);
        }
    }
}
