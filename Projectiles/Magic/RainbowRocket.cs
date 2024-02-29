using CalamityMod.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Graphics.Primitives;

namespace CalamityMod.Projectiles.Magic
{
    public class RainbowRocket : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public enum PartyCannonExplosionType
        {
            Pink = 0,
            Orange = 1,
            Yellow = 2,
            White = 3,
            SkyBlue = 4,
            Purple = 5,
            PalePink = 6,
            Count = 7
        }

        public ref float Time => ref Projectile.ai[0];
        public PartyCannonExplosionType RocketType
        {
            get => (PartyCannonExplosionType)(int)Projectile.ai[1];
            set => Projectile.ai[1] = (int)value;
        }
        public const float SwerveAngle = 0.02f;
        public const float SwerveAngleOffsetMax = 0.04f;
        public const float SwerveTime = 60f;
        public const float HomingAcceleration = 0.4f;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 52;

            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.DamageType = DamageClass.Magic;
        }

        #region AI
        public override void AI()
        {
            Time++;
            Lighting.AddLight(Projectile.Center, Main.hslToRgb((float)Math.Sin(Time / 20f) * 0.5f + 0.5f, 0.9f, 0.9f).ToVector3());

            Projectile.tileCollide = Time > 60f;

            Projectile.frameCounter++;
            if (Projectile.frameCounter % 4 == 3)
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];

            NPC potentialTarget = Projectile.Center.ClosestNPCAt(2300f, true, true);
            if (Time < SwerveTime)
                DoMovement_IdleSwerveFly();
            else if (potentialTarget != null)
                DoMovement_FlyToTarget(potentialTarget);

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        private void DoMovement_IdleSwerveFly()
        {
            float swerveAngleOffset = MathHelper.Lerp(-SwerveAngleOffsetMax, SwerveAngleOffsetMax, (float)RocketType / ((float)PartyCannonExplosionType.Count));
            Projectile.velocity = Projectile.velocity.RotatedBy(swerveAngleOffset + SwerveAngle);
        }

        private void DoMovement_FlyToTarget(NPC target)
        {
            float angleToTarget = Projectile.AngleTo(target.Center);
            float angleOffset = MathHelper.WrapAngle(angleToTarget - Projectile.velocity.ToRotation());
            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.Clamp(angleOffset, -0.2f, 0.2f));
            Projectile.velocity = Projectile.velocity.SafeNormalize(-Vector2.UnitY) * (Projectile.velocity.Length() + HomingAcceleration);

            if (Vector2.Dot(Projectile.velocity.SafeNormalize(Vector2.Zero), Projectile.SafeDirectionTo(target.Center)) < 0.75f)
                Projectile.velocity *= 0.75f;
        }
        #endregion

        #region Draw Effects

        internal Color GetRocketColor()
        {
            switch (RocketType)
            {
                case PartyCannonExplosionType.Pink:
                    return Color.Pink;
                case PartyCannonExplosionType.Orange:
                    return Color.Orange;
                case PartyCannonExplosionType.Yellow:
                    return Color.LightGoldenrodYellow * 0.8f;
                case PartyCannonExplosionType.White:
                    return Color.LightGray;
                case PartyCannonExplosionType.SkyBlue:
                    return Color.LightSkyBlue;
                case PartyCannonExplosionType.Purple:
                    return Color.Magenta;
                case PartyCannonExplosionType.PalePink:
                    return Color.Pink * 0.77f;
            }
            return Color.White;
        }

        internal Color ColorFunction(float completionRatio)
        {
            Color baseColor = Main.hslToRgb((Projectile.identity * 0.33f + completionRatio + Main.GlobalTimeWrappedHourly * 2f) % 1f, 1f, 0.54f);
            return Color.Lerp(GetRocketColor(), baseColor, MathHelper.Clamp(completionRatio * 0.8f, 0f, 1f)) * Projectile.Opacity;
        }

        internal float WidthFunction(float completionRatio)
        {
            float width;
            float maxWidthOutwardness = 8f;
            if (completionRatio < 0.1f)
                width = (float)Math.Sin(completionRatio / 0.1f * MathHelper.PiOver2) * maxWidthOutwardness + 0.1f;
            else
                width = MathHelper.Lerp(maxWidthOutwardness, 0f, Utils.GetLerpValue(0.1f, 1f, completionRatio, true));
            return width * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.oldPos[0] = Projectile.position + Projectile.velocity.SafeNormalize(Vector2.Zero) * 50f;
            PrimitiveSet.Prepare(Projectile.oldPos, new(WidthFunction, ColorFunction, (_) => Projectile.Size * 0.5f + Projectile.velocity), 80);

            Texture2D rocketTexture = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(rocketTexture,
                             Projectile.Center - Main.screenPosition,
                             rocketTexture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame),
                             GetRocketColor(),
                             Projectile.rotation,
                             rocketTexture.Size() * 0.5f,
                             Projectile.scale,
                             SpriteEffects.None,
                             0);
            return false;
        }
        #endregion

        #region Kill Effects
        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 1; i < Projectile.oldPos.Length; i++)
                {
                    if (Main.rand.NextBool(3))
                    {
                        float offsetAngle = MathHelper.Lerp(-MathHelper.PiOver4, MathHelper.PiOver4, i / (float)Projectile.oldPos.Length);
                        Vector2 spawnPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f;
                        Vector2 spawnVelocity = (Projectile.oldPos[i - 1] - Projectile.oldPos[i]).SafeNormalize(Vector2.Zero);
                        spawnVelocity = spawnVelocity.RotatedBy(offsetAngle);
                        spawnVelocity *= Main.rand.NextFloat(12f, 18f);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPosition, spawnVelocity, ModContent.ProjectileType<PartySparkle>(), Projectile.damage, 2f, Projectile.owner);
                    }
                }
            }

            Projectile.ExpandHitboxBy(350);
            Projectile.Damage();

            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            // There's no need to spawn dust from the server.
            if (Main.dedServ)
                return;

            switch (RocketType)
            {
                case PartyCannonExplosionType.Pink:
                    PinkMarkExplosionDust();
                    break;
                case PartyCannonExplosionType.Orange:
                    OrangeMarkExplosionDust();
                    break;
                case PartyCannonExplosionType.Yellow:
                    YellowMarkExplosionDust();
                    break;
                case PartyCannonExplosionType.White:
                    WhiteMarkExplosionDust();
                    break;
                case PartyCannonExplosionType.SkyBlue:
                    SkyBlueMarkExplosionDust();
                    break;
                case PartyCannonExplosionType.Purple:
                    PurpleMarkExplosionDust();
                    break;
                case PartyCannonExplosionType.PalePink:
                    PalePinkMarkExplosionDust();
                    break;
            }
        }
        #endregion

        #region Pink Mark
        public void PinkMarkExplosionDust()
        {
            // Spawn the main balloon.
            BalloonExplosionDust(Projectile.Center, Main.rand.NextFloat(8f, 15f), Color.Yellow);

            // Spawn the two smaller balloons.
            float absoluteOffsetAngle = Main.rand.NextFloat(0.4f, 0.7f) * -1f;
            Vector2 offset = Vector2.UnitY.RotatedBy(absoluteOffsetAngle) * 120f;
            offset += Vector2.UnitX * 40f;
            BalloonExplosionDust(Projectile.Center + offset, Main.rand.NextFloat(6f, 11f), Color.DeepPink, absoluteOffsetAngle);

            absoluteOffsetAngle = Main.rand.NextFloat(0.4f, 0.7f);
            offset = Vector2.UnitY.RotatedBy(absoluteOffsetAngle) * 120f;
            offset -= Vector2.UnitX * 40f;
            BalloonExplosionDust(Projectile.Center + offset, Main.rand.NextFloat(6f, 11f), Color.DeepPink, absoluteOffsetAngle);
        }
        public void BalloonExplosionDust(Vector2 center, float petalBurstSpeed, Color balloonColor, float absoluteOffsetAngle = 0)
        {
            float explosionOffsetAngle = Main.rand.NextFloat(-0.3f, 0.3f) + absoluteOffsetAngle;

            // Draw the balloon as a petal.
            for (float angle = 0f; angle <= MathHelper.TwoPi; angle += MathHelper.ToRadians(4f))
            {
                Vector2 initialUnitVector = angle.ToRotationVector2();
                float unitMultiplier = 2f + (1f + (float)Math.Cos(MathHelper.Pi + angle)); // This value is used to "squash" the circle to make it into a petal shape.
                Vector2 velocity = initialUnitVector * unitMultiplier;
                velocity.X *= 0.5f;
                velocity = velocity.SafeNormalize(Vector2.Zero); // Turn the petal velocity into a unit vector again to make future speed calculations easier.
                velocity = velocity.RotatedBy(MathHelper.PiOver2); // Flip the petal velocity. It's tilted to the side normally.
                velocity = velocity.RotatedBy(explosionOffsetAngle); // Account for the explosion offset angle that was determined at the start.

                Dust dust = Dust.NewDustPerfect(center, 261);
                dust.velocity = velocity * petalBurstSpeed;
                dust.noGravity = true;
                dust.color = balloonColor;
                dust.fadeIn = 1.5f;
            }

            // Draw the string below the petal, giving it the appearance of a balloon.
            StringExplosionDust(center, petalBurstSpeed, explosionOffsetAngle);
        }
        public void StringExplosionDust(Vector2 center, float petalBurstSpeed, float offsetAngle)
        {
            int evaluationPoints = 40;

            // When mapped out, these points give a rough string, which when used with a bezier curve, can be smoothened out to make it look like a real string.
            Vector2[] bezierControlPoints = new Vector2[]
            {
                new Vector2(0f, 40f),
                new Vector2(20f, 76f),
                new Vector2(-16f, 108f),
                new Vector2(-20f, 146f),
                new Vector2(-14f, 180f),
                new Vector2(10f, 214f)
            };
            BezierCurve bezierCurve = new BezierCurve(bezierControlPoints);
            for (int i = 0; i < evaluationPoints; i++)
            {
                Dust dust = Dust.NewDustPerfect(center, 261);
                dust.position = center + bezierCurve.Evaluate(i / (float)evaluationPoints).RotatedBy(offsetAngle) + Vector2.UnitY * petalBurstSpeed * 2f;
                dust.velocity = Vector2.Zero;
                dust.noGravity = true;
                dust.color = Color.White;
                dust.fadeIn = 1.5f;
            }
        }
        #endregion

        #region Orange Mark
        public void OrangeMarkExplosionDust()
        {
            // Spawn the main apple.
            AppleExplosionDust(Projectile.Center, Main.rand.NextFloat(8f, 15f));

            // Spawn two other apples to the side.
            float absoluteOffsetAngle = Main.rand.NextFloat(0.4f, 0.7f) * -1f;
            Vector2 offset = Vector2.UnitY.RotatedBy(absoluteOffsetAngle) * 120f;
            offset += Vector2.UnitX * 70f;
            AppleExplosionDust(Projectile.Center + offset, Main.rand.NextFloat(6f, 11f), absoluteOffsetAngle);

            absoluteOffsetAngle = Main.rand.NextFloat(0.4f, 0.7f);
            offset = Vector2.UnitY.RotatedBy(absoluteOffsetAngle) * 120f;
            offset -= Vector2.UnitX * 70f;
            AppleExplosionDust(Projectile.Center + offset, Main.rand.NextFloat(6f, 11f), absoluteOffsetAngle);
        }
        public void AppleExplosionDust(Vector2 center, float appleBurstSpeed, float offsetAngle = 0f)
        {
            for (float angle = 0f; angle <= MathHelper.TwoPi; angle += MathHelper.ToRadians(4f))
            {
                Vector2 initialUnitVector = angle.ToRotationVector2();
                float cosineValue = (float)Math.Cos(angle);

                Vector2 velocity = initialUnitVector;

                velocity = velocity.SafeNormalize(Vector2.Zero); // Turn the petal velocity into a unit vector again to make future speed calculations easier.
                velocity.X *= 1.333f;

                // Squash the circle to the shape of an apple.
                if (Math.Abs(cosineValue) < 0.35f)
                {
                    velocity.Y *= MathHelper.Lerp(0.9f, 1f, Math.Abs(cosineValue) / 0.35f);

                    // Squash the upper part slightly more than the lower part.
                    if (Math.Sign(cosineValue) == 1f)
                    {
                        velocity.Y *= 0.85f;
                    }
                }

                velocity = velocity.RotatedBy(offsetAngle); // Account for the explosion offset angle that was determined at the start.

                Dust dust = Dust.NewDustPerfect(center, 261);
                dust.velocity = velocity * appleBurstSpeed;
                dust.noGravity = true;
                dust.color = Color.Orange;
                dust.fadeIn = 1.5f;
            }
            AppleLeafExplosionDust(center + new Vector2(0f, -54).RotatedBy(offsetAngle), offsetAngle);
        }
        public void AppleLeafExplosionDust(Vector2 center, float offsetAngle)
        {
            int evaluationPoints = 40;

            // When mapped out, these points give a rough apple stem shape.
            Vector2[] bezierControlPoints = new Vector2[]
            {
                new Vector2(0f, 0f),
                new Vector2(-20f, -26f),
                new Vector2(-26f, -46f),
                new Vector2(-30f, -60f),
                new Vector2(-24f, -70f),
                new Vector2(-10f, -40f),
                new Vector2(0f, 0f),
            };
            BezierCurve bezierCurve = new BezierCurve(bezierControlPoints);
            for (int i = 0; i < evaluationPoints; i++)
            {
                Dust dust = Dust.NewDustPerfect(center, 261);
                dust.position = center + bezierCurve.Evaluate(i / (float)evaluationPoints).RotatedBy(offsetAngle);
                dust.velocity = Vector2.Zero;
                dust.noGravity = true;
                dust.color = Color.Brown;
                dust.fadeIn = 1.5f;
            }
        }
        #endregion

        #region Yellow Mark
        public void YellowMarkExplosionDust()
        {
            // Spawn three butterflies.
            ButterflyExplosionDust(Projectile.Center, Main.rand.NextFloat(8f, 12f));

            Vector2 offset = Vector2.UnitY.RotatedBy(Main.rand.NextFloat(0.4f, 0.7f) * -1f) * 120f;
            offset += Vector2.UnitX * 84f;
            ButterflyExplosionDust(Projectile.Center + offset, Main.rand.NextFloat(6f, 11f));

            offset = Vector2.UnitY.RotatedBy(Main.rand.NextFloat(0.4f, 0.7f)) * 120f;
            offset -= Vector2.UnitX * 84f;
            ButterflyExplosionDust(Projectile.Center + offset, Main.rand.NextFloat(6f, 11f));
        }
        public void ButterflyExplosionDust(Vector2 center, float butterflyBurstSpeed, float offsetAngle = 0f)
        {
            int petalCount = 4;
            // Draw the butterfly as a petal with 4 sides.
            for (float angle = 0f; angle <= MathHelper.TwoPi; angle += MathHelper.ToRadians(4f))
            {
                Vector2 initialUnitVector = angle.ToRotationVector2();
                float unitMultiplier = 2f + (1f + (float)Math.Cos(angle * petalCount)); // This value is used to "squash" the circle to make it into a petal shape.
                Vector2 velocity = initialUnitVector * unitMultiplier;
                velocity = velocity.RotatedBy(MathHelper.PiOver4 + MathHelper.PiOver2); // Tilt the vector to make it look like an actual butterfly.
                velocity = velocity.RotatedBy(offsetAngle);
                velocity *= 0.25f; // Normalization removes the spaces that make the vector petal-like and not circular, so multiplication must be utilized instead.

                Dust dust = Dust.NewDustPerfect(center, 262);
                dust.velocity = velocity * butterflyBurstSpeed;
                dust.noGravity = true;
                dust.color = Color.Pink;
                dust.fadeIn = 1.5f;
            }
            ButterflyAntennaExplosionDust(center + new Vector2(0f, -74f).RotatedBy(offsetAngle), offsetAngle);
        }
        public void ButterflyAntennaExplosionDust(Vector2 center, float offsetAngle = 0f)
        {
            int evaluationPoints = 40;

            // When mapped out, these points give a rough apple stem shape.
            Vector2[] bezierControlPoints = new Vector2[]
            {
                new Vector2(0f, 60f),
                new Vector2(0f, 30f),
                new Vector2(10f, -28f),
                new Vector2(20f, -42f),
                new Vector2(30f, -48f),
                new Vector2(42f, -40f),
                new Vector2(50f, -30f),
                new Vector2(52f, -24f),
            };
            BezierCurve bezierCurve = new BezierCurve(bezierControlPoints);
            for (int i = 0; i < evaluationPoints; i++)
            {
                Dust dust = Dust.NewDustPerfect(center, 263);
                dust.position = center + bezierCurve.Evaluate(i / (float)evaluationPoints).RotatedBy(offsetAngle);
                dust.velocity = Vector2.Zero;
                dust.noGravity = true;
                dust.color = Color.SkyBlue;
                dust.fadeIn = 1.5f;

                dust = Dust.CloneDust(dust);
                dust.position = center + bezierCurve.Evaluate(i / (float)evaluationPoints).RotatedBy(offsetAngle) * new Vector2(-1f, 1f);
            }
        }
        #endregion

        #region White Mark
        public void WhiteMarkExplosionDust()
        {
            // Spawn three diamonds.
            DiamondExplosionDust(Projectile.Center, Main.rand.NextFloat(8f, 12f));

            float absoluteOffsetAngle = Main.rand.NextFloat(0.4f, 0.7f);
            Vector2 offset = Vector2.UnitY.RotatedBy(absoluteOffsetAngle * -1f) * 120f;
            offset += Vector2.UnitX * 84f;
            DiamondExplosionDust(Projectile.Center + offset, Main.rand.NextFloat(6f, 11f));

            absoluteOffsetAngle = Main.rand.NextFloat(0.4f, 0.7f);
            offset = Vector2.UnitY.RotatedBy(absoluteOffsetAngle) * 120f;
            offset -= Vector2.UnitX * 84f;
            DiamondExplosionDust(Projectile.Center + offset, Main.rand.NextFloat(6f, 11f));
        }
        public void DiamondExplosionDust(Vector2 center, float diamondBurstSpeed, float offsetAngle = 0f)
        {
            int dustCount = 80;
            for (int i = 0; i < dustCount; i++)
            {
                Vector2 startingVelocity = Vector2.Zero;
                Vector2 endingVelocity = Vector2.Zero;

                switch (i / (dustCount / 4))
                {
                    case 0:
                        startingVelocity = Vector2.UnitY;
                        endingVelocity = Vector2.UnitX;
                        break;
                    case 1:
                        startingVelocity = Vector2.UnitX;
                        endingVelocity = -Vector2.UnitY;
                        break;
                    case 2:
                        startingVelocity = -Vector2.UnitY;
                        endingVelocity = -Vector2.UnitX;
                        break;
                    case 3:
                        startingVelocity = -Vector2.UnitX;
                        endingVelocity = Vector2.UnitY;
                        break;
                }
                Vector2 velocity = Vector2.Lerp(startingVelocity, endingVelocity, i / (dustCount / 4f) % 1f);
                velocity = velocity.RotatedBy(offsetAngle);
                velocity *= diamondBurstSpeed;
                velocity.X *= 0.667f;

                Dust dust = Dust.NewDustPerfect(center, 263);
                dust.position = center;
                dust.velocity = velocity;
                dust.noGravity = true;
                dust.color = Color.LightSteelBlue;
                dust.fadeIn = 1.5f;
            }
        }
        #endregion

        #region Sky Blue Mark
        public void SkyBlueMarkExplosionDust()
        {
            RainbowBoltExplosionDust(Projectile.Center, Main.rand.NextFloat(-0.2f, 0.2f));
        }
        public void RainbowBoltExplosionDust(Vector2 center, float offsetAngle = 0f)
        {
            // Spawn several clouds.
            for (int i = 0; i < 5; i++)
            {
                CloudExplosionDust(center, offsetAngle);
            }
            LightningExplosionDust(center, offsetAngle);
        }
        public void CloudExplosionDust(Vector2 center, float offsetAngle = 0f)
        {
            center += Main.rand.NextVector2CircularEdge(40f, 40f);
            Vector2 offset = Main.rand.NextVector2Square(0.9f, 1.15f);
            for (float angle = 0f; angle <= MathHelper.TwoPi; angle += MathHelper.ToRadians(2f))
            {
                Vector2 initialUnitVector = angle.ToRotationVector2();
                Vector2 spawnOffset = initialUnitVector.RotatedBy(offsetAngle);
                spawnOffset = spawnOffset.RotatedByRandom(0.4f);
                spawnOffset *= new Vector2(54f, 36f) * offset;

                spawnOffset -= Vector2.UnitY.RotatedBy(offsetAngle) * 90f;

                Dust dust = Dust.NewDustPerfect(center + spawnOffset, 263);
                dust.velocity = Vector2.Zero;
                dust.noGravity = true;
                dust.scale = 2.7f;
                dust.fadeIn = 1.5f;
            }
        }
        public void LightningExplosionDust(Vector2 center, float offsetAngle = 0f)
        {
            int evaluationPoints = 65;

            // When mapped out, these points give a rough (but slightly weird) lightning bolt.
            Vector2[] controlPoints = new Vector2[]
            {
                new Vector2(-38f, -64f),
                new Vector2(-42f, -44f),
                new Vector2(-26f, -28f),
                new Vector2(-24f, -10f),
                new Vector2(-34f, -4f),
                new Vector2(-48f, -10f),
                new Vector2(-40f, 12f),
                new Vector2(-30f, 30f),
                new Vector2(-20f, 48f),
                new Vector2(-8f, 28f),
                new Vector2(2f, 6f),
                new Vector2(-8f, -4f),
                new Vector2(4f, -16f),
                new Vector2(8f, -36f),
                new Vector2(12f, -56f),
                new Vector2(-38f, -64f)
            };
            for (int i = 0; i < evaluationPoints; i++)
            {
                int currentIndex = (int)(i / (float)evaluationPoints * controlPoints.Length);
                Vector2 currentPosition = controlPoints[currentIndex];
                Vector2 nextPosition = controlPoints[(currentIndex + 1) % controlPoints.Length];

                Dust dust = Dust.NewDustPerfect(center, 263);
                dust.position = center + Vector2.Lerp(currentPosition, nextPosition, i / (float)evaluationPoints * controlPoints.Length % 0.999f).RotatedBy(offsetAngle) * 2f;
                dust.position += Vector2.UnitY.RotatedBy(offsetAngle) * 138f;
                dust.velocity = Vector2.Zero;
                dust.noGravity = true;
                dust.color = Main.hslToRgb(i / (float)evaluationPoints * 3f % 1f, 0.6f, 0.7f);
                dust.scale = 1.4f;
                dust.fadeIn = 1.5f;
            }

            center += Main.rand.NextVector2CircularEdge(120f, 120f);
            Vector2 offset = Main.rand.NextVector2Square(0.9f, 1.15f);
            for (float angle = 0f; angle <= MathHelper.TwoPi; angle += MathHelper.ToRadians(2f))
            {
                Vector2 initialUnitVector = angle.ToRotationVector2();
                Vector2 spawnOffset = initialUnitVector.RotatedBy(offsetAngle);
                spawnOffset = spawnOffset.RotatedByRandom(0.4f);
                spawnOffset *= new Vector2(130f, 84f) * offset;

                spawnOffset += Vector2.UnitY.RotatedBy(offsetAngle) * 30f;

                Dust dust = Dust.NewDustPerfect(center + spawnOffset, 263);
                dust.velocity = Vector2.Zero;
                dust.noGravity = true;
                dust.scale = 4f;
                dust.fadeIn = 1.5f;
            }
        }
        #endregion

        #region Purple Mark
        public void PurpleMarkExplosionDust()
        {
            TwilightStarExplosionDust(Projectile.Center, 14f, Color.Magenta);
            for (int i = 0; i < 6; i++)
            {
                float angle = MathHelper.TwoPi * i / 6f;
                Vector2 offset = angle.ToRotationVector2() * Main.rand.NextFloat(140f, 180f);
                TwilightStarExplosionDust(Projectile.Center + offset, 4f, Color.White, Main.rand.NextFloat(-0.2f, 0.2f));
            }
        }
        public void TwilightStarExplosionDust(Vector2 center, float petalBurstSpeed, Color starColor, float absoluteOffsetAngle = 0f)
        {
            int pointsOnStar = 6;
            for (int i = 0; i < pointsOnStar; i++)
            {
                float angle = MathHelper.Pi * 1.5f - i * MathHelper.TwoPi / pointsOnStar;
                float nextAngle = MathHelper.Pi * 1.5f - (i + 2) * MathHelper.TwoPi / pointsOnStar;
                Vector2 start = angle.ToRotationVector2();
                Vector2 end = nextAngle.ToRotationVector2();
                int pointsOnStarSegment = 35;
                for (int j = 0; j < pointsOnStarSegment; j++)
                {
                    Dust dust = Dust.NewDustPerfect(center, 263);
                    dust.scale = 1.8f;
                    dust.velocity = Vector2.Lerp(start, end, j / (float)pointsOnStarSegment) * petalBurstSpeed;
                    dust.velocity.X *= 0.7f;
                    dust.velocity = dust.velocity.RotatedBy(absoluteOffsetAngle);
                    dust.fadeIn = 1.5f;
                    dust.color = starColor;
                    dust.noGravity = true;
                }
            }
        }
        #endregion

        #region Pale Pink Mark
        public void PalePinkMarkExplosionDust()
        {
            StarlightMarkExplosionDust(Projectile.Center);
            StarlightStarExplosionDust(Projectile.Center + new Vector2(16f, 36f), 2f);
        }
        public void StarlightMarkExplosionDust(Vector2 center)
        {
            int evaluationPoints = 60;

            // When mapped out, these points give a rough swerving stream shape.
            Vector2[] bezierControlPoints = new Vector2[]
            {
                new Vector2(-38f, -58f),
                new Vector2(-22f, -34f),
                new Vector2(-10f, -22f),
                new Vector2(6f, -11f),
                new Vector2(13f, 1f),
                new Vector2(-20f, 11f),
                new Vector2(16f, 11f),
                new Vector2(26f, 6f),
                new Vector2(24f, -11f),
                new Vector2(8f, -28f),
                new Vector2(-10f, -39f),
                new Vector2(-38f, -58f)
            };
            BezierCurve bezierCurve = new BezierCurve(bezierControlPoints);

            for (int i = 0; i < evaluationPoints; i++)
            {
                Dust dust = Dust.NewDustPerfect(center, 263);
                dust.position = center + bezierCurve.Evaluate(i / (float)evaluationPoints) * new Vector2(3f, 1.7f);
                dust.velocity = Vector2.Zero;
                dust.color = i >= evaluationPoints / 2 ? Color.Cyan : Color.SkyBlue;
                dust.fadeIn = 1.5f;
                dust.scale = 1.4f;
                dust.noGravity = true;

                dust = Dust.CloneDust(dust);
                dust.position = center + bezierCurve.Evaluate(i / (float)evaluationPoints) * new Vector2(1f, -1f) + new Vector2(30f, -12f);
            }
        }
        public void StarlightStarExplosionDust(Vector2 center, float starBurstSpeed)
        {
            int dustCount = 60;
            for (int i = 0; i < dustCount; i++)
            {
                Vector2 startingVelocity = Vector2.Zero;
                Vector2 endingVelocity = Vector2.Zero;

                switch (i / (dustCount / 4))
                {
                    case 0:
                        startingVelocity = Vector2.UnitY;
                        endingVelocity = Vector2.UnitX;
                        break;
                    case 1:
                        startingVelocity = Vector2.UnitX;
                        endingVelocity = -Vector2.UnitY;
                        break;
                    case 2:
                        startingVelocity = -Vector2.UnitY;
                        endingVelocity = -Vector2.UnitX;
                        break;
                    case 3:
                        startingVelocity = -Vector2.UnitX;
                        endingVelocity = Vector2.UnitY;
                        break;
                }
                Vector2 velocity = Vector2.Lerp(startingVelocity, endingVelocity, i / (dustCount / 4f) % 1f);
                velocity *= starBurstSpeed;
                velocity.X *= 0.75f;

                Dust dust = Dust.NewDustPerfect(center, 263);
                dust.position = center;
                dust.velocity = velocity;
                dust.noGravity = true;
                dust.color = Color.Purple;
                dust.fadeIn = 1.5f;

                dust = Dust.CloneDust(dust);
                dust.velocity = dust.velocity.RotatedBy(MathHelper.PiOver4);
                dust.color = Color.White;
            }
        }
        #endregion
    }
}
