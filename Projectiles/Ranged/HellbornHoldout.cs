using System;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.NPCs;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class HellbornHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<Hellborn>();
        public override float MaxOffsetLengthFromArm => 24f;
        public override float OffsetXUpwards => -5f;
        public override float BaseOffsetY => -5f;
        public override float OffsetYDownwards => 5f;
        public override float RecoilResolveSpeed => 0.5f;

        public ref float time => ref Projectile.ai[0];
        public float cooldownTimer = 0;
        public bool onCooldown => cooldownTimer > 0;
        public SlotId SoundSlot;

        public bool Spinning => Projectile.ai[2] == 0;
        public bool failedShot = false;
        public float drawRot = 0;
        public bool hasPlayedSound = false;
        public bool hasPlayedReloadSound = false;
        public float fade = 0;
        public int hitTimer = 0;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 62;
            Projectile.height = 34;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
            Projectile.tileCollide = false;
            Projectile.ArmorPenetration = 25;
        }
        public override void HoldoutAI()
        {
            if (hitTimer > 0)
                hitTimer--;

            fade = Utils.GetLerpValue(0, 40, time, true);
            if (SoundEngine.TryGetActiveSound(SoundSlot, out var sSound) && sSound.IsPlaying)
            {
                sSound.Volume = fade * 100 * 0.5f;
                sSound.Position = Projectile.Center;
            }

            if (onCooldown)
            {
                PostFiringCooldown();
                if (!Spinning)
                    return;
            }
            if (Spinning)
            {
                float armRot = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
                float spinArmRot = (((Projectile.Center + new Vector2(0, -3).RotatedBy(drawRot * 0.6f)) - Owner.Center).SafeNormalize(Vector2.UnitX) * 5).ToRotation();
                Owner.SetCompositeArmBack(true, FrontArmStretch, armRot + 0.6f * Projectile.direction);
                Owner.SetCompositeArmFront(true, BackArmStretch, spinArmRot + MathHelper.ToRadians(-90f));
            }
            if (time > 3)
            {
                if (Spinning)
                {
                    if (Owner.CantUseHoldout())
                    {
                        Projectile.Kill();
                        sSound?.Stop();
                    }
                    else
                    {
                        OffsetLengthFromArm = MathHelper.Lerp(OffsetLengthFromArm, 9, 0.5f);
                        drawRot += 0.75f * Projectile.direction * MathHelper.Clamp(fade, 0.3f, 1f);

                        Lighting.AddLight(Projectile.Center, Color.Lerp(Color.OrangeRed, Color.White, 0.6f).ToVector3() * fade * 0.7f);
                        if (!hasPlayedSound && Main.myPlayer == Projectile.owner)
                        {
                            SoundStyle spin = new("CalamityMod/Sounds/Item/SpinningWoosh");
                            SoundSlot = SoundEngine.PlaySound(spin with { Volume = 0.01f, Pitch = -0.1f, IsLooped = true }, Projectile.Center);
                            hasPlayedSound = true;
                        }

                        Particle Smear = new CircularSmearVFX(Projectile.Center, Color.Red * Main.rand.NextFloat(0.45f, 0.6f), Main.rand.NextFloat(-8, 8), Main.rand.NextFloat(1.13f, 1.25f) * fade);
                        GeneralParticleHandler.SpawnParticle(Smear);

                        Particle Smear2 = new CircularSmearVFX(Projectile.Center, Color.OrangeRed * Main.rand.NextFloat(0.45f, 0.6f), Main.rand.NextFloat(-8, 8), Main.rand.NextFloat(0.65f, 0.72f) * fade);
                        GeneralParticleHandler.SpawnParticle(Smear2);

                        if (Main.rand.NextBool() && fade > 0.2f)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Vector2 dustPos = Projectile.Center + (i * MathHelper.Pi + drawRot * 0.25f + MathHelper.PiOver2).ToRotationVector2().RotatedByRandom(0.5f) * 35f * fade;
                                Dust dust = Dust.NewDustPerfect(dustPos, DustID.FireworksRGB, (i * MathHelper.Pi + (drawRot * Projectile.direction) * 0.25f * Math.Sign(Projectile.velocity.X)).ToRotationVector2().RotatedBy(MathHelper.ToRadians(180f) * (Projectile.direction == 1 ? 0 : 1)) * -3f * Main.rand.NextFloat(0.5f, 1f) * fade);
                                dust.noGravity = true;
                                dust.scale = Main.rand.NextFloat(0.42f, 0.62f);
                                dust.color = Color.Lerp(Color.Orange, Color.Red, Main.rand.NextFloat(0f, 1f));

                            }
                        }
                        else if (fade > 0.2f)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                Vector2 dustPos = Projectile.Center + (i * MathHelper.Pi + drawRot * 0.5f + MathHelper.PiOver2).ToRotationVector2().RotatedByRandom(0.5f) * 87f * fade;
                                Dust dust = Dust.NewDustPerfect(dustPos, DustID.RainbowMk2, (i * MathHelper.Pi + (drawRot * Projectile.direction) * 0.5f * Math.Sign(Projectile.velocity.X)).ToRotationVector2().RotatedBy(MathHelper.ToRadians(180f) * (Projectile.direction == 1 ? 0 : 1)) * -7f * Main.rand.NextFloat(0.5f, 1f) * fade);
                                dust.noGravity = true;
                                dust.scale = Main.rand.NextFloat(0.62f, 0.82f);
                                dust.color = Color.Lerp(Color.Orange, Color.Red, Main.rand.NextFloat(0f, 1f));
                            }
                        }
                    }
                }
                else if (!onCooldown)
                {
                    if (Owner.Calamity().hellbornShots > 0)
                    {
                        Owner.SetScreenshake(5f);
                        OffsetLengthFromArm -= 35f;
                        cooldownTimer = Owner.itemAnimationMax * 2;
                        Owner.Calamity().hellbornShots--;
                        SoundStyle bigShot = new("CalamityMod/Sounds/Item/HellbornShoot");
                        SoundEngine.PlaySound(bigShot with { PitchVariance = 0.15f, Volume = 0.9f }, Projectile.Center);

                        Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 7;

                        Owner.velocity += shootVelocity * -0.68f;

                        if (Main.myPlayer == Projectile.owner)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, shootVelocity, ModContent.ProjectileType<HellbornProj>(), Projectile.damage, 0, Projectile.owner);
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, -shootVelocity.RotatedBy(0.75f * Projectile.direction) * 1f, ModContent.ProjectileType<HellbornShell>(), 0, 0, Projectile.owner);
                        }

                        for (int i = 0; i <= 13; i++)
                        {
                            Dust dust = Dust.NewDustPerfect(GunTipPosition, DustID.SteampunkSteam, shootVelocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.2f, 1.5f), 80, default, Main.rand.NextFloat(0.4f, 1.3f));
                            dust.noGravity = false;
                            dust.color = Color.White;

                            Dust dust2 = Dust.NewDustPerfect(GunTipPosition, DustID.FireworksRGB, shootVelocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.1f, 1f));
                            dust2.noGravity = true;
                            dust2.scale = Main.rand.NextFloat(0.52f, 0.72f);
                            dust2.color = Color.Lerp(Color.Orange, Color.Red, Main.rand.NextFloat(0f, 1f));
                        }
                        for (int i = 0; i < 14; i++)
                        {
                            Particle smoke = new HeavySmokeParticle(GunTipPosition, shootVelocity.RotatedByRandom(0.6f) * Main.rand.NextFloat(0.2f, 1.5f), Color.White, Main.rand.Next(40, 60 + 1), Main.rand.NextFloat(0.3f, 0.6f), 0.5f, Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextBool(), required: true);
                            GeneralParticleHandler.SpawnParticle(smoke);
                        }
                    }
                    else
                    {
                        OffsetLengthFromArm -= 8f;
                        failedShot = true;
                        SoundStyle bigShot = new("CalamityMod/Sounds/Item/DudFire");
                        SoundEngine.PlaySound(bigShot with { PitchVariance = 0.15f, Volume = 0.75f }, Projectile.Center);
                        cooldownTimer = Owner.itemAnimationMax;
                    }
                }
            }
            
            time++;
        }
        private void PostFiringCooldown()
        {
            Owner.channel = true;
            cooldownTimer -= (Spinning ? 1 : 2f);

            if (cooldownTimer > 1)
            {
                if (!Owner.Calamity().mouseRight && cooldownTimer < Owner.itemAnimationMax * 1.7f && !failedShot && !Main.mouseLeftRelease)
                    Projectile.ai[2] = 0;
                if (cooldownTimer <= 18 && !hasPlayedReloadSound)
                {
                    if (!failedShot)
                    {
                        SoundStyle fire = new("CalamityMod/Sounds/Item/HellbornReload");
                        SoundEngine.PlaySound(fire with { Volume = 0.8f }, Projectile.Center);
                    }
                    hasPlayedReloadSound = true;
                }
            }
            else if (!Spinning)
            {
                if (SoundEngine.TryGetActiveSound(SoundSlot, out var sSound) && sSound.IsPlaying)
                    sSound?.Stop();
                Projectile.Kill();
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (hitTimer == 0)
            {
                modifiers.SourceDamage *= 0.08f;
                if (Owner.Calamity().hellbornShots < 12)
                {
                    Owner.Calamity().hellbornShots++;
                }
                hitTimer = Projectile.localNPCHitCooldown;
            }
            else
            {
                modifiers.SourceDamage *= 0.02f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (time < 2)
                return false;
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Ranged/Hellborn").Value;
            Texture2D spinTexture = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Ranged/Hellborn").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f) - (Owner.gravDir == -1 ? MathHelper.Pi * Owner.direction : 0f);
            Vector2 rotationPoint = texture.Size() * 0.5f;
            SpriteEffects flipSprite = (Projectile.spriteDirection * Owner.gravDir == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.EntitySpriteDraw(texture, drawPosition + (Spinning ? new Vector2(0, -8).RotatedBy(drawRot * 0.6f) : Vector2.Zero), null, Projectile.GetAlpha(lightColor), drawRotation + drawRot, rotationPoint, Projectile.scale, flipSprite);
            return false;
        }
        public override bool? CanDamage() => Spinning && time > 5 ? null : false;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 100, targetHitbox);
        public override void KillHoldoutLogic()
        {
        }
    }
}
