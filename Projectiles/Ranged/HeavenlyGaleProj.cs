using CalamityMod.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Audio;
using Terraria.ID;
using CalamityMod.Sounds;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Particles;

namespace CalamityMod.Projectiles.Ranged
{
    public class HeavenlyGaleProj : BaseIdleHoldoutProjectile
    {
        public bool OwnerCanShoot => Owner.HasAmmo(Owner.ActiveItem()) && !Owner.noItems && !Owner.CCed;

        public float StringReelbackInterpolant
        {
            get
            {
                int duration = Owner.ActiveItem().useAnimation;
                float time = duration - ShootDelay;
                float firstHalf = Utils.GetLerpValue(8f, 0f, time, true);
                float secondHalf = Utils.GetLerpValue(8f, duration * 0.6f, time, true);
                return firstHalf + secondHalf;
            }
        }

        public float ChargeupInterpolant => Utils.GetLerpValue(HeavenlyGale.ShootDelay, HeavenlyGale.MaxChargeTime, ChargeTimer, true);

        public float StringHalfHeight => Projectile.height * 0.4f;

        public float StringReelbackDistance => Projectile.width * StringReelbackInterpolant * 0.25f;

        public ref float CurrentChargingFrames => ref Projectile.ai[0];

        public ref float ChargeTimer => ref Projectile.ai[1];

        public ref float ShootDelay => ref Projectile.localAI[0];

        public override int AssociatedItemID => ModContent.ItemType<HeavenlyGale>();

        public override int IntendedProjectileType => ModContent.ProjectileType<HeavenlyGaleProj>();

        public override void SetStaticDefaults() => DisplayName.SetDefault("Heavenly Gale");

        public override void SetDefaults()
        {
            Projectile.width = 138;
            Projectile.height = 138;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.MaxUpdates = 2;
        }

        public override void SafeAI()
        {
            Vector2 armPosition = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            Vector2 tipPosition = armPosition + Projectile.velocity * Projectile.width * 0.5f;

            // Activate shot behavior if the owner stops channeling or otherwise cannot use the weapon.
            bool activatingShoot = ShootDelay <= 0 && Main.mouseLeft && !Main.mapFullscreen && !Main.blockMouse;
            if (Main.myPlayer == Projectile.owner && OwnerCanShoot && activatingShoot)
            {
                SoundEngine.PlaySound(HeavenlyGale.FireSound, Projectile.Center);
                ShootDelay = Owner.ActiveItem().useAnimation;
                Projectile.netUpdate = true;
            }

            UpdateProjectileHeldVariables(armPosition);
            ManipulatePlayerVariables();

            // Fire arrows.
            if (ShootDelay > 0f && Projectile.FinalExtraUpdate())
            {
                float shootCompletionRatio = 1f - ShootDelay / (Owner.ActiveItem().useAnimation - 1f);
                float bowAngularOffset = (float)Math.Sin(MathHelper.TwoPi * shootCompletionRatio) * 0.4f;
                float damageFactor = Utils.Remap(ChargeTimer, 0f, HeavenlyGale.MaxChargeTime, 1f, HeavenlyGale.MaxChargeDamageBoost);

                // Fire arrows.
                if (ShootDelay % HeavenlyGale.ArrowShootRate == 0)
                {
                    Vector2 arrowDirection = Projectile.velocity.RotatedBy(bowAngularOffset);
                    
                    // Release a streak of energy.
                    Color energyBoltColor = CalamityUtils.MulticolorLerp(shootCompletionRatio, CalamityUtils.ExoPalette);
                    energyBoltColor = Color.Lerp(energyBoltColor, Color.White, 0.35f);
                    SquishyLightParticle exoEnergyBolt = new(tipPosition + arrowDirection * 16f, arrowDirection * 4.5f, 0.85f, energyBoltColor, 40, 1f, 5.4f, 4f, 0.08f);
                    GeneralParticleHandler.SpawnParticle(exoEnergyBolt);
                    
                    // Update the tip positiona for one frame.
                    tipPosition = armPosition + arrowDirection * Projectile.width * 0.5f;

                    if (Main.myPlayer == Projectile.owner)
                    {
                        int arrowDamage = (int)(Projectile.damage * damageFactor);
                        bool createLightning = ChargeTimer / HeavenlyGale.MaxChargeTime >= HeavenlyGale.ChargeLightningCreationThreshold;
                        Vector2 arrowVelocity = arrowDirection * 20f;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), tipPosition, arrowVelocity, ModContent.ProjectileType<ExoCrystalArrow>(), arrowDamage, Projectile.knockBack, Projectile.owner, createLightning.ToInt());
                    }
                }

                ShootDelay--;
                if (ShootDelay <= 0f)
                    ChargeTimer = 0f;
            }

            // Create exo energy at the tip of the bow.
            if (Main.rand.NextBool(4))
            {
                Color energyColor = Color.Orange;
                Vector2 verticalOffset = Vector2.UnitY.RotatedBy(Projectile.rotation) * 8f;
                if (Math.Cos(Projectile.rotation) < 0f)
                    verticalOffset *= -1f;

                SquishyLightParticle exoEnergy = new(tipPosition + verticalOffset, -Vector2.UnitY.RotatedByRandom(0.39f) * Main.rand.NextFloat(0.4f, 1.6f), 0.28f, energyColor, 25);
                GeneralParticleHandler.SpawnParticle(exoEnergy);
            }

            // Create a puff of energy in a star shape and play a sound to indicate that the bow is at max charge.
            if (ShootDelay <= 0)
                ChargeTimer++;
            if (ChargeTimer == HeavenlyGale.MaxChargeTime)
            {
                SoundEngine.PlaySound(SoundID.Item158 with { Volume = 1.6f }, Projectile.Center);
                for (int i = 0; i < 75; i++)
                {
                    float offsetAngle = MathHelper.TwoPi * i / 75f;

                    // Parametric equations for an asteroid.
                    float unitOffsetX = (float)Math.Pow(Math.Cos(offsetAngle), 3D);
                    float unitOffsetY = (float)Math.Pow(Math.Sin(offsetAngle), 3D);
                    
                    Vector2 puffDustVelocity = new Vector2(unitOffsetX, unitOffsetY) * 5f;
                    Dust magic = Dust.NewDustPerfect(tipPosition, 267, puffDustVelocity);
                    magic.scale = 1.8f;
                    magic.fadeIn = 0.5f;
                    magic.color = CalamityUtils.MulticolorLerp(i / 75f, CalamityUtils.ExoPalette);
                    magic.noGravity = true;
                }
                ChargeTimer++;
            }
        }
        
        public void UpdateProjectileHeldVariables(Vector2 armPosition)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                float aimInterpolant = Utils.GetLerpValue(10f, 40f, Projectile.Distance(Main.MouseWorld), true);
                Vector2 oldVelocity = Projectile.velocity;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(Main.MouseWorld), aimInterpolant);
                if (Projectile.velocity != oldVelocity)
                {
                    Projectile.netSpam = 0;
                    Projectile.netUpdate = true;
                }
            }

            Projectile.position = armPosition - Projectile.Size * 0.5f + Projectile.velocity.SafeNormalize(Vector2.UnitY) * 36f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
        }

        public void ManipulatePlayerVariables()
        {
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;

            // Make the player lower their front arm a bit to indicate the pulling of the string.
            // This is precisely calculated by representing the top half of the string as a right triangle and using SOH-CAH-TOA to
            // calculate the respective angle from the appropriate widths and heights.
            float frontArmRotation = (float)Math.Atan(StringHalfHeight / MathHelper.Max(StringReelbackDistance, 0.001f) * 0.5f);
            if (Owner.direction == -1)
                frontArmRotation += MathHelper.PiOver4;
            else
                frontArmRotation = MathHelper.PiOver2 - frontArmRotation;
            frontArmRotation += Projectile.rotation + MathHelper.Pi + Owner.direction * MathHelper.PiOver2 + 0.12f;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, frontArmRotation);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() - MathHelper.PiOver2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = texture.Size() * 0.5f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            // Draw the string of the bow. It reels back in the initial frames.
            float stringNeckOffset = 40f;
            Vector2 aimDirection = Projectile.rotation.ToRotationVector2();
            Vector2 center = Projectile.Center - aimDirection * 12f;
            Vector2 topOfBow = center + Vector2.UnitY.RotatedBy(Projectile.rotation) * StringHalfHeight;
            Vector2 bottomOfBow = center - Vector2.UnitY.RotatedBy(Projectile.rotation) * StringHalfHeight;
            Vector2 endOfString = center - aimDirection * (StringReelbackDistance + (1f - StringReelbackInterpolant) * stringNeckOffset * 0.5f);

            float chargeOffset = ChargeupInterpolant * Projectile.scale * 3f;
            Color chargeColor = Color.Lerp(Color.Lime, Color.Cyan, (float)Math.Cos(Main.GlobalTimeWrappedHourly * 7.1f) * 0.5f + 0.5f) * ChargeupInterpolant * 0.6f;
            chargeColor.A = 0;

            float rotation = Projectile.rotation;
            SpriteEffects direction = SpriteEffects.None;
            if (Math.Cos(rotation) < 0f)
            {
                direction = SpriteEffects.FlipHorizontally;
                rotation += MathHelper.Pi;
                topOfBow -= aimDirection * stringNeckOffset;
            }
            else
                bottomOfBow -= aimDirection * stringNeckOffset;

            Color stringColor = new(105, 239, 145);
            Main.spriteBatch.DrawLineBetter(topOfBow, endOfString, stringColor, 2f);
            Main.spriteBatch.DrawLineBetter(bottomOfBow, endOfString, stringColor, 2f);

            // Draw a backglow effect as an indicator of charge.
            for (int i = 0; i < 8; i++)
            {
                Vector2 drawOffset = (MathHelper.TwoPi * i / 8f).ToRotationVector2() * chargeOffset;
                Main.spriteBatch.Draw(texture, drawPosition + drawOffset, null, chargeColor, rotation, origin, Projectile.scale, direction, 0f);
            }
            Main.spriteBatch.Draw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), rotation, origin, Projectile.scale, direction, 0f);
            return false;
        }

        // The bow itself should not do contact damage.
        public override bool? CanDamage() => false;
    }
}
