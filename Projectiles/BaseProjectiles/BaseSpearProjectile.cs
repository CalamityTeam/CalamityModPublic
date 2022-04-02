using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.BaseProjectiles
{
    public abstract class BaseSpearProjectile : ModProjectile
    {
        public enum SpearType
        {
            TypicalSpear,
            GhastlyGlaiveSpear
        }
        // Spears likely suffer from the same "no-double" problems whips do because of their hide field
        // If we ever want to add a special dual-spear, be sure to not have projectile.hide active.

        // Also, be careful with the move and reelback speed. If they're bad values,
        // the spear might end up detaching from or impaling the player.

        public virtual void Behavior()
        {
            if (SpearAiType == SpearType.TypicalSpear)
            {
                // ai[0] = Speed value of the spear. Changes as time goes by.
                // localAI[0] = Special effect 0-1 flag value. Actived right before the spear goes backward.

                Player player = Main.player[projectile.owner];

                // Adjust owner stats based on this projectile
                player.direction = projectile.direction;
                player.heldProj = projectile.whoAmI;
                player.itemTime = player.itemAnimation;

                // Stick to the player
                projectile.position = player.Center - projectile.Size / 2f;

                // And move outward/inward based on the speed variable.
                projectile.position += projectile.velocity * projectile.ai[0];

                // If we're not movement, start.
                if (projectile.ai[0] == 0f)
                {
                    projectile.ai[0] = InitialSpeed;
                    projectile.netUpdate = true;
                }
                // Reel back
                if (player.itemAnimation < player.itemAnimationMax / 3)
                {
                    projectile.ai[0] -= ReelbackSpeed;

                    // If we haven't done the special effect yet (assuming there is one), do it.
                    // Note : Null Coalescing does not work in this case because we are invoking a method, not setting a value.
                    if (projectile.localAI[0] == 0f && EffectBeforeReelback != null && Main.myPlayer == projectile.owner)
                    {
                        projectile.localAI[0] = 1f;
                        EffectBeforeReelback.Invoke(projectile);
                    }
                }
                // Move forward
                else
                {
                    projectile.ai[0] += ForwardSpeed;
                }

                // If at the end of the animation, kill the projectile.
                if (player.itemAnimation == 0)
                    projectile.Kill();

                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.PiOver4;
                if (projectile.spriteDirection == -1)
                    projectile.rotation -= MathHelper.PiOver2;
            }
            else if (SpearAiType == SpearType.GhastlyGlaiveSpear)
            {
                Player player = Main.player[projectile.owner];

                Vector2 playerRelativePoint = player.RotatedRelativePoint(player.MountedCenter, true);

                projectile.direction = player.direction;
                player.heldProj = projectile.whoAmI;
                projectile.Center = playerRelativePoint;
                if (player.dead)
                {
                    projectile.Kill();
                    return;
                }
                // If the player isn't stuck, be it from the stoned or frozen debuff, do the usual AI
                if (!player.frozen)
                {
                    // Pretty much the same as regular spears.
                    if (Main.player[projectile.owner].itemAnimation < Main.player[projectile.owner].itemAnimationMax / 3)
                    {
                        if (projectile.localAI[0] == 0f && EffectBeforeReelback != null && Main.myPlayer == projectile.owner)
                        {
                            projectile.localAI[0] = 1f;
                            EffectBeforeReelback.Invoke(projectile);
                        }
                    }
                    projectile.spriteDirection = projectile.direction = player.direction;
                    // Decrement alpha if it's greater than zero. A 255 starting alpha is
                    // not required, this is just a solution for relevant edge-cases.
                    if (projectile.alpha > 0)
                    {
                        projectile.alpha -= 127;
                        if (projectile.alpha < 0)
                            projectile.alpha = 0;
                    }
                    if (projectile.localAI[0] > 0f)
                    {
                        projectile.localAI[0] -= 1f;
                    }
                    float inverseAnimationCompletion = 1f - (player.itemAnimation / (float)player.itemAnimationMax);
                    float originalVelocityDirection = projectile.velocity.ToRotation();
                    float originalVelocitySpeed = projectile.velocity.Length();

                    // The motion moves in an imaginary circle, but the cane does not because it relies on
                    // its ai[0] X multiplier, giving it the "swiping" motion.
                    Vector2 flatVelocity = Vector2.UnitX.RotatedBy(MathHelper.Pi + inverseAnimationCompletion * MathHelper.TwoPi) *
                        new Vector2(originalVelocitySpeed, projectile.ai[0]);

                    projectile.position += flatVelocity.RotatedBy(originalVelocityDirection) +
                        new Vector2(originalVelocitySpeed + TravelSpeed, 0f).RotatedBy(originalVelocityDirection);

                    // Determine how to rotate. The larger the 40 value is, the more rapidly the projectile rotates.
                    Vector2 destination = playerRelativePoint + flatVelocity.RotatedBy(originalVelocityDirection) + originalVelocityDirection.ToRotationVector2() * (originalVelocitySpeed + TravelSpeed + 40f);
                    projectile.rotation = player.AngleTo(destination) + MathHelper.PiOver4 * player.direction; //or this

                    // Rotate 180 degrees if facing to the right
                    if (projectile.spriteDirection == -1)
                        projectile.rotation += MathHelper.Pi;
                }

                // Kill the hook if the player's item use cycle is almost over and reset the reuseDelay
                // reuseDelay is typically used for burst shots, like the clockwork assult rifle, and is
                // decremented by the useTime. On reset it reverts to useAnimation - 1
                if (player.itemAnimation == 2)
                {
                    projectile.Kill();
                    player.reuseDelay = 2;
                }
            }
        }
        public virtual void ExtraBehavior()
        {

        }
        public override void AI()
        {
            Behavior();
            if ((SpearAiType == SpearType.GhastlyGlaiveSpear && !Main.player[projectile.owner].frozen) ||
                SpearAiType == SpearType.TypicalSpear)
            {
                ExtraBehavior();
            }
        }

        #region Virtual Values
        // Typical spear virtual values
        public virtual float InitialSpeed => 3f;
        public virtual float ReelbackSpeed => 1f;
        public virtual float ForwardSpeed => 0.75f;
        // Ghastly Glaive spear virtual values
        public virtual float TravelSpeed => 22f;
        // Neither
        public virtual Action<Projectile> EffectBeforeReelback => null;
        public virtual SpearType SpearAiType => SpearType.TypicalSpear;
        #endregion
    }
}
