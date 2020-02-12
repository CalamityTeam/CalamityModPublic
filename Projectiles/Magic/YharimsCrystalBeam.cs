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
        private const float BeamPosOffset = 16f;
        private const float MaxBeamScale = 1.8f;
        private const float FullDamageMultiplier = 3f;
        private const int NumSamplePoints = 2;
        private const float BeamTileCollisionWidth = 0f;
        private const float MaxBeamLength = 2400f;
        private const float LaserLerpRatio = 0.75f;
        private const float MainDustBeamEndOffset = 14.5f;
        private const float SidewaysDustBeamEndOffset = 4f;
        private const float BeamRenderTileOffset = 10.5f;
        private const float RenderColorMultiplier = 0.75f;
        private const float BeamHitboxCollisionWidth = 22f;

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
            // TODO -- why is the saturation the custom variable instead of the hue
            // this is probably one of the reasons why yharim's crystal is considered to be so ugly
            float customHue = GetHue(projectile.ai[0]);
            Color c = Main.hslToRgb(2.55f, customHue, 0.53f);
            c.A = 0;
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
            return (int)indexing / 6f;
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

            // -2.5, -1.5, 0.5, 1.5, or 2.5 based on the beam ID. Used to make the beams orient differently.
            float beamIdOffset = (int)projectile.ai[0] - 2.5f;

            Vector2 hostCrystalDir = Vector2.Normalize(hostCrystal.velocity);
            float projScale;
            float spinRate;
            float beamStartSidewaysOffset;
            float beamStartForwardsOffset;

            // At any point before 180 frames, these variables scale smoothly.
            if (hostCrystal.ai[0] < 180f)
            {
                // From 0 to 180 frames, this variable scales up from 0 to MaxBeamScale.
                projScale = MaxBeamScale * hostCrystal.ai[0] / 180f;

                // From 0 to 180 frames, this variable scales down from 20 to 6.
                beamStartSidewaysOffset = 20f - hostCrystal.ai[0] / 180f * 14f;

                // From 0 to 180 frames, this variable scales up from -22 to -2.
                beamStartForwardsOffset = -22f + hostCrystal.ai[0] / 180f * 20f;

                // From 0 to 120 frames, the opacity scales up from 0% to 40%.
                // Unknown variable scales down from 20 to 16.
                if (hostCrystal.ai[0] < 120f)
                {
                    projectile.Opacity = hostCrystal.ai[0] / 120f * 0.4f;
                    spinRate = 20f - 4f * (hostCrystal.ai[0] / 120f);
                }

                // Between 120 and 180 frames, the opacity scales up from 40% to 100%.
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
                spinRate = 6f;
                beamStartSidewaysOffset = 6f;
                beamStartForwardsOffset = -2f;
            }

            // The amount to which the angle changes reduces over time so that the beams look like they are focusing.
            // ((0 to infinity) plus (-2.5 to 2.5) * (20 to 6 but sticks at 16)) / ((120 to 36 but sticks at 96) * TwoPi)
            float beamAngle = (hostCrystal.ai[0] + beamIdOffset * spinRate) / (spinRate * 6f) * MathHelper.TwoPi;

            Vector2 unitRot = Vector2.UnitY.RotatedBy(beamAngle);
            float sinusoidYOffset = unitRot.Y * PiOver6 * (MaxBeamScale - projScale);

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
                Color customBeamColor = GetBeamColor();

                // Convert Terraria's current disco color into a single hue variable.
                float discoHue = Main.rgbToHsl(new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB)).X;

                // Create a few dust per frame a small distance from where the beam ends.
                Vector2 laserEndPos = projectile.Center + projectile.velocity * (projectile.localAI[1] - MainDustBeamEndOffset * projectile.scale);
                for (int i = 0; i < 2; ++i)
                {
                    // 50% chance for the dust to come off on either side of the beam.
                    float dustAngle = velAngle + (Main.rand.NextBool() ? 1f : -1f) * MathHelper.PiOver2;
                    float dustStartDist = Main.rand.NextFloat(1f, 1.8f);

                    // TODO -- confirm that this replacement is OK
                    // Vector2 dustVel = dustAngle.ToRotationVector2() * dustStartDist;
                    Vector2 dustVel = new Vector2((float)Math.Cos(dustAngle) * dustStartDist, (float)Math.Sin(dustAngle) * dustStartDist);
                    int d = Dust.NewDust(laserEndPos, 0, 0, 244, dustVel.X, dustVel.Y, 0, new Color(255, Main.DiscoG, 53), 3.3f);
                    Main.dust[d].color = customBeamColor;
                    Main.dust[d].noGravity = true;
                    Main.dust[d].scale = 1.2f;

                    // Scale up dust with the projectile if it's large.
                    if (projectile.scale > 1f)
                    {
                        Main.dust[d].velocity *= projectile.scale;
                        Main.dust[d].scale *= projectile.scale;
                    }

                    // TODO -- this looks like an unintentional change where yharim's crystal scales to 1.8 instead of 1.4
                    // If the projectile isn't exactly 1.4 scale (probably changed from vanilla last prism which only scales to 1.4...)
                    // then make an additional dust that's orange and smaller
                    if (projectile.scale != 1.4f)
                    {
                        Dust dust9 = Dust.CloneDust(d);
                        dust9.color = Color.Orange;
                        dust9.scale /= 2f;
                    }

                    // Color was previously just set to customBeamColor but now it's being lerped between its original color and a random disco offset
                    float hue = (discoHue + Main.rand.NextFloat() * 0.4f) % 1f;
                    // TODO -- again, scaling based on 1.4 instead of 1.8.
                    Main.dust[d].color = Color.Lerp(customBeamColor, Main.hslToRgb(2.55f, hue, 0.53f), projectile.scale / 1.4f);
                }

                // Low chance every frame to spawn a large "directly sideways" dust which doesn't move.
                if (Main.rand.NextBool(5))
                {
                    // Velocity, flipped sideways, times -50% to 50% of beam width.
                    Vector2 dustOffset = projectile.velocity.RotatedBy(MathHelper.PiOver2) * (Main.rand.NextFloat() - 0.5f) * projectile.width;
                    Vector2 dustPos = laserEndPos + dustOffset - Vector2.One * SidewaysDustBeamEndOffset;
                    int dustID = 244;
                    // This dust's color is entirely unaffected by the beam.
                    Color dustColor = new Color(255, Main.DiscoG, 53);
                    int d = Dust.NewDust(dustPos, 8, 8, dustID, 0f, 0f, 100, dustColor, 5f);
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
                DelegateMethods.v3_1 = customBeamColor.ToVector3() * 0.3f;
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
            Color customBeamColor = GetBeamColor();
            Vector2 projCenterTile = projectile.Center.Floor() + projectile.velocity * projectile.scale * BeamRenderTileOffset;
            Vector2 scaleVec = new Vector2(projectile.scale);

            // TODO -- why does this square projectile.scale?
            beamLength -= 14.5f * projectile.scale * projectile.scale;

            // f_1 is an unnamed decompiled variable which is probably the opacity of the beam
            DelegateMethods.f_1 = 1f;
            // c_1 is an unnamed decompiled variable which is the render color of the beam drawn by DelegateMethods.RainbowLaserDraw
            DelegateMethods.c_1 = customBeamColor * RenderColorMultiplier * projectile.Opacity;
            // Vector2 sheetInsertVec = projectile.Size / 2f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;

            // Draw the main beam with a rainbow framing/outline.
            Utils.LaserLineFraming llf = new Utils.LaserLineFraming(DelegateMethods.RainbowLaserDraw);
            Utils.DrawLaser(Main.spriteBatch, tex, projCenterTile - Main.screenPosition, projCenterTile + projectile.velocity * beamLength - Main.screenPosition, scaleVec, llf);

            // Draw another beam at half the size with a different color, again with a rainbow framing/outline.
            DelegateMethods.c_1 = new Color(255, Main.DiscoG, 53, 127) * 0.75f * projectile.Opacity;
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
