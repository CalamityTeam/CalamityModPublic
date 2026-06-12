using CalamityMod.Items.Weapons.Magic;
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

namespace CalamityMod.Projectiles.Magic
{
    internal class HeliumFlashHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<HeliumFlash>();
        public override float MaxOffsetLengthFromArm => 60f;
        public override float BaseOffsetY => 0f;
        public override float RecoilResolveSpeed => 0.4f;
        public override string Texture => "CalamityMod/Projectiles/Magic/HeliumFlashEmpty";
        public override Vector2 GunTipPosition => base.GunTipPosition - Vector2.UnitX.RotatedBy(Projectile.rotation) * 26;
        
        private ref float CurrentChargingFrames => ref Projectile.ai[0];
        private bool FullyCharged => CurrentChargingFrames >= HeliumFlash.FullChargeFrames;
        public SlotId HeliumChargeSlot;
        public static float BulletSpeed = 15f;
        public int time = 0;
        public int starcoreFrameCounter = 0;
        public int starcoreFrame = 0;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void KillHoldoutLogic()
        {
            if (Owner.CantUseHoldout(false) || HeldItem.type != Owner.HeldItem.type)
                Projectile.Kill();
        }

        public override void HoldoutAI()
        {
            if (SoundEngine.TryGetActiveSound(HeliumChargeSlot, out var ChargeSound) && ChargeSound.IsPlaying)
                ChargeSound.Position = Projectile.Center;

            // Starcore animation
            starcoreFrameCounter++;
            if (starcoreFrameCounter > (FullyCharged  ? 0 : 1))
            {
                starcoreFrame++;
                starcoreFrameCounter = 0;
            }
            if (starcoreFrame >= 6)
                starcoreFrame = 0;

            // Fire if the owner stops channeling or otherwise cannot use the weapon.
            if (Owner.CantUseHoldout())
            {
                KeepRefreshingLifetime = false;

                if (Projectile.ai[1] != 1f)
                {
                    Projectile.timeLeft = FullyCharged ? HeliumFlash.AftershotCooldownFrames * 3 : (int)(HeliumFlash.AftershotCooldownFrames * 1.5f);

                    ChargeSound?.Stop();

                    Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * BulletSpeed;
                    if (FullyCharged)
                    {
                        Projectile.ai[2] = 1f;
                        OffsetLengthFromArm -= 25;
                        SoundEngine.PlaySound(HeliumFlash.ChargeFire, Projectile.Center);

                        if (Main.myPlayer == Projectile.owner)
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, shootVelocity, ModContent.ProjectileType<VolatileStarcore>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0);
                        
                        Particle pulse = new CustomPulse(GunTipPosition, Vector2.Zero, Color.OrangeRed, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, Main.rand.NextFloat(-10f, 10f), 0f, 0.05f, 14);
                        GeneralParticleHandler.SpawnParticle(pulse);
                        Particle pulse2 = new CustomPulse(GunTipPosition, Vector2.Zero, Color.Red, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, Main.rand.NextFloat(-10f, 10f), 0f, 0.08f, 14);
                        GeneralParticleHandler.SpawnParticle(pulse2);

                        for (int i = 0; i < 17; i++)
                        {
                            Dust chargefull = Dust.NewDustPerfect(GunTipPosition, DustID.FireworksRGB);
                            chargefull.velocity = Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(5, 25);
                            chargefull.scale = Main.rand.NextFloat(0.65f, 0.95f);
                            chargefull.noGravity = true;
                            chargefull.color = Color.Lerp(Color.White, Main.rand.NextBool(4) ? Color.Orange : Color.OrangeRed, 0.7f);
                        }

                        Vector2 shootDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
                        Particle pulse3 = new GlowSparkParticle(GunTipPosition, shootDirection * 18, false, 6, 0.057f, Color.OrangeRed, new Vector2(1.7f, 0.8f), true);
                        GeneralParticleHandler.SpawnParticle(pulse3);
                        for (int i = 0; i <= 18; i++)
                        {
                            Vector2 sparkVelocity = shootVelocity / 2f;

                            float sparkScale1 = Main.rand.NextFloat(0.3f, 0.8f);
                            Vector2 sparkvelocity1 = sparkVelocity.RotatedByRandom(0.45f) * Main.rand.NextFloat(0.5f, 0.7f);
                            Particle spark1 = new LineParticle(GunTipPosition, sparkvelocity1, false, 40, sparkScale1, Main.rand.NextBool() ? Color.Red : Color.DarkRed);
                            GeneralParticleHandler.SpawnParticle(spark1);

                            float sparkScale2 = Main.rand.NextFloat(0.4f, 1f);
                            Vector2 sparkvelocity2 = sparkVelocity.RotatedByRandom(0.2f) * Main.rand.NextFloat(0.9f, 1.6f);
                            Particle spark2 = new LineParticle(GunTipPosition, sparkvelocity2, false, 40, sparkScale2, Main.rand.NextBool() ? Color.DarkOrange : Color.OrangeRed);
                            GeneralParticleHandler.SpawnParticle(spark2);
                        }
                    }
                    else
                    {
                        SoundStyle fire = new("CalamityMod/Sounds/Item/HeliumFlashDudFire");
                        SoundEngine.PlaySound(fire with { Volume = 0.4f, Pitch = 0.3f }, Projectile.Center);
                        for (int i = 0; i < 12; i++)
                        {
                            Dust dust2 = Dust.NewDustPerfect(GunTipPosition, DustID.FireworksRGB, Vector2.One.RotatedByRandom(100) * Main.rand.NextFloat(1f, 3.5f));
                            dust2.scale = Main.rand.NextFloat(0.55f, 0.9f);
                            dust2.noGravity = false;
                            dust2.color = Color.Lerp(Color.White, Main.rand.NextBool(4) ? Color.Orange : Color.OrangeRed, 0.7f);
                        }
                    }
                    Projectile.ai[1] = 1f;
                }
            }
            else
            {
                if (Projectile.ai[1] != 1f)
                    CurrentChargingFrames++;

                // Sounds
                if (FullyCharged)
                {
                    if ((CurrentChargingFrames - HeliumFlash.FullChargeFrames) % HeliumFlash.ChargeLoopSoundFrames == 0)
                        HeliumChargeSlot = SoundEngine.PlaySound(HeliumFlash.ChargeLoop, Projectile.Center);

                    if (Main.rand.NextBool())
                    {
                        Vector2 dustVel = Vector2.One.RotatedByRandom(100) * Main.rand.NextFloat(3, 5);
                        Dust dust2 = Dust.NewDustPerfect(GunTipPosition + dustVel, DustID.FireworksRGB, dustVel * 0.15f);
                        dust2.scale = Main.rand.NextFloat(0.35f, 0.7f);
                        dust2.noGravity = true;
                        dust2.color = Color.Lerp(Color.White, Main.rand.NextBool(4) ? Color.Orange : Color.OrangeRed, 0.7f);
                    }
                }
                else if (CurrentChargingFrames == 10)
                    HeliumChargeSlot = SoundEngine.PlaySound(HeliumFlash.Charge, Projectile.Center);

                // Charge-up visuals
                if (CurrentChargingFrames >= 10)
                {
                    if (!FullyCharged)
                    {
                        Particle streak = new ManaDrainStreak(Owner, Main.rand.NextFloat(0.06f + (CurrentChargingFrames / 180), 0.08f + (CurrentChargingFrames / 180)), Main.rand.NextVector2CircularEdge(2.5f, 2.5f) * Main.rand.NextFloat(0.3f * CurrentChargingFrames, 0.3f * CurrentChargingFrames), 0f, Color.Red, Color.Orange, 7, GunTipPosition);
                        GeneralParticleHandler.SpawnParticle(streak);
                    }
                }

                // Full charge effects
                if (CurrentChargingFrames == HeliumFlash.FullChargeFrames)
                {
                    SoundStyle fire = new("CalamityMod/Sounds/Item/HeliumFlashReady");
                    SoundEngine.PlaySound(fire with { Volume = 1f, Pitch = 0f }, Projectile.Center);
                    for (int i = 0; i < 18; i++)
                    {
                        Vector2 dustVel = Vector2.One.RotatedByRandom(100) * Main.rand.NextFloat(1, 5);
                        Dust dust2 = Dust.NewDustPerfect(GunTipPosition + dustVel, DustID.FireworksRGB, dustVel * 0.7f);
                        dust2.scale = Main.rand.NextFloat(0.45f, 0.9f);
                        dust2.noGravity = true;
                        dust2.color = Color.Lerp(Color.White, Main.rand.NextBool(4) ? Color.Orange : Color.OrangeRed, 0.7f);
                    }
                }
            }
            if (Projectile.ai[1] == 1f)
            {
                if (Projectile.ai[2] == 1 && Projectile.timeLeft == (int)(HeliumFlash.AftershotCooldownFrames * 1.4f))
                {
                    OffsetLengthFromArm += 8;
                }
                if (Projectile.ai[2] == 1 && Projectile.timeLeft == (int)(HeliumFlash.AftershotCooldownFrames * 1.1f))
                {
                    SoundStyle fire = new("CalamityMod/Sounds/Item/HeliumFlashSteamRelease");
                    SoundEngine.PlaySound(fire with { Volume = 0.6f, Pitch = 0f }, Projectile.Center);
                    for (int b = 0; b < 12; b++)
                    {
                        Vector2 smokeVel1 = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.ToRadians(84)) * 14; // Middle
                        Vector2 smokeVel2 = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.ToRadians(48f)) * 14; // Front
                        Vector2 smokeVel3 = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.ToRadians(125f)) * 14; // Back
                        for (int i = 0; i < 2; i++)
                        {
                            Particle smoke1 = new HeavySmokeParticle(GunTipPosition + smokeVel1 * 2, smokeVel1 * Main.rand.NextFloat(0.2f, 0.7f), Color.Lerp(Color.SlateGray, Color.Orange, Main.rand.NextFloat(0, 0.4f)), Main.rand.Next(15, 35 + 1), Main.rand.NextFloat(0.08f, 0.45f), 0.5f, Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextBool(), required: true);
                            GeneralParticleHandler.SpawnParticle(smoke1);
                            Particle smoke2 = new HeavySmokeParticle(GunTipPosition + smokeVel2 * 2, smokeVel2 * Main.rand.NextFloat(0.2f, 0.7f), Color.Lerp(Color.SlateGray, Color.Orange, Main.rand.NextFloat(0, 0.4f)), Main.rand.Next(15, 35 + 1), Main.rand.NextFloat(0.08f, 0.45f), 0.5f, Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextBool(), required: true);
                            GeneralParticleHandler.SpawnParticle(smoke2);
                            Particle smoke3 = new HeavySmokeParticle(GunTipPosition + smokeVel3 * 2, smokeVel3 * Main.rand.NextFloat(0.2f, 0.7f), Color.Lerp(Color.SlateGray, Color.Orange, Main.rand.NextFloat(0, 0.4f)), Main.rand.Next(15, 35 + 1), Main.rand.NextFloat(0.08f, 0.45f), 0.5f, Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextBool(), required: true);
                            GeneralParticleHandler.SpawnParticle(smoke3);

                            Dust dust1 = Dust.NewDustPerfect(GunTipPosition + smokeVel1 * 2, DustID.SteampunkSteam, smokeVel1.RotatedByRandom(0.2f) * Main.rand.NextFloat(0.03f, 0.3f), 180, default, Main.rand.NextFloat(0.3f, 1.1f));
                            dust1.noGravity = false;
                            dust1.color = Color.Lerp(Color.SlateGray, Color.Orange, Main.rand.NextFloat(0, 0.4f));
                            Dust dust2 = Dust.NewDustPerfect(GunTipPosition + smokeVel2 * 2, DustID.SteampunkSteam, smokeVel2.RotatedByRandom(0.2f) * Main.rand.NextFloat(0.03f, 0.3f), 180, default, Main.rand.NextFloat(0.3f, 1.1f));
                            dust2.noGravity = false;
                            dust2.color = Color.Lerp(Color.SlateGray, Color.Orange, Main.rand.NextFloat(0, 0.4f));
                            Dust dust3 = Dust.NewDustPerfect(GunTipPosition + smokeVel3 * 2, DustID.SteampunkSteam, smokeVel3.RotatedByRandom(0.2f) * Main.rand.NextFloat(0.03f, 0.3f), 180, default, Main.rand.NextFloat(0.3f, 1.1f));
                            dust3.noGravity = false;
                            dust3.color = Color.Lerp(Color.SlateGray, Color.Orange, Main.rand.NextFloat(0, 0.4f));

                            if (i == 0)
                            {
                                smokeVel1 *= -1;
                                smokeVel2 = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.ToRadians(-125f)) * 14;
                                smokeVel3 = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.ToRadians(-48f)) * 14;
                            }
                        }
                    }
                }

                CurrentChargingFrames *= FullyCharged ? 0 : 0.9f;
            }

            Lighting.AddLight(GunTipPosition, Color.OrangeRed.ToVector3() * 1.5f * Utils.GetLerpValue(0, HeliumFlash.FullChargeFrames, CurrentChargingFrames, true));

            time++;
        }

        public override void OnKill(int timeLeft)
        {
            if (SoundEngine.TryGetActiveSound(HeliumChargeSlot, out var ChargeSound))
                ChargeSound?.Stop();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (time < 2)
                return false;

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f) - (Owner.gravDir == -1 ? MathHelper.PiOver2 * Owner.direction : 0f);
            Vector2 rotationPoint = texture.Size() * 0.5f;
            SpriteEffects flipSprite = (Projectile.spriteDirection * Owner.gravDir == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            if (!Owner.CantUseHoldout() && !FullyCharged)
            {
                float rumble = MathHelper.Clamp(CurrentChargingFrames, 0f, HeliumFlash.FullChargeFrames);
                drawPosition += Main.rand.NextVector2Circular(rumble / 25f, rumble / 25f);
            }

            Texture2D orbTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/VolatileStarcore").Value;
            Rectangle frame = orbTexture.Frame(1, 6, 0, starcoreFrame);
            Vector2 origin = frame.Size() * 0.5f;

            Texture2D rechargeTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;

            float bonusScale = 1;
            if (CurrentChargingFrames >= HeliumFlash.FullChargeFrames)
            {
                bonusScale = MathHelper.Clamp(2 * Utils.GetLerpValue(HeliumFlash.FullChargeFrames * 1.2f, HeliumFlash.FullChargeFrames, CurrentChargingFrames, true), 1, 3);
            }

            // Glow Orb
            float randSize = Main.rand.NextFloat(0.8f, 1.2f);
            Main.EntitySpriteDraw(rechargeTexture, GunTipPosition - Main.screenPosition, null, Color.OrangeRed with { A = 0 }, Projectile.rotation, rechargeTexture.Size() * 0.5f, 0.5f * Utils.GetLerpValue(0, HeliumFlash.FullChargeFrames, CurrentChargingFrames, true) * randSize * bonusScale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(rechargeTexture, GunTipPosition - Main.screenPosition, null, Color.White with { A = 0 } * Utils.GetLerpValue(0, HeliumFlash.FullChargeFrames, CurrentChargingFrames, true) * 0.75f, Projectile.rotation, rechargeTexture.Size() * 0.5f, 0.25f * Utils.GetLerpValue(0, HeliumFlash.FullChargeFrames, CurrentChargingFrames, true) * randSize * bonusScale, SpriteEffects.None, 0);

            // Main staff
            Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), drawRotation + (MathHelper.ToRadians(45f * (Projectile.spriteDirection))), rotationPoint, Projectile.scale * Owner.gravDir, flipSprite);
            // Starcore
            Main.EntitySpriteDraw(orbTexture, GunTipPosition - Main.screenPosition, frame, Color.White, 0, origin, Projectile.scale * 0.5f * Utils.GetLerpValue(0, HeliumFlash.FullChargeFrames, CurrentChargingFrames, true), SpriteEffects.None, 0);
            return false;
        }
    }
}
