using System;
using System.IO;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using static CalamityMod.CalamityUtils;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Projectiles.Ranged
{
    public class AdamantiteAcceleratorHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ItemType<AdamantiteParticleAccelerator>();
        public override Vector2 GunTipPosition => Owner.MountedCenter + Projectile.rotation.ToRotationVector2() * 70f + (Vector2.UnitY * -12f * Owner.direction).RotatedBy(Projectile.rotation);
        public override float MaxOffsetLengthFromArm => base.MaxOffsetLengthFromArm;
        public override float OffsetXUpwards => base.OffsetXUpwards;
        public override float OffsetXDownwards => base.OffsetXDownwards;
        public override float BaseOffsetY => base.BaseOffsetY;
        public override float OffsetYUpwards => base.OffsetYUpwards;
        public override float OffsetYDownwards => base.OffsetYDownwards;

        public ref float ChargeTimer => ref Projectile.ai[0];
        public ref float DelayTimer => ref Projectile.ai[1];
        public ref float ChargeRate => ref Projectile.localAI[0];
        public ref float BounceBackPower => ref Projectile.localAI[1];

        private SlotId ChargeupSoundSlot;

        public override void KillHoldoutLogic()
        {
            int maxTime = AdamantiteParticleAccelerator.ChargeFrames + AdamantiteParticleAccelerator.CooldownFrames;
            if (ChargeTimer > maxTime)
            {
                if (!Owner.CantUseHoldout())
                    ResetToStart();
                else
                {
                    Projectile.Kill();
                    return;
                }
            }
        }

        // ChargeTimer is a time-dilated frame counter. DelayTimer is the frame counter for when the beams are fired.
        // ChargeRate is the rate at which the "frame" counter increases. BounceBackPower is there for the visuals of the gun recoiling in the players hands.
        public override void HoldoutAI()
        {
            // Calculate how quickly the gun should charge. Charge increases by some number close to 1 every frame.
            // Speed increasing reforges make this number greater than 1. Slowing reforges make it smaller than 1.
            if (ChargeRate == 0f)
                ChargeRate = 44f / HeldItem.useTime;

            if (ChargeTimer == 0f)
                ChargeupSoundSlot = SoundEngine.PlaySound(SoundID.DD2_WitherBeastAuraPulse with { Volume = SoundID.DD2_WitherBeastAuraPulse.Volume * 1.6f }, Projectile.Center);

            else if (SoundEngine.TryGetActiveSound(ChargeupSoundSlot, out var soundOut) && soundOut.IsPlaying)
            {
                soundOut.Sound.Pitch = MathF.Min(ChargeTimer * 2f, 1);
                soundOut.Position = Projectile.Center;
            }

            if (BounceBackPower > 0f)
                BounceBackPower -= 0.07f;

            // Increment the timer for the gun. If the timer has passed 44, destroy it.
            ChargeTimer += ChargeRate;

            // Compute the weapon's charge.
            float chargeLevel = MathHelper.Clamp(Projectile.ai[0] / AdamantiteParticleAccelerator.ChargeFrames, 0f, 1f);

            // Firing or charging?
            if (chargeLevel >= 1f)
            {
                //Fires the first beam of the positive polarity on the 1st frame
                if (DelayTimer == 0f)
                {
                    FiringEffects(new Color(235, 40, 121));
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Projectile redLaser = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), GunTipPosition, Projectile.rotation.ToRotationVector2(), ProjectileType<AdamAcceleratorBeam>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 120);
                        redLaser.Center = GunTipPosition;
                    }
                }

                //Fires the second beam of the negative polarity 8 frames after the first frame
                else if (DelayTimer == 8f)
                {
                    FiringEffects(new Color(49, 161, 246));
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Projectile blueLaser = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), GunTipPosition, Projectile.rotation.ToRotationVector2(), ProjectileType<AdamAcceleratorBeam>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, -120);
                        blueLaser.Center = GunTipPosition;
                    }
                }
                DelayTimer += 1f;
            }
        }

        public CurveSegment bounceAway = new CurveSegment(SineOutEasing, 0f, 0.5f, 0.5f);
        public CurveSegment moveBack = new CurveSegment(SineInEasing, 0.56f, 1f, -1f);
        public CurveSegment bounceBack = new CurveSegment(SineBumpEasing, 0.7f, 0f, -0.35f);
        internal float RecoilDisplacement => PiecewiseAnimation(1 - BounceBackPower, new CurveSegment[] { bounceAway, moveBack, bounceBack });

        public CurveSegment unsquish = new CurveSegment(SineOutEasing, 0f, 1f, -1f);
        public CurveSegment oversquish = new CurveSegment(SineBumpEasing, 0.7f, 0f, -0.6f);
        internal float RecoilSquish => PiecewiseAnimation(1 - MathHelper.Clamp(BounceBackPower * 2f, 0f, 1f), new CurveSegment[] { unsquish, oversquish });

        private void FiringEffects(Color color)
        {
            bool redBeam = (color == new Color(235, 40, 121));
            SoundEngine.PlaySound(SoundID.Item92 with { pitch = 0.7f, volume = 0.7f }, GunTipPosition);
            SoundStyle fire = new("CalamityMod/Sounds/Item/APAShoot");
            SoundEngine.PlaySound(fire with { pitch = (redBeam ? 0.3f : -0.2f)}, GunTipPosition);

            Particle pulse = new DirectionalPulseRing(GunTipPosition + Projectile.rotation.ToRotationVector2() * 5f, Vector2.Zero, color, new Vector2(0.5f, 1f), Projectile.rotation, 0.05f, 0.34f + Main.rand.NextFloat(0.3f), 30);
            GeneralParticleHandler.SpawnParticle(pulse);

            Owner.SetScreenshake(1f);
            BounceBackPower = 1f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D gun = Request<Texture2D>("CalamityMod/Items/Weapons/Ranged/AdamantiteParticleAccelerator").Value;

            SpriteEffects flip = Owner.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            float drawAngle = Projectile.rotation + (Owner.direction < 0 ? MathHelper.Pi : 0);
            Vector2 drawOrigin = new Vector2(Owner.direction < 0 ? gun.Width - 33f : 33f, 33f);
            Vector2 drawOffset = Owner.MountedCenter + Projectile.rotation.ToRotationVector2() * (10f - RecoilDisplacement * 5f) - Main.screenPosition;

            Vector2 scale = new Vector2(1f - 0.05f * RecoilSquish, 1f + 0.05f * RecoilSquish) * Projectile.scale;

            Main.EntitySpriteDraw(gun, drawOffset, null, lightColor, drawAngle, drawOrigin, scale, flip, 0);

            return false;
        }

        public override bool PreKill(int timeLeft)
        {
            if (!Owner.CantUseHoldout())
            {
                ResetToStart();
                return false;
            }

            return true;
        }

        public void ResetToStart()
        {
            ChargeTimer = 0f;
            DelayTimer = 0f;
        }

        public override void SendExtraAIHoldout(BinaryWriter writer)
        {
            writer.Write(ChargeRate);
            writer.Write(BounceBackPower);
        }

        public override void ReceiveExtraAIHoldout(BinaryReader reader)
        {
            ChargeRate = reader.ReadSingle();
            BounceBackPower = reader.ReadSingle();
        }
    }
}
