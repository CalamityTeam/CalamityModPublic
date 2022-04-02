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
            Player player = Main.player[projectile.owner];
            if (projectile.localAI[1] > 0f)
            {
                projectile.localAI[1] -= 1f;
            }
            // Rapidly appear
            projectile.alpha -= 42;
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            // Determine the starting velocity direction
            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = projectile.velocity.ToRotation();
            }
            float direction = (projectile.localAI[0].ToRotationVector2().X >= 0f).ToDirectionInt();
            if (projectile.ai[1] <= 0f)
            {
                direction *= -1f;
            }
            // As ai[0], the timer, goes up,
            Vector2 velocityAdditive = (direction * (projectile.ai[0] / 30f * MathHelper.TwoPi - MathHelper.PiOver2)).ToRotationVector2();

            // ai[1] = A starting rotation value. With a min of 0 and a max of pi/4
            // The larger it is, the larger the outward distance we travel.
            // It will always be compressed a bit relative to the X travel movement, however,
            // because sin(pi/4) = 1/sqrt(2), which is less than the default multiplier the X distance
            // receives: 1.
            velocityAdditive.Y *= (float)Math.Sin(projectile.ai[1]);
            if (projectile.ai[1] <= 0f)
            {
                velocityAdditive.Y *= -1f;
            }
            // Rotate by the starting velocity angle, to maintain the original rotation instead of
            // Constantly rotating upward or sideways
            velocityAdditive = velocityAdditive.RotatedBy(projectile.localAI[0]);
            projectile.ai[0] += 1f;
            if (projectile.ai[0] < 30f)
            {
                projectile.velocity += 48f * velocityAdditive;
            }
            else
            {
                projectile.Kill();
            }
            // Adjust position so that we're always sticking to the player.
            projectile.position = player.RotatedRelativePoint(player.MountedCenter, true) - projectile.Size / 2f;
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            projectile.spriteDirection = projectile.direction;
            projectile.timeLeft = 2;
            player.ChangeDir(projectile.direction);
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (projectile.velocity * projectile.direction).ToRotation();

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
                player.heldProj = projectile.whoAmI;
            centerDelta -= new Vector2(player.bodyFrame.Width - player.width, player.bodyFrame.Height - 42) / 2f;
            projectile.Center = player.RotatedRelativePoint(player.position + centerDelta, true) - projectile.velocity;

            // Cool dust
            if (projectile.alpha == 0)
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
        public Texture2D FlailTexture => Main.projectileTexture[projectile.type];

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
                Dust dust = Dust.NewDustDirect(projectile.position + projectile.velocity * 2f, projectile.width, projectile.height, ExudeDustType, 0f, 0f, 100, SpecialDrawColor, 2f);
                dust.noGravity = true;
                dust.velocity *= 2f;
                dust.velocity += projectile.localAI[0].ToRotationVector2();
                dust.fadeIn = 1.5f;
            }
            float counterMax = 18f;
            int counter = 0;
            while (counter < counterMax)
            {
                if (Main.rand.NextBool(4))
                {
                    Vector2 spawnPosition = projectile.position + projectile.velocity + projectile.velocity * (counter / counterMax);
                    Dust dust = Dust.NewDustDirect(spawnPosition, projectile.width, projectile.height, WhipDustType, 0f, 0f, 100, SpecialDrawColor, 1f);
                    dust.noGravity = true;
                    dust.fadeIn = 0.5f;
                    dust.velocity += projectile.localAI[0].ToRotationVector2();
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
            if (projectile.velocity == Vector2.Zero)
            {
                return false;
            }

            DrawHandleSprite(in lightColor);

            Vector2 normalizedVelocity = Vector2.Normalize(projectile.velocity);

            float speed = projectile.velocity.Length() + 16f - 40f * projectile.scale;

            Vector2 bodyDrawPosition = projectile.Center.Floor() + normalizedVelocity * projectile.scale * 20f;
            DrawType2BodySprite(in speed, in normalizedVelocity, in lightColor, ref bodyDrawPosition);

            bodyDrawPosition = projectile.Center.Floor() + normalizedVelocity * projectile.scale * 20f;
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
                                  projectile.Center.Floor() - Main.screenPosition + Vector2.UnitY * Main.player[projectile.owner].gfxOffY,
                                  new Rectangle?(handleFrame),
                                  lightColor,
                                  projectile.rotation + MathHelper.Pi,
                                  handleFrame.Size() / 2f - Vector2.UnitY * 4f,
                                  projectile.scale,
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
                                          bodyDrawPosition - Main.screenPosition + Vector2.UnitY * Main.player[projectile.owner].gfxOffY,
                                          new Rectangle?(type1BodyFrame),
                                          lightColor,
                                          projectile.rotation + MathHelper.Pi,
                                          new Vector2(type1BodyFrame.Width / 2, 0f),
                                          projectile.scale,
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
                                          bodyDrawPosition - Main.screenPosition + Vector2.UnitY * Main.player[projectile.owner].gfxOffY,
                                          new Rectangle?(type2BodyFrame),
                                          lightColor,
                                          projectile.rotation + MathHelper.Pi,
                                          new Vector2(type2BodyFrame.Width / 2, 0f),
                                          projectile.scale,
                                          SpriteEffects.None,
                                          0f);
                    counter += type2BodyFrame.Height * projectile.scale;
                    bodyDrawPosition += normalizedVelocity * type2BodyFrame.Height * projectile.scale;
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
                whipEndPosition - Main.screenPosition + Vector2.UnitY * Main.player[projectile.owner].gfxOffY,
                new Rectangle?(tailFrame),
                lightColor,
                projectile.rotation + MathHelper.Pi,
                FlailTexture.Frame(1, 1, 0, 0).Top(),
                projectile.scale,
                SpriteEffects.None,
                0f);
        }
        #endregion

        #region Collision and Grass Cut Logic
        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = projectile.velocity;
            Utils.PlotTileLine(projectile.Center, projectile.Center + unit, projectile.width * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CutTiles));
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float _ = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity, 16f * projectile.scale, ref _))
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}
