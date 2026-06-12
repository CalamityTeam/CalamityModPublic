using CalamityMod.Dusts;
using CalamityMod.Items;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class HolofibreImmolatorHoldout : BaseGunHoldoutProjectile
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public override int AssociatedItemID => ModContent.ItemType<HolofibreImmolator>();
        public override float MaxOffsetLengthFromArm => 15f;
        public override float OffsetXUpwards => -0f;
        public override float BaseOffsetY => -0f;
        public override float OffsetYDownwards => 0f;
        public override float WeaponTurnSpeed => 0.45f;

        public static float BulletSpeed = 12f;
        public SlotId ChargeSlot;
        public int time = 0;

        public ref float CurrentChargingFrames => ref Projectile.ai[0];
        public ref float ShotsLoaded => ref Projectile.ai[1];
        public int chargeMax = 0;
        public int downtime = 0;
        public static int spamDelayMax = 8;
        public int spamDelay = spamDelayMax;
        public bool ChargeLV1 => CurrentChargingFrames >= chargeMax;

        public override void KillHoldoutLogic()
        {
            if (HeldItem.type != Owner.HeldItem.type)
                Projectile.Kill();
        }
        public override void OnSpawn(IEntitySource source)
        {
            OffsetLengthFromArm = 15;
            Player Owner = Main.player[Projectile.owner];
            chargeMax = (int)(Owner.itemAnimationMax * 1.5f);
        }

        public override void HoldoutAI()
        {
            if (CurrentChargingFrames == 0)
            {
                SoundStyle sound = new("CalamityMod/Sounds/Item/ImmolatorCharge");
                ChargeSlot = SoundEngine.PlaySound(sound with { Volume = 0.7f, IsLooped = true }, Projectile.Center);
            }
            CalamityGlobalItem modItem = Owner.HeldItem.Calamity();
            Vector2 mountedCenter = Owner.MountedCenter;
            Vector2 ownerToMouse = Owner.Calamity().mouseWorld - mountedCenter;
            if (SoundEngine.TryGetActiveSound(ChargeSlot, out var ChargeSound) && ChargeSound.IsPlaying)
                ChargeSound.Position = Projectile.Center;
            if (ChargeLV1 && spamDelay > 0)
                spamDelay--;
            // Fire if the owner stops channeling or otherwise cannot use the weapon.
            bool autoSpam = (Owner.Calamity().mouseRight && !Main.mouseLeft);
            if ((!Main.mouseLeft || autoSpam) && downtime == 0 && time >= 1)
            {
                KeepRefreshingLifetime = false;

                Projectile.timeLeft = Owner.Calamity().mouseRight ? Owner.itemAnimationMax * 2 : Owner.itemAnimationMax;
                downtime = Owner.itemAnimationMax;
                if (Owner.Calamity().mouseRight && !Main.mouseLeft)
                    CurrentChargingFrames = 0;
                spamDelay = spamDelayMax;
                // Big Shot mode
                if (ChargeLV1)
                {
                    ChargeSound?.Stop();
                    SoundStyle sound = new("CalamityMod/Sounds/Item/ImmolatorFire");
                    SoundEngine.PlaySound(sound with { Volume = 0.7f, Pitch = 0.3f }, Projectile.Center);

                    Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * BulletSpeed;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        int charge2Damage = (int)(Projectile.damage * 4.5f);
                        float charge2KB = Projectile.knockBack * 3f;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, shootVelocity * 2, ModContent.ProjectileType<ImmolationArrow>(), charge2Damage, charge2KB, Projectile.owner);
                    }

                    for (int i = 0; i <= 25; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(GunTipPosition - Projectile.velocity * 15, Effects.ArsenalEffects.ArsenalPlasmaDust, shootVelocity.RotatedByRandom(MathHelper.ToRadians(15f)) * Main.rand.NextFloat(0.2f, 1.2f), 0, default, Main.rand.NextFloat(0.5f, 1.3f));
                        dust.noGravity = true;
                        dust.color = Effects.ArsenalEffects.ArsenalPlasmaColor;
                    }
                    CurrentChargingFrames = 0;
                }
                // Spread Fire mode
                else
                {
                    ChargeSound?.Stop();
                    SoundStyle sound = new("CalamityMod/Sounds/Item/ImmolatorFire");
                    SoundEngine.PlaySound(sound with { Volume = 0.7f, Pitch = Main.rand.NextFloat(-0.1f, 0.1f) }, Projectile.Center);

                    Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * BulletSpeed;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 fireVec = shootVelocity.RotatedBy((i == 0 ? 0.4f : i == 2 ? -0.4f : 0) * Utils.GetLerpValue(chargeMax * 1.1f, 0, CurrentChargingFrames, true));
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, fireVec * MathHelper.Clamp((2 * (Utils.GetLerpValue(0, chargeMax, CurrentChargingFrames, true))), 0.5f, 2), ModContent.ProjectileType<ImmolationSpray>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                    }
                    for (int i = 0; i <= 4; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(GunTipPosition - Projectile.velocity * 15, Effects.ArsenalEffects.ArsenalPlasmaDust, shootVelocity.RotatedByRandom(MathHelper.ToRadians(15f)) * Main.rand.NextFloat(0.9f, 1.2f), 0, default, Main.rand.NextFloat(0.8f, 1.3f));
                        dust.noGravity = false;
                        dust.color = Effects.ArsenalEffects.ArsenalPlasmaColor;
                        dust.fadeIn = 2;
                    }
                    CurrentChargingFrames = 0;
                }
            }
            else if (downtime == 0)
            {
                CurrentChargingFrames++;
                Projectile.timeLeft = Owner.itemAnimationMax;

                // Charge-up visuals
                if (CurrentChargingFrames >= 10)
                {
                    float particleScale = Utils.GetLerpValue(0, chargeMax, CurrentChargingFrames, true);
                    float strength = particleScale;
                    Vector3 DustLight = Effects.ArsenalEffects.ArsenalPlasmaColor.ToVector3();
                    Lighting.AddLight(GunTipPosition, DustLight * strength);
                }

                // Full charge dusts
                if (CurrentChargingFrames == chargeMax)
                {
                    ChargeSound?.Stop();
                    for (int i = 0; i < 20; i++)
                    {
                        Dust chargefull = Dust.NewDustPerfect(GunTipPosition, ModContent.DustType<LightDust>());
                        chargefull.velocity = (MathHelper.TwoPi * i / 20f).ToRotationVector2() * 4f + Owner.velocity * 0.5f;
                        chargefull.scale = Main.rand.NextFloat(0.6f, 0.8f);
                        chargefull.noGravity = false;
                        chargefull.color = Effects.ArsenalEffects.ArsenalPlasmaColor;
                    }
                    SoundStyle sound = new("CalamityMod/Sounds/Item/ImmolatorChargeLoop");
                    ChargeSlot = SoundEngine.PlaySound(sound with { Volume = 0.7f, IsLooped = true }, Projectile.Center);
                }

                if (CurrentChargingFrames > chargeMax && Main.rand.NextBool())
                {
                    Dust chargefull = Dust.NewDustPerfect(GunTipPosition, ModContent.DustType<SquashDust>());
                    chargefull.velocity = Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(5, 9);
                    chargefull.scale = Main.rand.NextFloat(0.7f, 0.85f);
                    chargefull.noGravity = true;
                    chargefull.color = Effects.ArsenalEffects.ArsenalPlasmaColor;
                    chargefull.fadeIn = 0.5f;
                }
            }

            if ((downtime == 1 && !Owner.Calamity().mouseRight) || Owner.dead)
            {
                ChargeSound?.Stop();
                Projectile.Kill();
            }

            if (downtime > 0)
                downtime--;

            time++;
        }
        public override void OnKill(int timeLeft)
        {
            if (SoundEngine.TryGetActiveSound(ChargeSlot, out var ChargeSound))
                ChargeSound?.Stop();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (time < 2)
                return false;

            Vector2 tipPosition = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(-0.05f * Projectile.direction) * 10f;

            float fade = Utils.GetLerpValue(chargeMax / 3, chargeMax, CurrentChargingFrames, true);

            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/DraedonsArsenal/HolofibreImmolator").Value;
            Texture2D glowTexture = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/DraedonsArsenal/HolofibreImmolatorGlow").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Color drawColor = Projectile.GetAlpha(lightColor);
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
            Vector2 rotationPoint = texture.Size() * 0.5f;
            SpriteEffects flipSprite = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Texture2D rechargeTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
            Texture2D pointTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/GlowSpark").Value;

            Main.EntitySpriteDraw(texture, drawPosition, null, drawColor, drawRotation, rotationPoint, Projectile.scale, flipSprite);
            Main.EntitySpriteDraw(glowTexture, drawPosition, null, Color.White, drawRotation, rotationPoint, Projectile.scale, flipSprite);

            if (CurrentChargingFrames > 0 && downtime == 0)
            {
                float randSize = Main.rand.NextFloat(0.8f, 1.2f);
                Main.EntitySpriteDraw(rechargeTexture, tipPosition - Main.screenPosition, null, Effects.ArsenalEffects.ArsenalPlasmaColor with { A = 0 }, Projectile.rotation, rechargeTexture.Size() * 0.5f, 0.25f * Utils.GetLerpValue(0, chargeMax, CurrentChargingFrames, true) * randSize, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(rechargeTexture, tipPosition - Main.screenPosition, null, Color.White with { A = 0 }, Projectile.rotation, rechargeTexture.Size() * 0.5f, 0.15f * Utils.GetLerpValue(0, chargeMax, CurrentChargingFrames, true) * randSize, SpriteEffects.None, 0);

                for (int i = 0; i < 3; i++)
                    Main.EntitySpriteDraw(pointTexture, tipPosition - Main.screenPosition + (Projectile.velocity * 15 * fade), null, Color.Lerp(Color.White, Effects.ArsenalEffects.ArsenalPlasmaColor, i * 0.5f) with { A = 0 } * fade * 0.7f, Projectile.velocity.RotatedBy(MathHelper.ToRadians(90f)).ToRotation(), pointTexture.Size() * 0.5f, new Vector2(0.7f - 0.2f * i, 0.7f + 0.3f * i) * 0.035f * fade * randSize * (CurrentChargingFrames == chargeMax ? 1.5f : 1) * (0.6f + 0.2f * i), flipSprite);
            }
            return false;
        }
    }
}
