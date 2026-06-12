using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.BaseProjectiles
{
    public abstract class BaseCustomUseStyleProjectile : ModProjectile
    {
        public bool whenSpawned = true;
        public override void SetDefaults()
        {
            // Width and height are set here so it can destroy cobwebs better lol
            Projectile.width = (int)Math.Max(HitboxSize.X, 1);
            Projectile.height = (int)Math.Max(HitboxSize.Y, 1);
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.noEnchantmentVisuals = true;
            Projectile.ContinuouslyUpdateDamageStats = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.timeLeft = (int)(Owner.HeldItem.useAnimation * (Owner.HeldItem.CountsAsClass<MeleeDamageClass>() ? Owner.inverseMeleeSpeed : 1)) + 1;
        }

        #region Fields

        /// <summary>
        /// REQUIRED. The item ID that this projectile is linked to.
        /// </summary>
        public virtual int AssignedItemID => ItemID.None;

        /// <summary>
        /// The offset in pixels of the weapon from the projectile owner's center. This works even while using AbsolutePosition.
        /// </summary>
        public Vector2 Offset = Vector2.Zero;

        /// <summary>
        /// The Player that is using the projectile.
        /// </summary>
        public virtual Player Owner => Main.player[Projectile.owner];

        /// <summary>
        /// The amount of extra size added to the projectile's scale.
        /// </summary>
        public virtual float AdditionalScale => 0;
        /// <summary>
        /// If the projectile should auto scale to it's Projectile.scale to 1 (or greater if it is a melee item)
        /// </summary>
        public virtual bool IgnoreAutoScale => false;
        /// <summary>
        /// The amount of pixels out the center of the projectile's hitbox is. Scales with Projectile.scale.
        /// </summary>
        public virtual float HitboxOutset => 30;

        /// <summary>
        /// The projectile's hitbox size in pixels. Scales with Projectile.scale.
        /// </summary>
        public virtual Vector2 HitboxSize => new Vector2(30, 30);

        /// <summary>
        /// The number of animations that this projectile has gone through. Useful for things like swing combos.
        /// </summary>
        public int NumberOfAnimations = 0;

        public float Animation = 0;

        /// <summary>
        /// If true, flips the sprite with a 45-degree sword tilt in mind.
        /// </summary>
        public bool FlipAsSword = false;

        /// <summary>
        /// If true, the sword will be active reguardless of the player being in an item use state.
        /// </summary>
        public bool IgnoreActiveAnimation = false;

        /// <summary>
        /// The offset in radians of the weapon's rotation from its current rotation.
        /// </summary>
        public float RotationOffset = 0f;

        /// <summary>
        /// The offset in radians of the rotation of the player's front arm.
        /// </summary>
        public float ArmRotationOffset = 0f;

        /// <summary>
        /// The offset in radians of the rotation of the player's back arm.
        /// </summary>
        public float ArmRotationOffsetBack = 0f;

        /// <summary>
        /// The amount of animation frames the projectile has in a vertical sheet.
        /// </summary>
        public virtual int FrameCount => 1;

        /// <summary>
        /// The individual frame currently being drawn in a vertical sheet.
        /// </summary>
        public int Frame = 0;

        /// <summary>
        /// The origin of the sprite when drawn on your character.
        /// </summary>
        /// <returns></returns>
        public virtual Vector2 SpriteOrigin => Projectile.Size / 2;

        /// <summary>
        /// Helper method for getting the rotation of the sprite plus RotationOffset, without having to make your own local variable every time.
        /// </summary>
        /// <returns></returns>
        public float FinalRotation => Projectile.rotation + RotationOffset;

        /// <summary>
        /// Useful for flipping the sprite horizontally or vertically in case of holdouts needing to be flipped based on player direction.
        /// </summary>
        public SpriteEffects spriteEffects = SpriteEffects.None;

        /// <summary>
        /// Rotation offset for the hitbox that doesn't affect the sprite. 
        /// Useful for if you have, for instance, a sword sprite at a 45 degree angle, and need that to reflect on the hitbox.
        /// </summary>
        /// <returns></returns>
        public virtual float HitboxRotationOffset => 0f;

        /// <summary>
        /// Whether or not the projectile can hit enemies at the current frame. 
        /// Useful for wind-up animations where you don't want the projectile to deal damage.
        /// </summary>
        public bool CanHit = true;

        /// <summary>
        /// Overrides the projectile owner's center as the projectile's location, so long as it isn't Vector2.Zero.
        /// If not equal to Vector2.Zero, this value will be affected by the projectile's velocity just like its normal position.
        /// </summary>
        public Vector2 AbsolutePosition = Vector2.Zero;

        /// <summary>
        /// If this field is true, the projectile will draw at all times. Otherwise, it will only draw when its associated item is in use.
        /// </summary>
        public bool DrawUnconditionally = false;

        /// <summary>
        /// The number of frames that the current use animation has progressed through.
        /// </summary>
        public float AnimationProgress = 0;
        #endregion

        #region Use Style
        /// <summary>
        /// Determines the behavior to reset to when the item is not in use.
        /// </summary>
        public virtual void ResetStyle() { }

        /// <summary>
        /// Determines the behavior of the projectile when spawned. (but it is multiplayer synced)
        /// </summary>
        public virtual void WhenSpawned() { }
        /// <summary>
        /// Determines the behavior of the projectile when the item is in use.
        /// </summary>
        public virtual void UseStyle() { }

        /// <summary>
        /// Determines the behavior of the projectile when the item use animation begins for the first time in a row.
        /// </summary>
        public virtual void OnBeginUse() { }

        /// <summary>
        /// Determines the behavior of the projectile when the item use animation ends.
        /// </summary>
        public virtual void OnEndUse() { }
        #endregion

        public override void AI()
        {
            if (whenSpawned)
            {
                WhenSpawned();
                whenSpawned = false;
                Projectile.timeLeft = (int)(Owner.HeldItem.useAnimation * (Owner.HeldItem.CountsAsClass<MeleeDamageClass>() ? Owner.inverseMeleeSpeed : 1)) + 1;
                Projectile.netUpdate = true;
            }

            bool ItemAnimationActive = Owner.ItemAnimationActive;

            if (Owner.HeldItem.type != AssignedItemID || Owner.dead)
            {
                Projectile.Kill();
            }

            Owner.Calamity().mouseWorldListener = true;
            Owner.Calamity().rightClickListener = true;

            float finalScale = (Owner.HeldItem.CountsAsClass<MeleeDamageClass>() ? Owner.GetMeleeScale() : Owner.HeldItem.scale) + AdditionalScale;
            if (!IgnoreAutoScale)
                Projectile.scale = MathHelper.Lerp(Projectile.scale, finalScale, 0.3f / Projectile.MaxUpdates);

            if (ItemAnimationActive || IgnoreActiveAnimation)
            {
                Animation++;

                UseStyle();
                Owner.heldProj = Projectile.whoAmI;
                Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + RotationOffset + ArmRotationOffset);
                Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + RotationOffset + ArmRotationOffsetBack);
            }
            else
            {
                Animation = 0;

                if (DrawUnconditionally)
                {
                    Owner.heldProj = Projectile.whoAmI;
                    Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + RotationOffset + ArmRotationOffset);
                    Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + RotationOffset + ArmRotationOffsetBack);
                }

                NumberOfAnimations = 0;
                ResetStyle();
            }

            AnimationProgress = Animation % Owner.itemAnimationMax;

            if (AbsolutePosition == Vector2.Zero)
            {
                Projectile.position = Owner.position + (Owner.Size / 2) - (Projectile.Size / 2) + Offset;
            }
            else
            {
                AbsolutePosition += Projectile.velocity;
                Projectile.position = AbsolutePosition - (Projectile.Size / 2) + Offset;
            }

            if (AnimationProgress == Owner.itemAnimationMax - 1)
            {
                OnEndUse();
                NumberOfAnimations++;
            }

            if (Owner.itemAnimation == Owner.itemAnimationMax - 1)
            {
                Projectile.timeLeft = (int)(Owner.HeldItem.useAnimation * (Owner.HeldItem.CountsAsClass<MeleeDamageClass>() ? Owner.inverseMeleeSpeed : 1)) + 1;
                OnBeginUse();
            }

            if (DrawUnconditionally) Projectile.timeLeft = Math.Max(Projectile.timeLeft, 2);
        }

        public override bool? CanHitNPC(NPC target)
        {
            bool bb = (target.immune[0] <= 0) && !target.friendly && !target.dontTakeDamage;

            return bb;
        }


        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = Owner.direction;
            base.ModifyHitNPC(target, ref modifiers);
        }

        public override bool? CanDamage()
        {
            return CanHit ? base.CanDamage() : false;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            Vector2 hitboxScale = HitboxSize * Projectile.scale;
            Vector2 cen = Projectile.Center + new Vector2(HitboxOutset * Projectile.scale, 0).RotatedBy(FinalRotation + HitboxRotationOffset);
            hitbox = new Rectangle((int)cen.X - (int)(hitboxScale.X / 2), (int)cen.Y - (int)(hitboxScale.Y / 2), (int)hitboxScale.X, (int)hitboxScale.Y);

            // Turn this on to show the hitbox, useful for testing if it's working how you want
            /*Particle blastRing = new CustomPulse(hitbox.TopLeft(), Vector2.Zero, Color.Red, "CalamityMod/Particles/LargeBloom", Vector2.One, Main.rand.NextFloat(-10, 10), 0.2f, 0.2f, 4);
            GeneralParticleHandler.SpawnParticle(blastRing);
            Particle blastRing2 = new CustomPulse(hitbox.TopRight(), Vector2.Zero, Color.Gold, "CalamityMod/Particles/LargeBloom", Vector2.One, Main.rand.NextFloat(-10, 10), 0.2f, 0.2f, 4);
            GeneralParticleHandler.SpawnParticle(blastRing2);
            Particle blastRing3 = new CustomPulse(hitbox.BottomLeft(), Vector2.Zero, Color.Green, "CalamityMod/Particles/LargeBloom", Vector2.One, Main.rand.NextFloat(-10, 10), 0.2f, 0.2f, 4);
            GeneralParticleHandler.SpawnParticle(blastRing3);
            Particle blastRing4 = new CustomPulse(hitbox.BottomRight(), Vector2.Zero, Color.Cyan, "CalamityMod/Particles/LargeBloom", Vector2.One, Main.rand.NextFloat(-10, 10), 0.2f, 0.2f, 4);
            GeneralParticleHandler.SpawnParticle(blastRing4);*/
            base.ModifyDamageHitbox(ref hitbox);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Only draw the projectile if the projectile's owner is currently using the item this projectile is attached to.
            if (Owner.itemAnimation > 0 || DrawUnconditionally)
            {
                Asset<Texture2D> tex = ModContent.Request<Texture2D>(Texture);

                float r = FlipAsSword ? MathHelper.ToRadians(90) : 0f;

                Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY), tex.Frame(1, FrameCount, 0, Frame), lightColor, Projectile.rotation + RotationOffset + r, FlipAsSword ? new Vector2(tex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, spriteEffects != SpriteEffects.None ? spriteEffects : (FlipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None));

            }
            return false; 
        }
    }
}
