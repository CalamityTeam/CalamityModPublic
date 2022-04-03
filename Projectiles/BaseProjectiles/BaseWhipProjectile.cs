using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.BaseProjectiles
{
    public abstract class BaseWhipProjectile : ModProjectile
    {
        // This class by default uses the values in the Mourningstar projectile
        // Note: the original solar eruption code uses hide and player.heldProj.
        // These two combined will prevent the second version of the whip from being drawn.

        public virtual void Behavior()
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.localAI[1] > 0f)
            {
                Projectile.localAI[1] -= 1f;
            }
            // Rapidly appear
            Projectile.alpha -= 42;
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
            // Determine the starting velocity direction
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = Projectile.velocity.ToRotation();
            }
            float direction = (Projectile.localAI[0].ToRotationVector2().X >= 0f).ToDirectionInt();
            if (Projectile.ai[1] <= 0f)
            {
                direction *= -1f;
            }
            // As ai[0], the timer, goes up,
            Vector2 velocityAdditive = (direction * (Projectile.ai[0] / 30f * MathHelper.TwoPi - MathHelper.PiOver2)).ToRotationVector2();

            // ai[1] = A starting rotation value. With a min of 0 and a max of pi/4
            // The larger it is, the larger the outward distance we travel.
            // It will always be compressed a bit relative to the X travel movement, however,
            // because sin(pi/4) = 1/sqrt(2), which is less than the default multiplier the X distance
            // receives: 1.
            velocityAdditive.Y *= (float)Math.Sin(Projectile.ai[1]);
            if (Projectile.ai[1] <= 0f)
            {
                velocityAdditive.Y *= -1f;
            }
            // Rotate by the starting velocity angle, to maintain the original rotation instead of
            // Constantly rotating upward or sideways
            velocityAdditive = velocityAdditive.RotatedBy(Projectile.localAI[0]);
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] < 30f)
            {
                Projectile.velocity += 48f * velocityAdditive;
            }
            else
            {
                Projectile.Kill();
            }
            // Adjust position so that we're always sticking to the player.
            Projectile.position = player.RotatedRelativePoint(player.MountedCenter, true) - Projectile.Size / 2f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();

            // Adjust the center based on player attributes.
            Vector2 centerDelta = Main.OffsetsPlayerOnhand[player.bodyFrame.Y / 56] * 2f;
            if (player.direction != 1)
            {
                centerDelta.X = player.bodyFrame.Width - centerDelta.X;
            }
            if (player.gravDir != 1f)
            {
                centerDelta.Y = player.bodyFrame.Height - centerDelta.Y;
            }
            if (player.heldProj == -1)
                player.heldProj = Projectile.whoAmI;
            centerDelta -= new Vector2(player.bodyFrame.Width - player.width, player.bodyFrame.Height - 42) / 2f;
            Projectile.Center = player.RotatedRelativePoint(player.position + centerDelta, true) - Projectile.velocity;

            // Cool dust
            if (Projectile.alpha == 0)
            {
                GenerateDust();
            }
        }
        public virtual void ExtraBehavior()
        {

        }
        public override void AI()
        {
            Behavior();
            ExtraBehavior();
        }
        public Texture2D FlailTexture => Main.projectileTexture[Projectile.type];

        #region Virtual Values
        public virtual Color SpecialDrawColor => new Color(255, 200, 0);
        public virtual int ExudeDustType => 244;
        public virtual int WhipDustType => 246;
        public virtual int HandleHeight => 54;
        public virtual int BodyType1SectionHeight => 18;
        public virtual int BodyType2SectionHeight => 18;
        public virtual int BodyType1StartY => 36;
        public virtual int BodyType2StartY => 58;
        public virtual int TailStartY => 90;
        public virtual int TailHeight => 52;
        #endregion

        #region Dust Effects
        /// <summary>
        /// Spawns dust that is emitted outward as well as dust that goes along where the whip is.
        /// </summary>
        public virtual void GenerateDust()
        {
            // Dust moving along the whip
            for (int i = 0; i < 2; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position + Projectile.velocity * 2f, Projectile.width, Projectile.height, ExudeDustType, 0f, 0f, 100, SpecialDrawColor, 2f);
                dust.noGravity = true;
                dust.velocity *= 2f;
                dust.velocity += Projectile.localAI[0].ToRotationVector2();
                dust.fadeIn = 1.5f;
            }
            float counterMax = 18f;
            int counter = 0;
            while (counter < counterMax)
            {
                if (Main.rand.NextBool(4))
                {
                    Vector2 spawnPosition = Projectile.position + Projectile.velocity + Projectile.velocity * (counter / counterMax);
                    Dust dust = Dust.NewDustDirect(spawnPosition, Projectile.width, Projectile.height, WhipDustType, 0f, 0f, 100, SpecialDrawColor, 1f);
                    dust.noGravity = true;
                    dust.fadeIn = 0.5f;
                    dust.velocity += Projectile.localAI[0].ToRotationVector2();
                    dust.noLight = true;
                }
                counter++;
            }
        }
        #endregion

        #region Draw Helpers
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            // If the velocity is zero, don't draw anything.
            // Doing so would lead to various divison by 0 errors during the normalization process.
            if (Projectile.velocity == Vector2.Zero)
            {
                return false;
            }

            DrawHandleSprite(in lightColor);

            Vector2 normalizedVelocity = Vector2.Normalize(Projectile.velocity);

            float speed = Projectile.velocity.Length() + 16f - 40f * Projectile.scale;

            Vector2 bodyDrawPosition = Projectile.Center.Floor() + normalizedVelocity * Projectile.scale * 20f;
            DrawType2BodySprite(in speed, in normalizedVelocity, in lightColor, ref bodyDrawPosition);

            bodyDrawPosition = Projectile.Center.Floor() + normalizedVelocity * Projectile.scale * 20f;
            DrawType1BodySprite(in speed, in normalizedVelocity, in lightColor, ref bodyDrawPosition);

            Vector2 whipEndPosition = bodyDrawPosition;
            DrawWhipTail(in whipEndPosition, in lightColor);
            return false;
        }

        /// <summary>
        /// Draws the handle of the whip.
        /// </summary>
        /// <param name="lightColor">The color to use when drawing.</param>
        public void DrawHandleSprite(in Color lightColor)
        {
            Rectangle handleFrame = new Rectangle(0, 0, FlailTexture.Width, HandleHeight);
            Main.spriteBatch.Draw(FlailTexture,
                                  Projectile.Center.Floor() - Main.screenPosition + Vector2.UnitY * Main.player[Projectile.owner].gfxOffY,
                                  new Rectangle?(handleFrame),
                                  lightColor,
                                  Projectile.rotation + MathHelper.Pi,
                                  handleFrame.Size() / 2f - Vector2.UnitY * 4f,
                                  Projectile.scale,
                                  SpriteEffects.None,
                                  0f);
        }

        /// <summary>
        /// Draws the first body frame of the whip.
        /// </summary>
        /// <param name="speed">The partially modified speed of the projectile. The exact value is determined as
        /// <code>projectile.velocity.Length() + 16f - 40f * projectile.scale;</code>
        /// </param>
        /// <param name="normalizedVelocity">The normalized velocity vector of the whip.</param>
        /// <param name="lightColor">The color to use when drawing.</param>
        /// <param name="bodyDrawPosition">The drawing position of the body segments. Modified in this method.</param>
        public void DrawType1BodySprite(in float speed, in Vector2 normalizedVelocity, in Color lightColor, ref Vector2 bodyDrawPosition)
        {
            Rectangle type1BodyFrame = new Rectangle(0, BodyType1StartY, FlailTexture.Width, BodyType1SectionHeight);
            bool reducedType1BodyCount = speed < 100f;
            int type1BodyDrawCount = reducedType1BodyCount ? 22 : 9;
            if (speed > 0f)
            {
                float speedRatio = speed / type1BodyDrawCount;
                bodyDrawPosition += normalizedVelocity * speedRatio * 0.25f;
                for (int i = 0; i < type1BodyDrawCount; i++)
                {
                    float drawPositionDeltaMult = speedRatio;
                    if (i == 0)
                    {
                        drawPositionDeltaMult *= 0.75f;
                    }
                    Main.spriteBatch.Draw(FlailTexture,
                                          bodyDrawPosition - Main.screenPosition + Vector2.UnitY * Main.player[Projectile.owner].gfxOffY,
                                          new Rectangle?(type1BodyFrame),
                                          lightColor,
                                          Projectile.rotation + MathHelper.Pi,
                                          new Vector2(type1BodyFrame.Width / 2, 0f),
                                          Projectile.scale,
                                          SpriteEffects.None,
                                          0f);
                    bodyDrawPosition += normalizedVelocity * drawPositionDeltaMult;
                }
            }
        }

        /// <summary>
        /// Draws the second body frame of the whip.
        /// </summary>
        /// <param name="speed">The partially modified speed of the projectile. The exact value is determined as
        /// <code>projectile.velocity.Length() + 16f - 40f * projectile.scale;</code>
        /// </param>
        /// <param name="normalizedVelocity">The normalized velocity vector of the whip.</param>
        /// <param name="lightColor">The color to use when drawing.</param>
        /// <param name="bodyDrawPosition">The drawing position of the body segments. Modified in this method.</param>
        public void DrawType2BodySprite(in float speed, in Vector2 normalizedVelocity, in Color lightColor, ref Vector2 bodyDrawPosition)
        {
            // Draw body segment without the molten rock part sticking to it.
            // From a drawing standpoint, this is the second chain type of the flail
            Rectangle type2BodyFrame = new Rectangle(0, BodyType2StartY, FlailTexture.Width, BodyType2SectionHeight);
            if (speed > 0f)
            {
                float counter = 0f;
                while (counter + 1f < speed)
                {
                    if (speed - counter < type2BodyFrame.Height)
                    {
                        type2BodyFrame.Height = (int)(speed - counter);
                    }
                    Main.spriteBatch.Draw(FlailTexture,
                                          bodyDrawPosition - Main.screenPosition + Vector2.UnitY * Main.player[Projectile.owner].gfxOffY,
                                          new Rectangle?(type2BodyFrame),
                                          lightColor,
                                          Projectile.rotation + MathHelper.Pi,
                                          new Vector2(type2BodyFrame.Width / 2, 0f),
                                          Projectile.scale,
                                          SpriteEffects.None,
                                          0f);
                    counter += type2BodyFrame.Height * Projectile.scale;
                    bodyDrawPosition += normalizedVelocity * type2BodyFrame.Height * Projectile.scale;
                }
            }
        }

        /// <summary>
        /// Draws the tail of the whip.
        /// </summary>
        /// <param name="whipEndPosition">The position to draw the tail.</param>
        /// <param name="lightColor">The color to use when drawing.</param>
        public void DrawWhipTail(in Vector2 whipEndPosition, in Color lightColor)
        {
            Rectangle tailFrame = new Rectangle(0, TailStartY, FlailTexture.Width, TailHeight);
            Main.spriteBatch.Draw(FlailTexture,
                whipEndPosition - Main.screenPosition + Vector2.UnitY * Main.player[Projectile.owner].gfxOffY,
                new Rectangle?(tailFrame),
                lightColor,
                Projectile.rotation + MathHelper.Pi,
                FlailTexture.Frame(1, 1, 0, 0).Top(),
                Projectile.scale,
                SpriteEffects.None,
                0f);
        }
        #endregion

        #region Collision and Grass Cut Logic
        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = Projectile.velocity;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + unit, Projectile.width * Projectile.scale, new Utils.PerLinePoint(DelegateMethods.CutTiles));
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float _ = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity, 16f * Projectile.scale, ref _))
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}
