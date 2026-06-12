using System;
using CalamityMod.Dusts;
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
    public class ApoctosisArrayHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<ApoctosisArray>();
        public override Vector2 GunTipPosition => base.GunTipPosition - Vector2.UnitY.RotatedBy(Projectile.rotation) * 1.5f * Projectile.spriteDirection;
        public override float MaxOffsetLengthFromArm => 47f;
        public override float OffsetXUpwards => -5f;
        public override float BaseOffsetY => -13f;
        public override float OffsetYDownwards => 5f;
        public override float WeaponTurnSpeed => 0.35f;
        public override float RecoilResolveSpeed => shootingTimer >= 0 ? 0.3f : 0.05f;

        public bool firing => Projectile.ai[2] == 0;
        public ref float shootingTimer => ref Projectile.ai[0];
        public ref float manaPower => ref Projectile.ai[1];
        public Color effectsColor { get; set; } = Color.Crimson;
        public int fireRate => Owner.HeldItem.useAnimation;
        public bool fullyCharged = false;
        public int time = 0;
        public int shotNum = 0;
        public SlotId ionHum { get; set; }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void KillHoldoutLogic()
        {
            if (Owner.dead)
            {
                if (SoundEngine.TryGetActiveSound(ionHum, out var hum) && hum.IsPlaying)
                {
                    hum?.Stop();
                }
                Projectile.Kill();
            }
        }

        public override void HoldoutAI()
        {
            bool isAtMaxMana = ((float)Owner.statMana / (float)Owner.statManaMax2) >= 1;
            Projectile.timeLeft++;

            if (time == 0)
                shootingTimer = fireRate;

            if (shootingTimer < 0)
            {
                OnCooldown();
                shootingTimer++;
                if (shootingTimer == 0)
                {
                    shootingTimer = fireRate;
                    if (SoundEngine.TryGetActiveSound(ionHum, out var hum) && hum.IsPlaying)
                    {
                        hum?.Stop();
                    }
                }
                return;
            }

            if (HeldItem.type != Owner.HeldItem.type)
            {
                if (manaPower > 0)
                    shootingTimer = -75;
                if (SoundEngine.TryGetActiveSound(ionHum, out var hum) && hum.IsPlaying)
                {
                    hum?.Stop();
                }
                Projectile.Kill();
            }

            if (shootingTimer == 0)
            {
                if (firing)
                {
                    if (Owner.Calamity().mouseRight)
                    {
                        Projectile.ai[2] = 5;
                        return;
                    }
                    if (Owner.CheckMana(HeldItem, -1, true, false))
                    {
                        Shoot(false);
                        shootingTimer = fireRate;
                    }
                    else
                    {
                        SoundEngine.PlaySound(SoundID.MaxMana with { Pitch = -0.5f }, Projectile.Center);
                        shootingTimer = -40;
                    }
                }
                else // Charging mode
                {
                    if (SoundEngine.TryGetActiveSound(ionHum, out var hum) && hum.IsPlaying)
                    {
                        hum.Position = Projectile.Center;
                        hum.Pitch = MathHelper.Lerp(-0.5f, 0.1f, manaPower);
                    }
                    else
                    {
                        SoundStyle charge = new("CalamityMod/Sounds/Item/IonChargeLoop");
                        ionHum = SoundEngine.PlaySound(charge with { Volume = 0.7f, IsLooped = true }, Projectile.Center);
                    }

                    if (Owner.Calamity().mouseRight)
                    {
                        if (!isAtMaxMana)
                        {
                            // This gives mana recharge a consistent rate rather than being variable by a frame
                            // Without it the mana recharge rate would change if you started charging on an even value rather than an odd one
                            if (manaPower == 0)
                                time = 100;

                            if (time % 2 == 0)
                            {
                                Owner.statMana += (int)(Owner.statManaMax2 * 0.025f); // 80 frames to reach max mana
                                manaPower += 0.0275f; // 72 frames to reach max power
                                // Mana power reaches max at 90% mana regained

                                if (Owner.statMana > Owner.statManaMax2)
                                    Owner.statMana = Owner.statManaMax2;
                            }
                            if (manaPower > 1)
                                manaPower = 1;

                            Vector2 vel = Projectile.velocity.RotateRandom(0.7f) * Main.rand.NextFloat(7, 14);
                            Dust dust2 = Dust.NewDustPerfect(GunTipPosition + vel * 10, ModContent.DustType<LightDust>(), -vel);
                            dust2.scale = Main.rand.NextFloat(0.35f, 0.55f);
                            dust2.noGravity = true;
                            dust2.color = Main.rand.NextBool() ? Color.Lerp(effectsColor, Color.White, 0.5f) : effectsColor;
                        }

                        if (isAtMaxMana && !fullyCharged && manaPower >= 0.25f)
                        {
                            SoundStyle fullCharge = new("CalamityMod/Sounds/Item/DudFire");
                            for (int i = 0; i < 2; i++)
                                SoundEngine.PlaySound(fullCharge with { Volume = .6f, Pitch = -0.7f + i * 1.5f, MaxInstances = 2 }, Projectile.Center);
                            fullyCharged = true;
                            OffsetLengthFromArm -= 7f * manaPower;
                        }

                        OffsetLengthFromArm += 1.3f * manaPower;
                    }
                    else
                    {
                        if (fullyCharged)
                        {
                            Shoot(true);
                            fullyCharged = false;
                        }
                        else // Failure to full charge
                        {
                            SoundEngine.PlaySound(SoundID.Item109 with { Volume = 0.7f, Pitch = 1f }, Projectile.Center);
                            for (int i = 0; i < 10; i++)
                            {
                                Vector2 vel = Projectile.velocity.RotateRandom(Math.PI);
                                Dust dust2 = Dust.NewDustPerfect(GunTipPosition, ModContent.DustType<LightDust>(), -vel);
                                dust2.scale = Main.rand.NextFloat(0.95f, 1.35f) * (manaPower + 0.3f);
                                dust2.noGravity = false;
                                dust2.color = Main.rand.NextBool() ? Color.Lerp(effectsColor, Color.White, 0.5f) : effectsColor;
                            }
                        }
                        manaPower = 0;
                        shootingTimer = -75;
                        Projectile.ai[2] = 0; // Return to regular firing mode
                    }
                }
            }
            else
            {
                if (!Owner.Calamity().mouseRight && !Main.mouseLeft)
                {
                    if (SoundEngine.TryGetActiveSound(ionHum, out var hum) && hum.IsPlaying)
                    {
                        hum?.Stop();
                    }
                    Projectile.Kill();
                }
            }

            if (shootingTimer > 0)
                shootingTimer--;

            time++;
        }
        public void OnCooldown()
        {
            Projectile.timeLeft++;

            if (SoundEngine.TryGetActiveSound(ionHum, out var hum) && hum.IsPlaying)
            {
                float lerp = Utils.GetLerpValue(-60, 0, shootingTimer, true);
                hum.Position = Projectile.Center;
                hum.Pitch = MathHelper.Lerp(0f, -0.6f, lerp);
                hum.Volume = 1 - lerp;
            }

            Vector2 smokeVel = new Vector2(0, -6) * Main.rand.NextFloat(0.1f, 1.1f);
            Particle smoke = new HeavySmokeParticle(GunTipPosition, smokeVel, Color.SlateGray, Main.rand.Next(40, 60 + 1), Main.rand.NextFloat(0.3f, 0.6f), 0.5f, Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextBool());
            GeneralParticleHandler.SpawnParticle(smoke);
        }
        public void Shoot(bool big)
        {
            float manaPercent = ((float)Owner.statMana / (float)Owner.statManaMax2);
            Vector2 shootDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Vector2 firingVelocity = (shootDirection * 4);
            float damageBoost = Math.Max((manaPower * 60) * (manaPower >= 1 ? 1.5f : 1f), 1);

            if (big)
            {
                if (SoundEngine.TryGetActiveSound(ionHum, out var hum) && hum.IsPlaying)
                {
                    hum?.Stop();
                }
                
                SoundStyle fire = new("CalamityMod/Sounds/Item/ImpalerLaunch");
                for (int i = 0; i < (manaPower >= 1 ? 2 : 1); i++)
                    SoundEngine.PlaySound(fire with { Volume = 0.9f, Pitch = -0.7f, MaxInstances = 2 }, Projectile.Center);
                
                SoundStyle fire2 = new("CalamityMod/Sounds/Item/PulseRifleFire");
                SoundEngine.PlaySound(fire2 with { Volume = 0.75f, Pitch = (manaPower >= 1 ? 0.7f : 0.5f) }, Projectile.Center);

                Owner.SetScreenshake(12f * manaPower);
                float projSpeedMult = (manaPower + 0.4f) * (manaPower >= 1 ? 3f : 2f);

                float variance = Main.rand.NextFloat(-0.8f, 0.8f);
                Vector2 vel = firingVelocity * projSpeedMult;
                if (Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, vel, ModContent.ProjectileType<ApoctosisBlast>(), (int)(Projectile.damage * damageBoost), Projectile.knockBack * 4, Projectile.owner, 0, 0, 5);
                
                OffsetLengthFromArm -= 24f * manaPower;
                Owner.velocity -= Projectile.velocity * 14f * manaPower;
                Projectile.rotation += 0.5f * Projectile.direction;

                for (int i = 0; i < Math.Max(3, 30 * manaPower); i++)
                {
                    Particle sparks = new SparkParticle(GunTipPosition, Projectile.velocity.RotatedByRandom(0.7f) * Main.rand.NextFloat(7, 25), false, 35, Main.rand.NextFloat(0.5f, 0.9f), Main.rand.NextBool() ? Color.Lerp(effectsColor, Color.White, 0.5f) : effectsColor);
                    GeneralParticleHandler.SpawnParticle(sparks);
                }
                Particle pulse3 = new CustomSpark(GunTipPosition, shootDirection * 7, "CalamityMod/Particles/HighResHollowCircleHardEdgeAlt", false, 23, 0.085f * manaPower, effectsColor, new Vector2(2f, 0.7f), shrinkSpeed: 0.1f);
                GeneralParticleHandler.SpawnParticle(pulse3);
                Particle pulse4 = new CustomSpark(GunTipPosition, shootDirection * 13, "CalamityMod/Particles/HighResHollowCircleHardEdgeAlt", false, 18, 0.06f * manaPower, effectsColor, new Vector2(2f, 0.7f), shrinkSpeed: 0.1f);
                GeneralParticleHandler.SpawnParticle(pulse4);
            }
            else
            {
                SoundStyle shoot = new("CalamityMod/Sounds/Item/ApoctosisShoot");
                SoundEngine.PlaySound(shoot with { Volume = 0.25f, Pitch = 0.5f - 1.1f * manaPercent, MaxInstances = -1 }, Projectile.Center);
                if (shotNum % 2 == 0)
                {
                    if (Main.myPlayer == Projectile.owner)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, firingVelocity, ModContent.ProjectileType<IonBlast>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0);
                    for (int k = 1; k <= 10; k++)
                    {
                        Color useColor = Main.rand.NextBool() ? Color.Lerp(effectsColor, Color.White, 0.3f) : effectsColor;
                        Vector2 shootVel = (shootDirection * 15).RotatedByRandom(0.5f) * Main.rand.NextFloat(0.1f, 1.8f);

                        Dust dust2 = Dust.NewDustPerfect(GunTipPosition, ModContent.DustType<LightDust>(), shootVel);
                        dust2.scale = Main.rand.NextFloat(0.85f, 1f);
                        dust2.noGravity = true;
                        dust2.color = useColor;

                        if (k % 2 == 0)
                        {
                            Particle blast = new CustomSpark(GunTipPosition, Projectile.velocity * 3 * k, "CalamityMod/Particles/BloomLineSoftEdge", false, 11, 0.04f, useColor, new Vector2(1f, 0.8f), shrinkSpeed: 0.8f);
                            GeneralParticleHandler.SpawnParticle(blast);
                        }
                    }
                }
                else
                {
                    for (int i = -1; i <= 1; i += 2)
                    {
                        Vector2 vel = firingVelocity.RotatedBy(0.23f * i);
                        if (Main.myPlayer == Projectile.owner)
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, vel, ModContent.ProjectileType<IonBlast>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, i);

                        shootDirection = shootDirection.RotatedBy(0.23f * i);
                        for (int k = 1; k <= 10; k++)
                        {
                            Color useColor = Main.rand.NextBool() ? Color.Lerp(effectsColor, Color.White, 0.3f) : effectsColor;
                            Vector2 shootVel = (shootDirection * 15).RotatedByRandom(0.5f) * Main.rand.NextFloat(0.1f, 1.8f);

                            Dust dust2 = Dust.NewDustPerfect(GunTipPosition, ModContent.DustType<LightDust>(), shootVel);
                            dust2.scale = Main.rand.NextFloat(0.85f, 1f);
                            dust2.noGravity = true;
                            dust2.color = useColor;

                            if (k % 2 == 0)
                            {
                                Particle blast = new CustomSpark(GunTipPosition, vel.SafeNormalize(Vector2.UnitX) * 3 * k, "CalamityMod/Particles/BloomLineSoftEdge", false, 11, 0.04f, useColor, new Vector2(1f, 0.8f), shrinkSpeed: 0.8f);
                                GeneralParticleHandler.SpawnParticle(blast);
                            }
                        }
                    }
                }
                OffsetLengthFromArm -= 5f;

                shotNum++;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float manaPercent = ((float)Owner.statMana / (float)Owner.statManaMax2);
            if (time < 2)
                return false;
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Texture2D texGlow = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Magic/ApoctosisArrayGlow").Value;
            Texture2D tex2 = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
            Texture2D sparkle = ModContent.Request<Texture2D>("CalamityMod/Particles/HalfStar").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
            SpriteEffects flipSprite = (Projectile.spriteDirection * Owner.gravDir == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 shake = Main.rand.NextVector2Circular(2, 2) * manaPower;

            Main.EntitySpriteDraw(tex, drawPosition + shake, null, Projectile.GetAlpha(lightColor), drawRotation, tex.Size() * 0.5f, Projectile.scale, flipSprite);

            Main.EntitySpriteDraw(texGlow, drawPosition + shake, null, Color.White, drawRotation, texGlow.Size() * 0.5f, Projectile.scale, flipSprite);

            if (shootingTimer >= 0)
            {
                float reverseManaPower = MathHelper.Lerp(0.7f, 0.1f, manaPower > 0 ? (1 - manaPower) : manaPercent);
                for (int i = 0; i < 5; i++)
                {
                    float iMult = (1 - 0.1f * i);
                    if (manaPower > 0)
                        Main.EntitySpriteDraw(tex2, GunTipPosition - Main.screenPosition + shake, null, Color.Lerp(effectsColor, Color.White, i * 0.1f) with { A = 0 }, Main.rand.NextFloat(-5f, 5f), tex2.Size() * 0.5f, new Vector2(1f, 0.35f) * 0.75f * manaPower * Main.rand.NextFloat(0.7f, 1.3f) * iMult, flipSprite);

                    for (int b = -1; b <= 1; b += 2)
                    {
                        float sine = MathHelper.Lerp(((float)Math.Sin(Main.GlobalTimeWrappedHourly * (firing ? 20f : 35f) / MathHelper.Pi)), reverseManaPower * b, 0.75f);
                        Vector2 scale = new Vector2(0.3f, 1f * sine * b) * (Main.rand.NextFloat(3f, 4.5f) * iMult + manaPower * 1.2f);
                        float rotation = Projectile.rotation + (time * manaPower * Math.Max(i - 2, 0) * 0.2f) + MathHelper.PiOver4 * b;
                        Main.EntitySpriteDraw(sparkle, Projectile.Center - Main.screenPosition + Projectile.velocity.RotatedBy(0.44f * Projectile.direction) * 11.7f, null, Color.Lerp(effectsColor, Color.White, i * 0.1f) with { A = 0 }, rotation, sparkle.Size() * 0.5f, scale, flipSprite);
                    }
                }
            }
            return false;
        }
    }
}
