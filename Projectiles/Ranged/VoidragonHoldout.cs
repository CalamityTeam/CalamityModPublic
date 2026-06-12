using System;
using System.Runtime.InteropServices;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class VoidragonHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<Voidragon>();
        public override float MaxOffsetLengthFromArm => 40f;
        public override float OffsetXUpwards => -5f;
        public override float BaseOffsetY => -1f;
        public override float OffsetYDownwards => 5f;
        public override Vector2 GunTipPosition => Projectile.Center + (Projectile.velocity * 55).RotatedBy(-0.09f * Projectile.direction);
        public const float LaserAimLag = 0.92f; // Last Prism-esque aim lag. Higher = Slower. Last Prism is 0.92f.

        public int Time = 0;
        public int beamTimer = 500;
        public int shotCounter = 0;
        public int framesBetweenShots = 0;
        public bool swapType = false;
        public bool firingBeam = false;
        public float chargeVisTimer = 0;
        public float maxChargeSpeed = 2;
        public float chargePulseTimer = 0;
        public bool playChargeSound = true;

        public SlotId soundSlot;

        public override void KillHoldoutLogic()
        {
            //If the player is dead, kill the holdout.
            if (Owner.CantUseHoldout() || HeldItem.type != AssociatedItemID)
                Projectile.Kill();
        }
        public override void OnKill(int timeLeft)
        {
            if (SoundEngine.TryGetActiveSound(soundSlot, out var ChargeSound))
                ChargeSound?.Stop();
        }
        public override void HoldoutAI()
        {
            if (SoundEngine.TryGetActiveSound(soundSlot, out var ChargeSound) && ChargeSound.IsPlaying)
                ChargeSound.Position = Projectile.Center;

            //Spawn dust on the holdout's eyes based on the amount of damage scaling.
            for (int i = 0; i < 2; i++)
            {
                int dustType = ModContent.DustType<VoidDustInverted>();
                float rotMulti = Main.rand.NextFloat(0.3f, 1f);
                Dust dust = Dust.NewDustPerfect(GunTipPosition + (-Projectile.velocity.RotatedBy(0.15 * Projectile.direction) * 50), dustType);
                dust.scale = Main.rand.NextFloat(1.2f, 1.8f) * (Owner.Calamity().sharkGunDamageScaling * 0.02f) - rotMulti * 0.1f;
                dust.noGravity = true;
                dust.velocity = new Vector2(-1.2f, -2).RotatedBy(Projectile.rotation + (Projectile.direction == -1 ? MathHelper.TwoPi * 0.75f : 0)).RotatedByRandom(rotMulti * 0.8f) * (Main.rand.NextFloat(1f, 3.2f) - rotMulti) * (Owner.Calamity().sharkGunDamageScaling * 0.015f);
                dust.alpha = Main.rand.Next(90, 150);
                dust.color = Main.rand.NextBool() ? Color.Indigo : Color.DarkBlue;
            }
            //Smoothly scale down the gun visuals before reloading
            if (Time < 30)
            {
                if (Owner.Calamity().sharkGunDamageScaling > 0)
                    Owner.Calamity().sharkGunDamageScaling--;
            }
            //Pause to give the effect of reloading
            if (Time == 30)
            {
                Player Owner = Main.player[Projectile.owner];
                SoundEngine.PlaySound(SoundID.Item149, Projectile.Center);
                if (Main.netMode != NetmodeID.Server)
                {
                    string goreType = "VoidragonMag";
                    Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center + (-Projectile.velocity * 18), Projectile.velocity.RotatedBy(2f * -Owner.direction) * Main.rand.NextFloat(0.6f, 0.7f), Mod.Find<ModGore>(goreType).Type);
                }
                //Set the scaling to 0 whenever it reloads
                Owner.Calamity().sharkGunDamageScaling = 0;
            }
            if (Time >= 90)
            {
                if (framesBetweenShots == 0 && shotCounter <= 50)
                {
                    Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 28;
                    #region Debug Text Display
                    //Main.NewText(shotCounter);
                    //Main.NewText(Owner.Calamity().sharkGunDamageScaling);
                    #endregion
                    //Do not look in here if you value your sanity
                    #region Visuals and Sounds
                    SoundStyle fire = new("CalamityMod/Sounds/Item/GunShotTiny");
                    SoundEngine.PlaySound(fire with { Volume = 0.4f, Pitch = 0.1f, PitchVariance = 0.1f, MaxInstances = -1 }, Projectile.Center);
                    for (int i = 0; i <= 4; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(3) ? 303 : 244, (shootVelocity * Main.rand.NextFloat(0.2f, 1.1f)).RotatedByRandom(0.4f));
                        dust.noGravity = true;
                        dust.scale = Main.rand.NextFloat(0.8f, 1.4f);
                        Particle smoke = new HeavySmokeParticle(GunTipPosition, Vector2.Zero, Main.rand.NextBool() ? Color.Black : Color.Indigo * 0.6f, 10, 0.85f, 1.1f, 5f, false, 0, true);
                        GeneralParticleHandler.SpawnParticle(smoke);
                    }
                    for (int i = 0; i <= 2; i++)
                    {
                        Particle spark = new VoidSparkParticle(GunTipPosition, shootVelocity.RotatedByRandom(0.8f) * Main.rand.NextFloat(1.2f, 1.5f), false, 25, 0.1f, Main.rand.NextBool() ? Color.Indigo : Color.BlueViolet);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                    Particle sparker = new CustomPulse(GunTipPosition, Vector2.Zero, Color.Purple, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), 0.02f, 0.5f, 2, true, 0.8f);
                    GeneralParticleHandler.SpawnParticle(sparker);
                    sparker.DrawLayer = Enums.GeneralDrawLayer.AfterEverything;
                    GenericSparkle sparker2 = new GenericSparkle(GunTipPosition, Vector2.Zero, Main.rand.NextBool() ? Color.Indigo : Color.BlueViolet * 0.9f, Color.Indigo, Main.rand.NextFloat(1.4f, 1.6f), 2, 0, 2.22f);
                    GeneralParticleHandler.SpawnParticle(sparker2);
                    sparker2.DrawLayer = Enums.GeneralDrawLayer.AfterEverything;
                    GenericSparkle sparker3 = new GenericSparkle(GunTipPosition, Vector2.Zero, Main.rand.NextBool() ? Color.Indigo : Color.BlueViolet * 0.9f, Color.Indigo, Main.rand.NextFloat(2.5f, 2.6f), 2, 0, 2.68f);
                    GeneralParticleHandler.SpawnParticle(sparker3);
                    Particle center = new SnowflakeSparkle(GunTipPosition, Vector2.Zero, Color.GhostWhite, Color.Indigo, 0.8f, 2, Main.rand.NextFloat(-10, 10), 0.5f, 7);
                    GeneralParticleHandler.SpawnParticle(center);
                    center.DrawLayer = Enums.GeneralDrawLayer.AfterEverything;
                    sparker3.DrawLayer = Enums.GeneralDrawLayer.AfterEverything;
                    #endregion

                    //How many frames between firing projectiles, and how far the gun moves backward to give the effect of recoil. Change this number to edit fire rate
                    framesBetweenShots = 3;
                    OffsetLengthFromArm -= 2.5f;
                    //Here we detect which ammo the bullets will use
                    Owner.PickAmmo(Owner.HeldItem, out int bulletAMMO, out float SpeedNoUse, out int bulletDamage, out float kBackNoUse, out _);
                    //Alternate between shooting bullets and void blasts. We seperate it into two different projectiles due to the bullets needing to use a Global Projectile to track the damage multiplier
                    if (Main.myPlayer == Projectile.owner)
                    {
                        if (!swapType)
                        {
                            Projectile scalingShot = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), GunTipPosition, shootVelocity.RotatedByRandom(MathHelper.ToRadians(1.5f)), bulletAMMO, Projectile.damage, Projectile.knockBack, Projectile.owner);
                            CalamityGlobalProjectile cgp = scalingShot.Calamity();
                            cgp.sharkBullets = true;
                            //Only eject casings from the gun if bullets are fired, preventing too many at once
                            if (Main.netMode != NetmodeID.Server)
                            {
                                string goreType = "VoidragonCasing";
                                Vector2 spawnOffset = new Vector2(0, -13f);
                                Vector2 spawnPosition = Projectile.Center + (-Projectile.velocity * 18) + spawnOffset;
                                Gore.NewGore(Projectile.GetSource_FromAI(), spawnPosition, -Projectile.velocity * 4f, Mod.Find<ModGore>(goreType).Type);
                            }
                        }
                        if (swapType)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, shootVelocity.RotatedByRandom(MathHelper.ToRadians(1.5f)), ModContent.ProjectileType<VoidBlast>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                        swapType = !swapType;
                    }
                    shotCounter++;
                    //Allow a pause before firing the laser. The long delay lets the laser's charge sound play in full
                    if (shotCounter == 50)
                    {
                        framesBetweenShots = 150;
                        shotCounter++;
                    }
                }
                if (framesBetweenShots > 0)
                    framesBetweenShots--;
            }
            if (shotCounter == 51 && !firingBeam) //Charging
            {
                SoundStyle charge = new("CalamityMod/Sounds/Item/VoidragonCharge");
                if (playChargeSound)
                    soundSlot = SoundEngine.PlaySound(charge with { Volume = 0.7f, IsLooped = true, pitch = -0.5f }, Projectile.Center);
                playChargeSound = false;
                float chargeCompletion = Utils.GetLerpValue(150, 0, framesBetweenShots, true);
                chargeVisTimer = MathHelper.Lerp(0, maxChargeSpeed, chargeCompletion);
                chargePulseTimer += 0.6f + MathHelper.Lerp(0, maxChargeSpeed, MathF.Pow(chargeCompletion, 2.5f));

                if (SoundEngine.TryGetActiveSound(soundSlot, out var sSound) && sSound.IsPlaying)
                    sSound.Pitch = -0.85f + 1.5f * chargeCompletion;
            }
            else
            {
                chargePulseTimer = 0;
                chargeVisTimer = MathHelper.Lerp(chargeVisTimer, 0, 0.12f);
            }
            if (shotCounter == 51 && framesBetweenShots == 0)
            {
                //Main.NewText(beamTimer);
                float beamCompletion = Utils.GetLerpValue(500, 0, beamTimer, true);
                if (!firingBeam)
                {
                    Owner.Calamity().GeneralScreenShakePower = 10f;
                    if (SoundEngine.TryGetActiveSound(soundSlot, out var cSound))
                        cSound?.Stop();
                    SoundStyle start = new("CalamityMod/Sounds/Item/VoidragonStrongStart");
                    SoundStyle start2 = new("CalamityMod/Sounds/Item/MagnaCannonShot");
                    for (int i = 0; i < 3; i++)
                        SoundEngine.PlaySound((i > 0 ? start2 : start) with { Volume = 1f, pitch = 0f - 0.5f * i, MaxInstances = 3 }, Projectile.Center);
                    SoundStyle fire = new("CalamityMod/Sounds/Item/VoidragonLaser");
                    soundSlot = SoundEngine.PlaySound(fire with { Volume = 1f, IsLooped = true, pitch = 0f }, Projectile.Center);
                }
                if (SoundEngine.TryGetActiveSound(soundSlot, out var sSound) && sSound.IsPlaying)
                {
                    sSound.Pitch = 1f - 1.5f * MathF.Pow(beamCompletion, 10f) + 0.6f * beamCompletion;
                    sSound.Volume = 1f - 1 * MathF.Pow(beamCompletion, 10f);
                }
                firingBeam = true;
                if (firingBeam && beamTimer > 0)
                {
                    //If the player hasn't hit any shots, increase the multiplier from 0 to 1 to avoid the laser doing 0 damage
                    if (Owner.Calamity().sharkGunDamageScaling == 0)
                    {
                        Owner.Calamity().sharkGunDamageScaling++;
                    }
                    //Spawn the laser only once at the start of the timer. Change beamTimer to edit its lifetime
                    //I fucking hate this laser by the way
                    if (Main.myPlayer == Projectile.owner && beamTimer == 500)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, Projectile.velocity.SafeNormalize(Vector2.UnitX) * 5, ModContent.ProjectileType<AbyssalFire>(), (int)(Projectile.damage * 0.05f) * Owner.Calamity().sharkGunDamageScaling, Projectile.knockBack, Projectile.owner, Projectile.whoAmI);
                    beamTimer--;
                }
                else
                {
                    firingBeam = false;
                    Projectile.Kill();
                }
            }
            Time++;
        }
        public override void ManageHoldout()
        {
            Vector2 storedVelocity = Projectile.velocity;
            base.ManageHoldout();

            if (firingBeam)
            {
                Vector2 aimVector = (Main.MouseWorld - Owner.RotatedRelativePoint(Owner.MountedCenter, true)).SafeNormalize(Vector2.UnitY);
                aimVector = Vector2.Normalize(Vector2.Lerp(aimVector, Vector2.Normalize(storedVelocity), LaserAimLag));

                if (aimVector != storedVelocity)
                    Projectile.netUpdate = true;
                Projectile.velocity = aimVector;

                Projectile.rotation = Projectile.velocity.ToRotation();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Time < 2)
                return false;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D texture2 = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
            Texture2D texture3 = ModContent.Request<Texture2D>("CalamityMod/Particles/RoarPulse").Value;
            Texture2D texture4 = ModContent.Request<Texture2D>("CalamityMod/Particles/GlowOrbParticle").Value;
            Texture2D sparkle = ModContent.Request<Texture2D>("CalamityMod/Particles/HalfStar").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
            Vector2 rotationPoint = texture.Size() * 0.5f;
            SpriteEffects flipSprite = (Projectile.spriteDirection * Owner.gravDir == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 shake = Main.rand.NextVector2Circular(6, 6);

            float chargeVisual = MathF.Pow(Utils.GetLerpValue(0, maxChargeSpeed / 1.5f, chargeVisTimer, true), 1.5f);
            float finalChargeVisual = MathF.Pow(Utils.GetLerpValue(maxChargeSpeed / 1.5f, maxChargeSpeed, chargeVisTimer, true), 3f);

            int max = 30;
            float ringScaling = MathF.Pow(1 - Utils.GetLerpValue(0, max, chargePulseTimer % max, true), 1.15f);
            float ringOpacity = 1 - MathF.Abs(Utils.GetLerpValue(0.5f, 1, ringScaling, false));

            for (int i = 0; i < 22; i++)
            {
                Color auraColor = Color.Lerp(Color.Indigo, Color.BlueViolet, i * 0.01f) with { A = 0 } * 0.6f;
                Vector2 drawOffset = ((MathHelper.TwoPi * i / 22f).ToRotationVector2() * 5) + Main.rand.NextVector2Circular(7, 7);
                Main.EntitySpriteDraw(texture, drawPosition + drawOffset, null, auraColor, drawRotation, rotationPoint, Projectile.scale * Main.rand.NextFloat(0.02f, 0.025f) * Owner.Calamity().sharkGunDamageScaling, flipSprite);
            }

            Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), drawRotation, rotationPoint, Projectile.scale * Owner.gravDir, flipSprite);

            //Black aura
            for (int i = 0; i < 15; i++)
                Main.EntitySpriteDraw(texture4, GunTipPosition - Main.screenPosition, null, Color.Lerp(Color.Black, Color.Indigo with { A = 0 }, finalChargeVisual) * (0.35f + ringOpacity * 0.65f), drawRotation + Main.rand.NextFloat(-4f, 4f), texture4.Size() / 2, new Vector2(0.85f + i * 0.065f, 0.85f - i * 0.065f) * (Projectile.scale + finalChargeVisual * 1.8f) * Owner.gravDir * chargeVisual * (4.25f - ringScaling * 2.5f), flipSprite);
            
            for (int i = 0; i < 3; i ++) //Glow aura
                Main.EntitySpriteDraw(texture2, GunTipPosition - Main.screenPosition, null, Color.Lerp(Color.BlueViolet, Color.White, (i == 0 ? 0.8f : 0)) with { A = 0 }, drawRotation + Main.rand.NextFloat(-4f, 4f), texture2.Size() / 2, (Projectile.scale + finalChargeVisual * 1.8f) * Owner.gravDir * chargeVisual * 0.5f * (i == 0 ? 0.75f : 1), flipSprite);

            //Pulse Rings
            Main.EntitySpriteDraw(texture3, GunTipPosition - Main.screenPosition, null, Color.BlueViolet with { A = 0 } * ringOpacity * 0.5f, drawRotation + Main.rand.NextFloat(-4f, 4f), texture3.Size() / 2, Projectile.scale * Owner.gravDir * chargeVisual * ringScaling * 0.5f, flipSprite);

            return false;
        }
    }
}
