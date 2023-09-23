using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Particles;
using Terraria.Graphics.Effects;
using System;

namespace CalamityMod.Projectiles.Ranged
{
    public class TitaniumRailgunScope : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public ref float Charge => ref Projectile.ai[0];
        // Stores the max charge before firing, then stores the target post-recoil rotation after firing
        public ref float MaxChargeOrTargetRotation => ref Projectile.ai[1];
        public const float BaseMaxCharge = 60f;
        public const float MinimumCharge = 18f;
        public float ChargePercent => MathHelper.Clamp(Charge / MaxChargeOrTargetRotation, 0f, 1f);

        public Player Owner => Main.player[Projectile.owner];

        public Vector2 MousePosition => Owner.Calamity().mouseWorld - Owner.MountedCenter;
        public const float WeaponLength = 52f;
        public const float MaxSightAngle = MathHelper.Pi * (2f / 3f);

        public Color ScopeColor => Color.White;

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override bool? CanDamage() => false;

        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            Owner.Calamity().mouseWorldListener = true;

            if (Owner.channel && Charge != -1)
            {
                // Only let the owner do this
                if (Projectile.owner != Main.myPlayer)
                    return;

                // Increment the charge and set the projectile's properties
                Charge++;
                Projectile.rotation = MousePosition.ToRotation();
                Projectile.Center = Projectile.rotation.ToRotationVector2() * WeaponLength + Owner.MountedCenter;

                // Set the player's properties
                Owner.heldProj = Projectile.whoAmI;
                Owner.ChangeDir(MousePosition.X >= 0 ? 1 : -1);
                Owner.itemRotation = (MousePosition * Owner.direction).ToRotation();

                // Re-increment use time so that the post-shot delay is consistent with speed increases/decreases
                Owner.itemTime++;
                Owner.itemAnimation++;
                Projectile.timeLeft = Owner.itemAnimation;

                // Play a sound to let the player know they're at max charge
                if (Charge == MaxChargeOrTargetRotation)
                    SoundEngine.PlaySound(SoundID.Item82 with { Volume = SoundID.Item82.Volume * 0.7f }, Owner.MountedCenter);
                
                // Idly emit particles every other frame while at max charge
                if (ChargePercent == 1f && Charge % 2 == 0)
                {
                    Vector2 direction = MousePosition.SafeNormalize(Vector2.UnitX);
                    Vector2 sparkVelocity = direction.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4)) * 6f;
                    CritSpark spark = new CritSpark(Owner.MountedCenter + direction * WeaponLength, sparkVelocity + Owner.velocity, Color.White, Color.LightBlue, 1f, 16);
                    GeneralParticleHandler.SpawnParticle(spark);
                }

                // Sync the projectile in MP
                Projectile.netUpdate = true;
            }
            else
            {
                // On the first frame of firing
                if (Charge != -1 && Projectile.owner == Main.myPlayer)
                {
                    // Don't fire and immediately reset cooldowns if minimum charge isn't met
                    if (Charge < MinimumCharge)
                    {
                        Projectile.netUpdate = true;
                        Projectile.Kill();
                        Owner.itemTime = 1;
                        Owner.itemAnimation = 1;
                        return;
                    }

                    // Calculate the direction of the shot
                    Vector2 direction = MousePosition.SafeNormalize(Vector2.UnitX);

                    // Apply some recoil to the player
                    Owner.velocity += direction * (ChargePercent * -5f);
                    Owner.Calamity().GeneralScreenShakePower = 4f * ChargePercent;

                    // Spawn the laser
                    int shotDamage = (int)(Projectile.damage * ChargePercent);
                    Projectile.NewProjectile(new EntitySource_ItemUse_WithAmmo(Owner, Owner.HeldItem, -1), Owner.MountedCenter + direction * WeaponLength, direction, ModContent.ProjectileType<TitaniumRailgunShot>(), shotDamage, Projectile.knockBack * ChargePercent, Projectile.owner, ai1: ChargePercent);

                    // Calculate the target end recoil and initial recoil based off of charge level
                    // Initial recoil is manually set to give a smoother recoil animation
                    float recoil = direction.RotatedBy((MathHelper.PiOver4 - MathHelper.Pi) * ChargePercent * Owner.direction).ToRotation();
                    float initialRecoil = Owner.itemRotation.ToRotationVector2().RotatedBy(MathHelper.PiOver4 * -Owner.direction * ChargePercent).ToRotation();

                    // Spawn a ring particle based off charge percent to emphasize the impact
                    float originalScale = 0.2f * ChargePercent;
                    float maxScale = 1f * ChargePercent;
                    Particle pulse = new DirectionalPulseRing(Owner.MountedCenter + direction * WeaponLength, Vector2.Zero, Color.White, new Vector2(0.5f, 1f), direction.ToRotation(), originalScale, maxScale, 30);
                    GeneralParticleHandler.SpawnParticle(pulse);

                    // Play a sound with volume scaling with charge percent
                    SoundEngine.PlaySound(SoundID.Item62 with { Volume = SoundID.Item62.Volume * ChargePercent }, Owner.MountedCenter);

                    // Summon Luxor's gift manually, as channeling tends to be.... unbalanced, to say the least, if you don't
                    if (Owner.Calamity().luxorsGift)
                    {
                        double rangedDamage = shotDamage * 0.15;
                        if (rangedDamage >= 1D)
                        {
                            float speed = 24f * ChargePercent;
                            int projectile = Projectile.NewProjectile(new EntitySource_ItemUse_WithAmmo(Owner, Owner.HeldItem, -1), Projectile.Center, direction * speed, ModContent.ProjectileType<LuxorsGiftRanged>(), (int)rangedDamage, 0f, Projectile.owner);
                            if (projectile.WithinBounds(Main.maxProjectiles))
                                Main.projectile[projectile].DamageType = DamageClass.Generic;
                        }
                    }

                    // Set the initial recoil, mark the projectile as fired, and set the target recoil
                    Owner.itemRotation = initialRecoil;
                    Charge = -1;
                    MaxChargeOrTargetRotation = recoil;
                    Projectile.netUpdate = true;
                    return;
                }

                // Step the player's target rotation towards the max recoil
                float newRotation = UpdateAimPostShotRecoil(MaxChargeOrTargetRotation.ToRotationVector2());
                Owner.heldProj = Projectile.whoAmI;
                Owner.itemRotation = newRotation;
            }    
        }

        // Gently adjusts the aim vector of the cannon to point towards the mouse.
        private float UpdateAimPostShotRecoil(Vector2 target) => Vector2.Lerp(target * Owner.direction, Owner.itemRotation.ToRotationVector2(), 0.825f).ToRotation();

        public override bool PreDraw(ref Color lightColor)
        {
            // If the projectile has already fired, don't draw sights
            if (Charge == -1)
                return false;

            float sightsSize = 700f;
            float sightsResolution = MathHelper.Lerp(0.04f, 0.2f, Math.Min(ChargePercent * 1.5f, 1));

            // Converge the sights
            float spread = (1f - ChargePercent) * MaxSightAngle;
            float halfAngle = spread / 2f;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Color sightsColor = Color.Lerp(Color.LightBlue, Color.Crimson, ChargePercent);

            //Setup the spread gradient effect.
            Effect spreadEffect = Filters.Scene["CalamityMod:SpreadTelegraph"].GetShader().Shader;
            spreadEffect.Parameters["centerOpacity"].SetValue(0.9f);
            spreadEffect.Parameters["mainOpacity"].SetValue(ChargePercent);
            spreadEffect.Parameters["halfSpreadAngle"].SetValue(halfAngle);
            spreadEffect.Parameters["edgeColor"].SetValue(sightsColor.ToVector3());
            spreadEffect.Parameters["centerColor"].SetValue(sightsColor.ToVector3());
            spreadEffect.Parameters["edgeBlendLength"].SetValue(0.07f);
            spreadEffect.Parameters["edgeBlendStrength"].SetValue(8f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, spreadEffect, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2f, texture.Height / 2f), sightsSize, 0, 0);

            //Setup the laser sights effect.
            Effect laserScopeEffect = Filters.Scene["CalamityMod:PixelatedSightLine"].GetShader().Shader;
            laserScopeEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/CertifiedCrustyNoise").Value);
            laserScopeEffect.Parameters["noiseOffset"].SetValue(Main.GameUpdateCount * -0.003f);

            laserScopeEffect.Parameters["mainOpacity"].SetValue(ChargePercent); //Opacity increases as the gun charges
            laserScopeEffect.Parameters["Resolution"].SetValue(new Vector2(sightsResolution * sightsSize));
            laserScopeEffect.Parameters["laserAngle"].SetValue(-Projectile.rotation + halfAngle);
            laserScopeEffect.Parameters["laserWidth"].SetValue(0.0025f + (float)Math.Pow(ChargePercent, 5) * ((float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f) * 0.002f + 0.002f));
            laserScopeEffect.Parameters["laserLightStrenght"].SetValue(7f);

            laserScopeEffect.Parameters["color"].SetValue(sightsColor.ToVector3());
            laserScopeEffect.Parameters["darkerColor"].SetValue(Color.Black.ToVector3());
            laserScopeEffect.Parameters["bloomSize"].SetValue(0.06f);
            laserScopeEffect.Parameters["bloomMaxOpacity"].SetValue(0.4f);
            laserScopeEffect.Parameters["bloomFadeStrenght"].SetValue(7f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, laserScopeEffect, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, 0, new Vector2(texture.Width / 2f, texture.Height / 2f), sightsSize, 0, 0);

            laserScopeEffect.Parameters["laserAngle"].SetValue(-Projectile.rotation - halfAngle);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, 0, new Vector2(texture.Width / 2f, texture.Height / 2f), sightsSize, 0, 0);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
