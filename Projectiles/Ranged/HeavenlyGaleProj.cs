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
    public class HeavenlyGaleProj : BaseIdleHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
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
        public float StringReelbackDistance => Projectile.width * StringReelbackInterpolant * 0.3f;

        // Where the string should go, relative to the center of the projectile
        public Vector2 topStringOffset => new Vector2(-40f, -56f);
        public Vector2 bottomStringOffset => new Vector2(-40f, 46f);
        public float StringHalfHeight => (Math.Abs(topStringOffset.Y) + Math.Abs(bottomStringOffset.Y)) * 0.5f;

        public float ChargeupInterpolant => Utils.GetLerpValue(HeavenlyGale.ShootDelay, HeavenlyGale.MaxChargeTime, ChargeTimer, true);
        public ref float CurrentChargingFrames => ref Projectile.ai[0];
        public ref float ChargeTimer => ref Projectile.ai[1];

        public ref float ShootDelay => ref Projectile.localAI[0];

        public override int AssociatedItemID => ModContent.ItemType<HeavenlyGale>();
        public override int IntendedProjectileType => ModContent.ProjectileType<HeavenlyGaleProj>();

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 176;
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
            Vector2 tipPosition = armPosition + Projectile.velocity * Projectile.width * 0.45f;

            // Activate shot behavior if the owner stops channeling or otherwise cannot use the weapon.
            bool activatingShoot = ShootDelay <= 0 && Main.mouseLeft && !Main.mapFullscreen && !Owner.mouseInterface;
            if (Main.myPlayer == Projectile.owner && OwnerCanShoot && activatingShoot)
            {
                SoundEngine.PlaySound(HeavenlyGale.FireSound, Projectile.Center);
                ShootDelay = Owner.ActiveItem().useAnimation;
                Projectile.netUpdate = true;
            }

            // Update damage based on current ranged damage stat, since this projectile exists regardless of if it's being fired.
            Projectile.damage = Owner.ActiveItem() is null ? 0 : Owner.GetWeaponDamage(Owner.ActiveItem());

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

                    // Update the tip position for one frame.
                    tipPosition = armPosition + arrowDirection * Projectile.width * 0.45f;

                    if (Main.myPlayer == Projectile.owner && Owner.HasAmmo(Owner.ActiveItem()))
                    {
                        Item heldItem = Owner.ActiveItem();
                        Owner.PickAmmo(heldItem, out int projectileType, out float shootSpeed, out int damage, out float knockback, out _);
                        damage = (int)(damage * damageFactor);
                        projectileType = ModContent.ProjectileType<ExoCrystalArrow>();

                        bool createLightning = ChargeTimer / HeavenlyGale.MaxChargeTime >= HeavenlyGale.ChargeLightningCreationThreshold;
                        Vector2 arrowVelocity = arrowDirection * shootSpeed;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), tipPosition, arrowVelocity, projectileType, damage, knockback, Projectile.owner, createLightning.ToInt());
                    }
                }

                ShootDelay--;
                if (ShootDelay <= 0f)
                    ChargeTimer = 0f;
            }

            // Create orange exo energy at the tip of the bow.
            Color energyColor = Color.Orange;
            Vector2 verticalOffset = Vector2.UnitY.RotatedBy(Projectile.rotation) * 8f;
            if (Math.Cos(Projectile.rotation) < 0f)
                verticalOffset *= -1f;

            if (Main.rand.NextBool(4))
            {
                SquishyLightParticle exoEnergy = new(tipPosition + verticalOffset, -Vector2.UnitY.RotatedByRandom(0.39f) * Main.rand.NextFloat(0.4f, 1.6f), 0.28f, energyColor, 25);
                GeneralParticleHandler.SpawnParticle(exoEnergy);
            }

            // Create light at the tip of the bow.
            DelegateMethods.v3_1 = energyColor.ToVector3();
            Utils.PlotTileLine(tipPosition - verticalOffset, tipPosition + verticalOffset, 10f, DelegateMethods.CastLightOpen);
            Lighting.AddLight(tipPosition, energyColor.ToVector3());

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

            Projectile.position = armPosition - Projectile.Size * 0.5f + Projectile.velocity.SafeNormalize(Vector2.UnitY) * 44f;
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
            Texture2D textureGlow = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/HeavenlyGaleProjGlow").Value;
            Vector2 origin = texture.Size() * 0.5f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            // Draw the string of the bow. It reels back in the initial frames.
            Vector2 topOfBow = Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation + topStringOffset.ToRotation()) * topStringOffset.Length();
            Vector2 bottomOfBow = Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation + bottomStringOffset.ToRotation()) * bottomStringOffset.Length();
            Vector2 endOfString = Projectile.Center - Projectile.rotation.ToRotationVector2() * (StringReelbackDistance + (1f - StringReelbackInterpolant) * 25f);

            float chargeOffset = ChargeupInterpolant * Projectile.scale * 3f;
            Color chargeColor = Color.Lerp(Color.Lime, Color.Cyan, (float)Math.Cos(Main.GlobalTimeWrappedHourly * 7.1f) * 0.5f + 0.5f) * ChargeupInterpolant * 0.6f;
            chargeColor.A = 0;

            float rotation = Projectile.rotation;
            SpriteEffects direction = SpriteEffects.None;
            if (Math.Cos(rotation) < 0f)
            {
                direction = SpriteEffects.FlipHorizontally;
                rotation += MathHelper.Pi;
            }

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
            Main.spriteBatch.Draw(textureGlow, drawPosition, null, Color.White, rotation, origin, Projectile.scale, direction, 0f);
            return false;
        }

        // The bow itself should not do contact damage.
        public override bool? CanDamage() => false;
    }
}
