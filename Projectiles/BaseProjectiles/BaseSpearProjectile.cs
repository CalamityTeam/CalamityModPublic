using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.BaseProjectiles
{
    public abstract class BaseSpearProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
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

                Player player = Main.player[Projectile.owner];

                // Adjust owner stats based on this projectile
                player.direction = Projectile.direction;
                player.heldProj = Projectile.whoAmI;
                player.itemTime = player.itemAnimation;

                // Stick to the player
                Projectile.Center = player.RotatedRelativePoint(player.MountedCenter);

                // And move outward/inward based on the speed variable.
                Projectile.position += Projectile.velocity * Projectile.ai[0];

                // If we're not movement, start.
                if (Projectile.ai[0] == 0f)
                {
                    Projectile.ai[0] = InitialSpeed;
                    Projectile.netUpdate = true;
                }
                // Reel back
                if (player.itemAnimation < player.itemAnimationMax / 3)
                {
                    Projectile.ai[0] -= ReelbackSpeed;

                    // If we haven't done the special effect yet (assuming there is one), do it.
                    // Note : Null Coalescing does not work in this case because we are invoking a method, not setting a value.
                    if (Projectile.localAI[0] == 0f && EffectBeforeReelback != null && Main.myPlayer == Projectile.owner)
                    {
                        Projectile.localAI[0] = 1f;
                        EffectBeforeReelback.Invoke(Projectile);
                    }
                }
                // Move forward
                else
                {
                    Projectile.ai[0] += ForwardSpeed;
                }

                // If at the end of the animation, kill the projectile.
                if (player.itemAnimation == 0)
                    Projectile.Kill();

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.PiOver4;
                if (Projectile.spriteDirection == -1)
                    Projectile.rotation -= MathHelper.PiOver2;
            }
            else if (SpearAiType == SpearType.GhastlyGlaiveSpear)
            {
                Player player = Main.player[Projectile.owner];

                Vector2 playerRelativePoint = player.RotatedRelativePoint(player.MountedCenter, true);

                Projectile.direction = player.direction;
                player.heldProj = Projectile.whoAmI;
                Projectile.Center = playerRelativePoint;
                if (player.dead)
                {
                    Projectile.Kill();
                    return;
                }
                // If the player isn't stuck, be it from the stoned or frozen debuff, do the usual AI
                if (!player.frozen)
                {
                    // Pretty much the same as regular spears.
                    if (Main.player[Projectile.owner].itemAnimation < Main.player[Projectile.owner].itemAnimationMax / 3)
                    {
                        if (Projectile.localAI[0] == 0f && EffectBeforeReelback != null && Main.myPlayer == Projectile.owner)
                        {
                            Projectile.localAI[0] = 1f;
                            EffectBeforeReelback.Invoke(Projectile);
                        }
                    }
                    Projectile.spriteDirection = Projectile.direction = player.direction;
                    // Decrement alpha if it's greater than zero. A 255 starting alpha is
                    // not required, this is just a solution for relevant edge-cases.
                    if (Projectile.alpha > 0)
                    {
                        Projectile.alpha -= 127;
                        if (Projectile.alpha < 0)
                            Projectile.alpha = 0;
                    }
                    if (Projectile.localAI[0] > 0f)
                    {
                        Projectile.localAI[0] -= 1f;
                    }
                    float inverseAnimationCompletion = 1f - (player.itemAnimation / (float)player.itemAnimationMax);
                    float originalVelocityDirection = Projectile.velocity.ToRotation();
                    float originalVelocitySpeed = Projectile.velocity.Length();

                    // The motion moves in an imaginary circle, but the cane does not because it relies on
                    // its ai[0] X multiplier, giving it the "swiping" motion.
                    Vector2 flatVelocity = Vector2.UnitX.RotatedBy(MathHelper.Pi + inverseAnimationCompletion * MathHelper.TwoPi) *
                        new Vector2(originalVelocitySpeed, Projectile.ai[0]);

                    Projectile.position += flatVelocity.RotatedBy(originalVelocityDirection) +
                        new Vector2(originalVelocitySpeed + TravelSpeed, 0f).RotatedBy(originalVelocityDirection);

                    // Determine how to rotate. The larger the 40 value is, the more rapidly the projectile rotates.
                    Vector2 destination = playerRelativePoint + flatVelocity.RotatedBy(originalVelocityDirection) + originalVelocityDirection.ToRotationVector2() * (originalVelocitySpeed + TravelSpeed + 40f);
                    Projectile.rotation = player.AngleTo(destination) + MathHelper.PiOver4 * player.direction; //or this

                    // Rotate 180 degrees if facing to the right
                    if (Projectile.spriteDirection == -1)
                        Projectile.rotation += MathHelper.Pi;
                }

                // Kill the hook if the player's item use cycle is almost over and reset the reuseDelay
                // reuseDelay is typically used for burst shots, like the clockwork assult rifle, and is
                // decremented by the useTime. On reset it reverts to useAnimation - 1
                if (player.itemAnimation == 2)
                {
                    Projectile.Kill();
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
            if ((SpearAiType == SpearType.GhastlyGlaiveSpear && !Main.player[Projectile.owner].frozen) ||
                SpearAiType == SpearType.TypicalSpear)
            {
                ExtraBehavior();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (SpearAiType == SpearType.TypicalSpear)
            {
                Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
                Vector2 drawPosition = Projectile.Center - Main.screenPosition;
                Vector2 origin = Vector2.Zero;
                Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, 0, 0);
                return false;
            }
            return base.PreDraw(ref lightColor);
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
