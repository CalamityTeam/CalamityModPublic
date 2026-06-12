using System;
using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class XykWings : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];
        public CalamityPlayer moddedOwner => Owner.Calamity();
        public Color drawColor => Owner.Calamity().XykFXColor;
        public int time = 0;
        public int flightTimer = 0;
        public float wingFlapHeight = 0;
        public Vector2 expectedWingPosition1;
        public Vector2 expectedWingPosition2;
        public Vector2 WingVel1;
        public Vector2 WingVel2;
        public float backWingRot = 0;
        public int lastDir = 0;
        public float spawnFade = 0;
        public float spawnFadePow => (Projectile.ai[1] == 5 ? (float)Math.Pow(spawnFade, 1) : (float)Math.Pow(spawnFade, 4));
        public int deathFadeTimer = 0;

        public bool iAmTopWing = false;
        public Vector2 playerCenterPoint;
        public int groundTime = 0;
        public int dashTimer = 0;
        public bool doDashEffect = true;
        public float dashfx = 0;
        public int timeFalling = 0;
        public bool BreakApart = false;

        public ref float wingNum => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.scale = 0;
        }

        public override void AI()
        {
            float intendedScale = 0.85f;
            float spawnAnimTime = 12 + wingNum;
            if (time == 0)
            {
                expectedWingPosition1 = Owner.Center;
                expectedWingPosition2 = Owner.Center;
                playerCenterPoint = Owner.MountedCenter;
            }
            if (time <= spawnAnimTime)
            {
                spawnFade = Utils.GetLerpValue(0, spawnAnimTime, time, true);
                Projectile.scale = intendedScale * Math.Min(spawnFade + 0.5f, 1);
                if (time == spawnAnimTime)
                {
                    Projectile.scale = 1.6f;
                }
            }
            else if (Projectile.ai[1] == 0)
                Projectile.scale = MathHelper.Lerp(Projectile.scale, intendedScale, 0.08f);

            if (moddedOwner.XykVisualsBlue || moddedOwner.XykVisualsOrange)
                Projectile.timeLeft++;
            else
                Projectile.Kill();
            if (Owner.dead)
                Projectile.Kill();

            playerCenterPoint = Owner.MountedCenter;

            if (Owner.wingTime == Owner.wingTimeMax && Owner.velocity.Y == 0 && Owner.dashDelay != -1)
                groundTime++;
            else
                groundTime = 0;
            if (((groundTime >= 180 || Owner.wingTime <= 0) && wingNum == checkActiveWings() - 1) || Projectile.ai[1] == 5)
            {
                if (Projectile.ai[1] == 0)
                {
                    iAmTopWing = true;
                    foreach (Projectile p in Main.ActiveProjectiles)
                        if (p.type == ModContent.ProjectileType<XykWings>() && p.owner == Owner.whoAmI && p.ai[1] == 0)
                            p.ai[1] = 5;
                    return;
                }
                if (deathFadeTimer == 0)
                    BreakApart = Owner.wingTime <= 0;
                if (BreakApart)
                {
                    Projectile.extraUpdates = 1;
                    int startTime = (int)(6 * (wingNum + 1));
                    if (deathFadeTimer == startTime)
                    {
                        WingVel1 = Vector2.UnitX * -Main.rand.NextFloat(4f, 7f) * lastDir + Vector2.UnitY.RotatedByRandom(0.3f) * -Main.rand.NextFloat(5, 7);
                        WingVel2 = Vector2.UnitX * Main.rand.NextFloat(4f, 7f) * lastDir + Vector2.UnitY.RotatedByRandom(0.3f) * -Main.rand.NextFloat(5, 7);
                    }
                    if (deathFadeTimer > startTime)
                    {
                        WingVel1.X *= 0.97f;
                        if (WingVel1.Y < 15)
                            WingVel1.Y += 0.2f;
                        if (WingVel1.Y < 5)
                            WingVel1.Y *= 0.97f;
                        WingVel2.X *= 0.97f;
                        if (WingVel2.Y < 15)
                            WingVel2.Y += 0.2f;
                        if (WingVel2.Y < 5)
                            WingVel2.Y *= 0.97f;

                        expectedWingPosition1 += WingVel1;
                        expectedWingPosition2 += WingVel2;

                        Projectile.rotation += 0.01f + wingNum * 0.005f;
                        backWingRot -= 0.01f + wingNum * 0.005f;
                    }
                    else
                    {
                        expectedWingPosition1 += Owner.velocity * 0.65f;
                        expectedWingPosition2 += Owner.velocity * 0.65f;
                    }
                }
                else
                {
                    int startTime = (int)(2 * wingNum);
                    if (deathFadeTimer == startTime)
                    {
                        WingVel1 = (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * 15;
                        WingVel2 = (backWingRot + MathHelper.PiOver2).ToRotationVector2() * 7;
                    }
                    if (deathFadeTimer > startTime)
                    {
                        expectedWingPosition1 += WingVel1;
                        expectedWingPosition2 += WingVel2;
                        WingVel1 *= 0.955f;
                        WingVel2 *= 0.955f;
                    }
                    else
                    {
                        expectedWingPosition1 += Owner.velocity * 0.85f;
                        expectedWingPosition2 += Owner.velocity * 0.85f;
                    }
                }

                dashfx = MathHelper.Lerp(dashfx, 0, 0.07f);
                deathFadeTimer++;

                int fadeTime = (BreakApart ? 90 : 20);
                spawnFade = Utils.GetLerpValue(fadeTime, 0, deathFadeTimer, true);
                Projectile.scale = intendedScale * spawnFadePow;
                if (deathFadeTimer >= fadeTime)
                {
                    Projectile.Kill();
                    return;
                }
                return;
            }
            else
            {
                bool isFlying = Owner.controlJump;
                bool isFalling = Owner.controlDown && !isFlying && Owner.velocity.Y > 0;
                bool isUpBoosting = Owner.wingTime > 0 && Owner.controlJump && Owner.controlUp;
                bool isHovering = Owner.wingTime > 0 && Owner.controlDown && Owner.controlJump && !isUpBoosting;
                if (isFalling || timeFalling < 0)
                    timeFalling++;
                else if (timeFalling > 0)
                    timeFalling -= 3;
                float fallingLerp = (float)Math.Pow(Utils.GetLerpValue(0, 40, timeFalling, true), 3);
                float topWingNum = (checkActiveWings() - 1);

                float sine = (float)Math.Sin((time * 0.35f * (isFalling ? 3 : (isHovering || isUpBoosting) ? 3.2f : isFlying ? 2f : 1) + wingNum * ((isHovering || isUpBoosting) ? 3f : 2f)) / MathHelper.Pi);

                if (Owner.dashDelay == -1 || isUpBoosting || isHovering || Owner.Calamity().adrenalineModeActive || Owner.Calamity().rageModeActive || isFalling && Owner.velocity.Y > 16)
                {
                    dashfx = MathHelper.Lerp(dashfx, 1, 0.25f);
                    float bonusScale = 1;
                    if (Owner.Calamity().rageModeActive)
                        bonusScale += 0.25f;
                    if (Owner.Calamity().adrenalineModeActive)
                        bonusScale += 0.5f;
                    if (Projectile.scale < bonusScale)
                        Projectile.scale = bonusScale;
                }
                else
                {
                    dashfx = MathHelper.Lerp(dashfx, 0, 0.03f);
                    if (dashfx > 0)
                        dashfx -= 0.01f;
                    else
                        dashfx = 0;
                }

                float flapSpeed = MathHelper.Lerp(0.06f * (isHovering ? 2.5f : isFlying ? 2f : 1), 0.55f * (isHovering ? 2.5f : isFlying ? 2f : 1), (float)Math.Pow(Utils.GetLerpValue(-1, 1, sine), 3));

                wingFlapHeight = MathHelper.Lerp(wingFlapHeight, sine, flapSpeed) * spawnFadePow;

                for (int i = -1; i <= 1; i += 2)
                {
                    float dir = i * lastDir;
                    Vector2 flightMovement = Vector2.UnitY.RotatedBy(isFalling ? MathHelper.ToRadians(13) * dir * fallingLerp : 0) * (-15 * Projectile.scale + 10 * (isFalling ? 0.35f : isUpBoosting ? 5f : isHovering ? 1.5f : isFlying ? 2.5f : 1) * wingFlapHeight) * ((Owner.velocity.Y == 0) ? 0 : 1);
                    float wingPartDist = (1.6f / (checkActiveWings() + 1)) * wingNum * (isFalling ? 1 - 0.3f * fallingLerp : isHovering ? 0.5f : 1);
                    Vector2 destination = (playerCenterPoint - (Vector2.UnitX.RotatedBy(isFalling ? MathHelper.ToRadians(13) * dir * fallingLerp : 0) * (22 + 7 * wingFlapHeight * (i == -1 ? 0.5f : 1)) * Projectile.scale).RotatedBy((-0.8f + (isFalling ? 0.7f * fallingLerp : 0) + wingPartDist + (wingNum == topWingNum ? 0.1f : wingNum == 0 ? -0.1f : 0)) * dir) * dir + (Vector2.One * (isFlying ? 0 : 1)).RotatedBy((time * 0.12f + wingNum * 0.9f) * dir)) + flightMovement;

                    float lerpSpeed = (1 + wingNum * (0.1f + 0.15f * fallingLerp) / (Owner.moveSpeed * 0.3f + 1));
                    float spawnDistance = 150;
                    if (i == 1)
                        expectedWingPosition1 += ((destination + destination.DirectionFrom(playerCenterPoint) * Math.Max(1, spawnDistance * (1 - spawnFadePow))) - expectedWingPosition1) / lerpSpeed;
                    else
                        expectedWingPosition2 += ((destination + (Vector2.UnitX * -6f * lastDir) + destination.DirectionFrom(playerCenterPoint) * Math.Max(1, spawnDistance * (1 - spawnFadePow))) - expectedWingPosition2) / lerpSpeed;
                }
                Projectile.Center = Owner.Center;

                Projectile.rotation = expectedWingPosition1.DirectionFrom(playerCenterPoint).ToRotation() - MathHelper.PiOver2;
                backWingRot = expectedWingPosition2.DirectionFrom(playerCenterPoint).ToRotation() - MathHelper.PiOver2;

                if (wingNum == 0)
                    Lighting.AddLight(Owner.Center, Color.Lerp(drawColor, Color.White, 0.5f).ToVector3() * 1.1f * spawnFadePow);

                if (dashfx > 0.3f && (wingNum == checkActiveWings() - 1 || wingNum == 0))
                {
                    for (int i = -2; i <= 2; i += 1)
                    {
                        if (i == 0)
                            i++;

                        bool front = i > 0;
                        Vector2 pos1 = expectedWingPosition1 + (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * (45 + 5 * wingNum) * (Projectile.scale + 0.15f);
                        Vector2 pos2 = expectedWingPosition2 + (backWingRot + MathHelper.PiOver2).ToRotationVector2() * (15 + 3 * wingNum) * (Projectile.scale + 0.15f);

                        if (Owner.Calamity().XykVisualsBlue)
                        {
                            bool circle = Main.rand.NextBool(3);
                            Dust dust = Dust.NewDustPerfect(front ? pos1 : pos2, circle ? ModContent.DustType<SquashDustHollow>() : ModContent.DustType<SquashDust>(), Owner.velocity.RotatedByRandom(circle ? 0.5f : 0) * Main.rand.NextFloat(-0.5f, -0.2f) * dashfx);
                            dust.scale = Main.rand.NextFloat(1.3f, 1.4f) * (front ? 1 : 0.6f) * dashfx * (circle ? 0.6f : 1f) * (float)Math.Pow(Projectile.scale, 0.4f);
                            dust.noGravity = true;
                            dust.color = drawColor;
                            dust.noLightEmittence = true;
                            dust.fadeIn = 1.5f * (circle ? 0 : 1f);
                        }
                        else
                        {
                            bool square = Main.rand.NextBool(3);
                            Dust dust = Dust.NewDustPerfect(front ? pos1 : pos2, square ? ModContent.DustType<SquareDust>() : ModContent.DustType<SquashDust>(), Owner.velocity.RotatedByRandom(0.25f) * Main.rand.NextFloat(-0.5f, -0.2f) * dashfx);
                            dust.scale = Main.rand.NextFloat(0.8f, 1f) * (front ? 1 : 0.6f) * dashfx * (square ? 1 : 1.5f) * (float)Math.Pow(Projectile.scale, 0.4f);
                            dust.noGravity = true;
                            dust.color = drawColor;
                            dust.noLightEmittence = true;
                            dust.fadeIn = 0.1f * (square ? 1 : 3f);
                        }
                    }
                }

                time++;
            }
        }
        public override void PostDraw(Color lightColor)
        {
            if (!BreakApart)
                lastDir = Owner.direction;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float topWingNum = (checkActiveWings() - 1);
            float width = 1;
            float height = 1;
            Texture2D tex;
            if (wingNum == 0)
                tex = moddedOwner.XykVisualsOrange ? ModContent.Request<Texture2D>("CalamityMod/Particles/XykWingOrange2").Value : ModContent.Request<Texture2D>("CalamityMod/Particles/XykWingBlue2").Value;
            else if (wingNum == topWingNum || iAmTopWing)
            {
                height = 1.5f;
                tex = moddedOwner.XykVisualsOrange ? ModContent.Request<Texture2D>("CalamityMod/Particles/XykWingOrange1").Value : ModContent.Request<Texture2D>("CalamityMod/Particles/XykWingBlue1").Value;
            }
            else
            {
                width = 2;
                height = 1f + 0.1f * wingNum;
                tex = moddedOwner.XykVisualsOrange ? ModContent.Request<Texture2D>("CalamityMod/Particles/XykWingOrange3").Value : ModContent.Request<Texture2D>("CalamityMod/Particles/XykWingBlue3").Value;
            }

            Texture2D glow = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;

            Vector2 scale = new Vector2(width, height * spawnFadePow) * 0.12f * Projectile.scale;
            Vector2 scaleBack = new Vector2(width * 1.5f, height * 0.7f * spawnFadePow) * 0.07f * Projectile.scale;
            Vector2 baseDrawPos = -Main.screenPosition + new Vector2(0, Owner.gfxOffY);
            if (dashfx > 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    Main.EntitySpriteDraw(glow, expectedWingPosition1 + baseDrawPos, null, (i < 2 ? Color.White : drawColor) with { A = 0 } * 0.65f, Projectile.rotation + (i % 2 == 0 ? MathHelper.PiOver2 : 0) + MathHelper.PiOver4, glow.Size() * 0.5f, new Vector2(0.2f, 1.2f) * (i < 2 ? 0.7f : 1) * Projectile.scale * (Main.rand.NextFloat(0.25f, 0.35f) + 0.02f * wingNum) * (float)Math.Pow(dashfx, 7), SpriteEffects.None, 0);
                    Main.EntitySpriteDraw(glow, expectedWingPosition2 + baseDrawPos, null, (i < 2 ? Color.White : drawColor) with { A = 0 } * 0.65f, backWingRot + (i % 2 == 0 ? MathHelper.PiOver2 : 0) + MathHelper.PiOver4, glow.Size() * 0.5f, new Vector2(0.2f, 1.2f) * (i < 2 ? 0.3f : 0.6f) * Projectile.scale * (Main.rand.NextFloat(0.25f, 0.35f) + 0.02f * wingNum) * (float)Math.Pow(dashfx, 7), SpriteEffects.None, 0);
                }
                int draws = 12;
                for (int i = 0; i < draws; i++)
                {
                    Vector2 addedPos = ((MathHelper.TwoPi * i / draws).ToRotationVector2() * 2.5f + Main.rand.NextVector2Circular(2, 2)) * dashfx;
                    Main.EntitySpriteDraw(tex, expectedWingPosition1 + baseDrawPos + addedPos, null, drawColor with { A = 0 } * spawnFadePow * 0.3f * dashfx, Projectile.rotation, new Vector2(tex.Width * 0.5f, 0), scale, lastDir == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

                    Main.EntitySpriteDraw(tex, expectedWingPosition2 + baseDrawPos + addedPos, null, drawColor with { A = 0 } * spawnFadePow * 0.3f * dashfx, backWingRot, new Vector2(tex.Width * 0.5f, 0), scaleBack, lastDir == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
                }
            }

            Main.EntitySpriteDraw(tex, expectedWingPosition1 + baseDrawPos, null, Color.Lerp(drawColor, Color.White, dashfx) with { A = 0 } * spawnFadePow * MathHelper.Lerp(1, 0.6f, dashfx), Projectile.rotation, new Vector2(tex.Width * 0.5f, 0), scale, lastDir == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

            Main.EntitySpriteDraw(tex, expectedWingPosition2 + baseDrawPos, null, Color.Lerp(drawColor, Color.White, dashfx) with { A = 0 } * spawnFadePow * MathHelper.Lerp(1, 0.6f, dashfx), backWingRot, new Vector2(tex.Width * 0.5f, 0), scaleBack, lastDir == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            return false;
        }
        public int checkActiveWings()
        {
            int numOfActiveWings = 0;
            foreach (Projectile p in Main.ActiveProjectiles)
                if (p.type == ModContent.ProjectileType<XykWings>() && p.owner == Owner.whoAmI && p.ai[1] == 0)
                    numOfActiveWings++;
            if (Projectile.ai[1] == 5)
                numOfActiveWings = 7;
            return numOfActiveWings;
        }
        public override bool? CanDamage() => false;
    }
}
